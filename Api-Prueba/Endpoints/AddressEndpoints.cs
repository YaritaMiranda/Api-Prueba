using FluentValidation;
using UserCurrencyApi.Application.Addresses.Commands;
using UserCurrencyApi.Application.Addresses.Queries;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Addresses;

namespace UserCurrencyApi.Endpoints;

public static class AddressEndpoints
{
    public static RouteGroupBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var userAddressGroup = app.MapGroup("/users/{userId:int}/addresses")
            .WithTags("Addresses");

        userAddressGroup.MapPost("/", async (
            int userId,
            CreateAddressRequest request,
            IValidator<CreateAddressRequest> validator,
            CreateAddressCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new CreateAddressCommand(userId, request.Street, request.City, request.Country, request.ZipCode),
                cancellationToken);

            return result.ToHttpResult(address => Results.Created($"/addresses/{address.Id}", address));
        });

        userAddressGroup.MapGet("/", async (
            int userId,
            GetUserAddressesQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetUserAddressesQuery(userId), cancellationToken);
            return result.ToHttpResult(Results.Ok);
        });

        var addressGroup = app.MapGroup("/addresses")
            .WithTags("Addresses");

        addressGroup.MapPut("/{id:int}", async (
            int id,
            UpdateAddressRequest request,
            IValidator<UpdateAddressRequest> validator,
            UpdateAddressCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new UpdateAddressCommand(id, request.Street, request.City, request.Country, request.ZipCode),
                cancellationToken);

            return result.ToHttpResult(Results.Ok);
        });

        addressGroup.MapDelete("/{id:int}", async (
            int id,
            DeleteAddressCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new DeleteAddressCommand(id), cancellationToken);
            return result.ToHttpResult(_ => Results.NoContent());
        });

        return addressGroup;
    }
}
