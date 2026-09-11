using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Addresses;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Addresses.Commands;

public sealed record UpdateAddressCommand(int Id, string Street, string City, string Country, string? ZipCode);

public sealed class UpdateAddressCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<AddressResponse>> HandleAsync(
        UpdateAddressCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var address = await dbContext.Addresses.FindAsync([command.Id], cancellationToken);

        if (address is null)
        {
            return OperationResult<AddressResponse>.NotFound("Dirección no encontrada.");
        }

        address.Street = command.Street.Trim();
        address.City = command.City.Trim();
        address.Country = command.Country.Trim();
        address.ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<AddressResponse>.Success(
            new AddressResponse(address.Id, address.UserId, address.Street, address.City, address.Country, address.ZipCode));
    }
}
