using DragonAsia.SalesMan.Models;
using System;
using System.Collections.Generic;

namespace DragonAsia.SalesMan.Repositories
{
	public interface IOrderRepository
    {
		IEnumerable<Order> RetrieveAll(DateTime fr, DateTime to);
		Order Retrieve(string guid);
		void Create(Order entity);
		void Update(Order entity);
		void Delete(string guid);
	}
}
