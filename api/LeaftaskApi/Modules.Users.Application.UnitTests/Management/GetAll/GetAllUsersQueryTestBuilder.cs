using Modules.Users.Application.Management.GetAll;

namespace Modules.Users.Application.UnitTests.Management.GetAll;

internal sealed class GetAllUsersQueryTestBuilder
{
    private string? _cursor;
    private int _limit = 10;
    private string? _search;
    private List<string> _sort = [];

    private GetAllUsersQueryTestBuilder() { }

    public static GetAllUsersQueryTestBuilder AQuery() => new();

    public GetAllUsersQueryTestBuilder WithLimit(int limit)
    {
        _limit = limit;
        return this;
    }

    public GetAllUsersQueryTestBuilder WithSearch(string? search)
    {
        _search = search;
        return this;
    }

    public GetAllUsersQueryTestBuilder WithCursor(string? cursor)
    {
        _cursor = cursor;
        return this;
    }

    public GetAllUsersQueryTestBuilder WithSort(List<string> sort)
    {
        _sort = sort;
        return this;
    }

    public GetAllUsersQuery Build() => new()
    {
        Limit = _limit,
        Cursor = _cursor,
        Sort = _sort,
        Search = _search
    };
}
