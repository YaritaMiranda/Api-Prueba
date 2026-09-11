using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Addresses;
using UserCurrencyApi.Domain.Entities;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Addresses.Commands;

public sealed record CreateAddressCommand(int UserId, string Street, string City, string Country, string? ZipCode);

public sealed class CreateAddressCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<AddressResponse>> HandleAsync(
        CreateAddressCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var userExists = await dbContext.Users
            .AnyAsync(user => user.Id == command.UserId, cancellationToken);

        if (!userExists)
        {
            return OperationResult<AddressResponse>.NotFound("Usuario no encontrado.");
        }

        var address = new Address
        {
            UserId = command.UserId,
            Street = command.Street.Trim(),
            City = command.City.Trim(),
            Country = command.Country.Trim(),
            ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim()
        };

        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<AddressResponse>.Success(
            new AddressResponse(address.Id, address.UserId, address.Street, address.City, address.Country, address.ZipCode));
    }
}
