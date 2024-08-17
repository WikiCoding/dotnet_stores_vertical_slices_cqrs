using Carter;
using FluentValidation;
using MediatR;
using Store_API.Contracts;
using Store_API.Database;
using Store_API.Models;

namespace Store_API.Features.Stores
{
    public static class CreateStore
    {
        public class Command : IRequest<StoreResponse>
        {
            public string StoreName { get; set; } = string.Empty;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.StoreName).NotEmpty();
                RuleFor(x => x.StoreName).MinimumLength(2);
            }
        }

        internal sealed class Handler : IRequestHandler<Command, StoreResponse>
        {
            private readonly StoresDbContext _dbContext;
            private readonly IValidator<Command> _validator;

            public Handler(StoresDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<StoreResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_validator.Validate(request).IsValid) throw new ArgumentException("Bad store name input");

                var store = new Store() { Name = request.StoreName };

                _dbContext.Add(store);

                await _dbContext.SaveChangesAsync(cancellationToken);

                // we might want to add here a CreatedStoreEvent Event that will send async message through Kafka or RabbitMQ so the Queries side consumes it.

                var res = new StoreResponse(storeId: store.Id.ToString(), storeName: store.Name);

                return res;
            }
        }
    }

    public class CreateStoreEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/stores", async (StoreRequest request, ISender sender) =>
            {
                var command = new CreateStore.Command
                {
                    StoreName = request.storeName,
                };

                try
                {
                    var res = await sender.Send(command);

                    return Results.Created("api/v1/stores", res);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest();
                }
            });
        }
    }
}
