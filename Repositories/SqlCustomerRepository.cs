using System.Collections.Generic;
using System.Threading.Tasks;
using SalesMan.Models;

namespace SalesMan.Repositories;

public class SqlCustomerRepository : ICustomerRepository
{
    public Task<string> CreateAsync(Customer? entity)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<Customer?> RetrieveAsync(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> UpdateAsync(string id, Customer? entity)
    {
        throw new System.NotImplementedException();
    }
}
