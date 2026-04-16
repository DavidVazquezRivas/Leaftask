namespace BuildingBlocks.DrivingInfrastructure.Responses.Meta;

public static class SortMetaParser
{
    public static IReadOnlyList<SortMeta>? Parse(IReadOnlyCollection<string>? sortParameters)
    {
        if (sortParameters is null || sortParameters.Count == 0)
        {
            return null;
        }

        List<SortMeta> sortMetaList = [];

        foreach (string sort in sortParameters)
        {
            string[] parts = sort.Split(':', 2);
            if (parts.Length == 2 && Enum.TryParse(parts[1], true, out SortDirection direction))
            {
                sortMetaList.Add(new SortMeta
                {
                    Field = parts[0],
                    Direction = direction
                });
            }
        }

        return sortMetaList;
    }
}
