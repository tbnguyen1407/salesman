using SalesMan.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesMan.Repositories;

public interface ICustomerRepository
{
	Task<IEnumerable<Customer>> RetrieveAllAsync();
	Task<Customer?> RetrieveAsync(string id);
	Task<string> CreateAsync(Customer? entity);
	Task<bool> UpdateAsync(string id, Customer? entity);
	Task<bool> DeleteAsync(string id);
}
