using System.Collections.Generic;
using System.Threading.Tasks;
using SalesMan.Models;

namespace SalesMan.Repositories;

public class SqlItemRepository : IItemRepository
{
    public Task<string> CreateAsync(Item? entity)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<Item>> RetrieveAllAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<Item?> RetrieveAsync(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> UpdateAsync(string id, Item? entity)
    {
        throw new System.NotImplementedException();
    }
}
