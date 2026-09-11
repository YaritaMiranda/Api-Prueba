using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Contracts.Addresses;
using UserCurrencyApi.Contracts.Users;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Queries;

public sealed record GetUserByIdQuery(int Id);

public sealed class GetUserByIdQueryHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<UserDetailResponse?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == query.Id)
            .Select(user => new UserDetailResponse(
                user.Id,
                user.Name,
                user.Email,
                user.IsActive,
                user.Addresses
                    .OrderBy(address => address.Id)
                    .Select(address => new AddressResponse(
                        address.Id,
                        address.UserId,
                        address.Street,
                        address.City,
                        address.Country,
                        address.ZipCode))
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
