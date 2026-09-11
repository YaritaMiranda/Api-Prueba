using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Addresses;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Addresses.Queries;

public sealed record GetUserAddressesQuery(int UserId);

public sealed class GetUserAddressesQueryHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<IReadOnlyList<AddressResponse>>> HandleAsync(
        GetUserAddressesQuery query,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == query.UserId, cancellationToken);

        if (!userExists)
        {
            return OperationResult<IReadOnlyList<AddressResponse>>.NotFound("Usuario no encontrado.");
        }

        var addresses = await dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == query.UserId)
            .OrderBy(address => address.Id)
            .Select(address => new AddressResponse(
                address.Id,
                address.UserId,
                address.Street,
                address.City,
                address.Country,
                address.ZipCode))
            .ToArrayAsync(cancellationToken);

        return OperationResult<IReadOnlyList<AddressResponse>>.Success(addresses);
    }
}
