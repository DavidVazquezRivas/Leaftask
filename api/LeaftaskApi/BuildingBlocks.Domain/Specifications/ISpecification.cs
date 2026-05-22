using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }
}
