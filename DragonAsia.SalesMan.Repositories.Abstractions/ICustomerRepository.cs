using DragonAsia.SalesMan.Models;
using System.Collections.Generic;

namespace DragonAsia.SalesMan.Repositories
{
	public interface ICustomerRepository
	{
		IEnumerable<Customer> RetrieveAll();
		Customer Retrieve(string guid);
		void Create(Customer entity);
		void Update(Customer entity);
		void Delete(string guid);
	}
}
