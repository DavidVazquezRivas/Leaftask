using Modules.Organizations.Application.Management.GetMyOrganizations;

namespace Modules.Organizations.Application.UnitTests.TestBuilders;

internal sealed class GetMyOrganizationsQueryTestBuilder
{
    private int _limit = 10;
    private string? _cursor;
    private IReadOnlyCollection<string> _sort = [];

    private GetMyOrganizationsQueryTestBuilder() { }

    public static GetMyOrganizationsQueryTestBuilder AQuery() => new();

    public GetMyOrganizationsQueryTestBuilder WithLimit(int limit)
    {
        _limit = limit;
        return this;
    }

    public GetMyOrganizationsQueryTestBuilder WithCursor(string? cursor)
    {
        _cursor = cursor;
        return this;
    }

    public GetMyOrganizationsQueryTestBuilder WithSort(IReadOnlyCollection<string> sort)
    {
        _sort = sort;
        return this;
    }

    public GetMyOrganizationsQuery Build() => new()
    {
        Limit = _limit,
        Cursor = _cursor,
        Sort = _sort
    };
}
