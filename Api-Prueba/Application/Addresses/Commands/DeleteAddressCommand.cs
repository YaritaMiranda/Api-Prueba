using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Addresses.Commands;

public sealed record DeleteAddressCommand(int Id);

public sealed class DeleteAddressCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<bool>> HandleAsync(
        DeleteAddressCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var address = await dbContext.Addresses.FindAsync([command.Id], cancellationToken);

        if (address is null)
        {
            return OperationResult<bool>.NotFound("Dirección no encontrada.");
        }

        dbContext.Addresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<bool>.Success(true);
    }
}
