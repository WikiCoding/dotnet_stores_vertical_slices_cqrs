using MongoDB.Bson;

namespace Store_API.Contracts
{
    public record StoreResponse(string storeId, string storeName)
    {
    }
}
