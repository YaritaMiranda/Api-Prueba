using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Contracts.Users;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Queries;

public sealed record GetUsersQuery(bool? IsActive);

public sealed class GetUsersQueryHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<IReadOnlyList<UserSummaryResponse>> HandleAsync(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var usersQuery = dbContext.Users.AsNoTracking();

        if (query.IsActive.HasValue)
        {
            usersQuery = usersQuery.Where(user => user.IsActive == query.IsActive.Value);
        }

        return await usersQuery
            .OrderBy(user => user.Id)
            .Select(user => new UserSummaryResponse(user.Id, user.Name, user.Email, user.IsActive))
            .ToArrayAsync(cancellationToken);
    }
}
