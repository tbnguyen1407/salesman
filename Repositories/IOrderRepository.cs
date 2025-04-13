using SalesMan.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesMan.Repositories;

public interface IOrderRepository
{
	Task<IEnumerable<Order>> RetrieveAllAsync(DateTime fr, DateTime to);
	Task<Order?> RetrieveAsync(string id);
	Task<string> CreateAsync(Order? entity);
	Task<bool> UpdateAsync(string id, Order? entity);
	Task<bool> DeleteAsync(string id);
}
