using FluentValidation;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Application.Users.Commands;
using UserCurrencyApi.Application.Users.Queries;
using UserCurrencyApi.Contracts.Users;

namespace UserCurrencyApi.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapPost("/", async (
            CreateUserRequest request,
            IValidator<CreateUserRequest> validator,
            CreateUserCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new CreateUserCommand(request.Name, request.Email),
                cancellationToken);

            return result.ToHttpResult(user => Results.Created($"/users/{user.Id}", user));
        });

        group.MapGet("/", async (
            bool? isActive,
            GetUsersQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var users = await handler.HandleAsync(new GetUsersQuery(isActive), cancellationToken);
            return Results.Ok(users);
        });

        group.MapGet("/{id:int}", async (
            int id,
            GetUserByIdQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var user = await handler.HandleAsync(new GetUserByIdQuery(id), cancellationToken);
            return user is null
                ? Results.NotFound(new Contracts.Common.ErrorResponse("Usuario no encontrado."))
                : Results.Ok(user);
        });

        group.MapPut("/{id:int}", async (
            int id,
            UpdateUserRequest request,
            IValidator<UpdateUserRequest> validator,
            UpdateUserCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new UpdateUserCommand(id, request.Name, request.Email, request.IsActive),
                cancellationToken);

            return result.ToHttpResult(Results.Ok);
        });

        group.MapDelete("/{id:int}", async (
            int id,
            DeleteUserCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new DeleteUserCommand(id), cancellationToken);
            return result.ToHttpResult(_ => Results.NoContent());
        });

        group.MapPost("/bulk", async (
            BulkCreateUsersRequest request,
            IValidator<BulkCreateUsersRequest> validator,
            BulkCreateUsersCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var response = await handler.HandleAsync(
                new BulkCreateUsersCommand(request.Users),
                cancellationToken);

            return Results.Ok(response);
        });

        return group;
    }
}
