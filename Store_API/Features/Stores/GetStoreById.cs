using Carter;
using MediatR;
using MongoDB.Bson;
using Store_API.Contracts;
using Store_API.Database;

namespace Store_API.Features.Stores
{
    public static class GetStoreById
    {
        public class Query : IRequest<StoreResponse>
        {
            public ObjectId Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, StoreResponse>
        {
            private readonly StoresDbContext _context;

            public Handler(StoresDbContext context)
            {
                _context = context;
            }

            public async Task<StoreResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var store = await _context.Stores.FindAsync(request.Id) ?? throw new InvalidDataException("Not found");
                var response = new StoreResponse(store.Id.ToString(), store.Name);
                
                return response;
            }
        }
    }

    public class GetStoreByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/stores/{id}", async (ObjectId id, ISender sender) =>
            {
                var query = new GetStoreById.Query { Id = id };
                try
                {
                    var result = await sender.Send(query);

                    return Results.Ok(result);
                }
                catch (Exception ex) 
                {
                    return Results.BadRequest(ex);
                }
            });
        }
    }
}
