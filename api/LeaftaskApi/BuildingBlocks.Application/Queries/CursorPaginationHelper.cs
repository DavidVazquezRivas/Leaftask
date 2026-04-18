using System.Text.Json;

namespace BuildingBlocks.Application.Queries;

public sealed record CursorSortFieldDefinition<TItem>(
    string Name,
    Func<TItem, IComparable> Selector,
    Func<string, IComparable> ParseCursorValue,
    Func<IComparable, string> FormatCursorValue);

public static class CursorPaginationHelper
{
    public static PaginatedResult<TDto> Paginate<TItem, TDto>(
        IReadOnlyList<TItem> items,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        IReadOnlyDictionary<string, CursorSortFieldDefinition<TItem>> fields,
        IReadOnlyList<string> defaultSort,
        Func<TItem, TDto> projector)
    {
        List<SortCriterion<TItem>> criteria = BuildCriteria(sort, fields, defaultSort);
        List<TItem> orderedItems = ApplyOrdering(items, criteria);

        if (!string.IsNullOrWhiteSpace(cursor))
        {
            if (!TryDecodeCursor(cursor, out string[] cursorValues) || cursorValues.Length != criteria.Count)
            {
                return new PaginatedResult<TDto>([], null, false);
            }

            int startIndex = orderedItems.FindIndex(item => CompareToCursor(item, criteria, cursorValues) > 0);
            if (startIndex < 0)
            {
                return new PaginatedResult<TDto>([], null, false);
            }

            orderedItems = orderedItems.Skip(startIndex).ToList();
        }

        List<TItem> page = orderedItems.Take(limit + 1).ToList();
        bool hasMore = page.Count > limit;
        if (hasMore)
        {
            page = page.Take(limit).ToList();
        }

        string? nextCursor = hasMore ? EncodeCursor(page[^1], criteria) : null;

        return new PaginatedResult<TDto>(page.Select(projector).ToList(), nextCursor, hasMore);
    }

    private static List<SortCriterion<TItem>> BuildCriteria<TItem>(
        IReadOnlyCollection<string> sort,
        IReadOnlyDictionary<string, CursorSortFieldDefinition<TItem>> fields,
        IReadOnlyList<string> defaultSort)
    {
        List<string> requestedSort = sort.Count == 0 ? [.. defaultSort] : [.. sort];

        List<SortCriterion<TItem>> criteria = [];
        foreach (string item in requestedSort)
        {
            string[] parts = item.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new InvalidOperationException($"Invalid sort definition '{item}'.");
            }

            if (!fields.TryGetValue(parts[0], out CursorSortFieldDefinition<TItem>? field))
            {
                throw new InvalidOperationException($"Unsupported sort field '{parts[0]}'.")
                {
                    Data =
                    {
                        ["SortField"] = parts[0],
                        ["AvailableFields"] = fields.Keys,
                        ["RequestedSort"] = sort,
                        ["DefaultSort"] = defaultSort
                    }
                };
            }

            criteria.Add(new SortCriterion<TItem>(field, string.Equals(parts[1], "desc", StringComparison.OrdinalIgnoreCase)));
        }

        return criteria;
    }

    private static List<TItem> ApplyOrdering<TItem>(IReadOnlyList<TItem> items, IReadOnlyList<SortCriterion<TItem>> criteria)
    {
        IEnumerable<TItem> ordered = items;
        IOrderedEnumerable<TItem>? orderedEnumerable = null;

        foreach (SortCriterion<TItem> criterion in criteria)
        {
            orderedEnumerable = orderedEnumerable is null
                ? ApplyFirstOrder(ordered, criterion)
                : ApplyNextOrder(orderedEnumerable, criterion);
        }

        return (orderedEnumerable ?? ordered).ToList();
    }

    private static IOrderedEnumerable<TItem> ApplyFirstOrder<TItem>(
        IEnumerable<TItem> source,
        SortCriterion<TItem> criterion) =>
        criterion.Descending
            ? source.OrderByDescending(criterion.Field.Selector)
            : source.OrderBy(criterion.Field.Selector);

    private static IOrderedEnumerable<TItem> ApplyNextOrder<TItem>(
        IOrderedEnumerable<TItem> source,
        SortCriterion<TItem> criterion) =>
        criterion.Descending
            ? source.ThenByDescending(criterion.Field.Selector)
            : source.ThenBy(criterion.Field.Selector);

    private static int CompareToCursor<TItem>(
        TItem item,
        IReadOnlyList<SortCriterion<TItem>> criteria,
        string[] cursorValues)
    {
        for (int index = 0; index < criteria.Count; index++)
        {
            SortCriterion<TItem> criterion = criteria[index];
            IComparable itemValue = criterion.Field.Selector(item);
            IComparable cursorValue = criterion.Field.ParseCursorValue(cursorValues[index]);
            int comparison = itemValue.CompareTo(cursorValue);

            if (comparison != 0)
            {
                return criterion.Descending ? -comparison : comparison;
            }
        }

        return 0;
    }

    private static bool TryDecodeCursor(string cursor, out string[] values)
    {
        try
        {
            values = JsonSerializer.Deserialize<string[]>(Convert.FromBase64String(cursor)) ?? [];
            return true;
        }
        catch
        {
            values = [];
            return false;
        }
    }

    private static string EncodeCursor<TItem>(TItem item, IReadOnlyList<SortCriterion<TItem>> criteria)
    {
        string[] values = criteria.Select(criterion => criterion.Field.FormatCursorValue(criterion.Field.Selector(item))).ToArray();
        return Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(values));
    }

    private sealed record SortCriterion<TItem>(CursorSortFieldDefinition<TItem> Field, bool Descending);
}
