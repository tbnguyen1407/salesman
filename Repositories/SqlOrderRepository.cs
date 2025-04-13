using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesMan.Models;

namespace SalesMan.Repositories;

public class SqlOrderRepository : IOrderRepository
{
    public Task<string> CreateAsync(Order? entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> RetrieveAllAsync(DateTime fr, DateTime to)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> RetrieveAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(string id, Order? entity)
    {
        throw new NotImplementedException();
    }
}
