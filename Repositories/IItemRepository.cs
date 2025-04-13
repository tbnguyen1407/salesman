using SalesMan.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesMan.Repositories;

public interface IItemRepository
{
	Task<IEnumerable<Item>> RetrieveAllAsync();
	Task<Item?> RetrieveAsync(string id);
	Task<string> CreateAsync(Item? entity);
	Task<bool> UpdateAsync(string id, Item? entity);
	Task<bool> DeleteAsync(string id);
}
