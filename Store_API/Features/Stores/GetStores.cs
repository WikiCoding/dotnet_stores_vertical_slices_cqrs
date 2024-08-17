using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store_API.Contracts;
using Store_API.Database;

namespace Store_API.Features.Stores
{
    public static class GetStores
    {
        public class Query : IRequest<List<StoreResponse>>
        {
        }

        internal sealed class Handler : IRequestHandler<Query, List<StoreResponse>>
        {
            private readonly StoresDbContext _context;

            public Handler(StoresDbContext context)
            {
                _context = context;
            }

            public async Task<List<StoreResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var stores = await _context.Stores.ToListAsync();

                return stores.ConvertAll<StoreResponse>(store => new(store.Id.ToString(), store.Name));
            }
        }
    }

    public class GetStoresEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/stores", async (ISender sender) =>
            {
                var query = new GetStores.Query();

                var stores = await sender.Send(query);

                return Results.Ok(stores);
            });
        }
    }
}
