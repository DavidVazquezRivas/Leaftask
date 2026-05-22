using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

public abstract class BaseSpecification<T>(Expression<Func<T, bool>> criteria) : ISpecification<T>
{
    private readonly List<Expression<Func<T, object>>> _includes = [];
    public Expression<Func<T, bool>> Criteria { get; } = criteria;
    public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
        => _includes.Add(includeExpression);
}
