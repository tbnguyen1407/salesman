using DragonAsia.SalesMan.Models;
using System.Collections.Generic;

namespace DragonAsia.SalesMan.Repositories
{
	public interface IItemRepository
	{
		IEnumerable<Item> RetrieveAll();
		Item Retrieve(string guid);
		void Create(Item entity);
		void Update(Item entity);
		void Delete(string guid);
	}
}
