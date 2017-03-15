using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories.FileDb.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DragonAsia.SalesMan.Repositories
{
	public class ItemRepository : IItemRepository
	{
		#region fields & constants

		private string root = Settings.Default.db_path + "/items";
		private string extension = ".json";

		#endregion

		public ItemRepository()
		{
			if (!Directory.Exists(root))
				Directory.CreateDirectory(root);
		}

		#region public methods

		public void Create(Item entity)
		{
			try
			{
				var newGuid = IoHelper.GetNewGuid(root, extension);
				if (newGuid == null)
					return;

				entity.Guid = newGuid;
				var file = root + "/" + newGuid + extension;
				File.WriteAllText(file, JsonConvert.SerializeObject(entity, Formatting.Indented));
			}
			catch { throw; }
		}

		public void Delete(string guid)
		{
			var file = root + "/" + guid + extension;

			try
			{
				File.Delete(file);
			}
			catch { throw; }
		}

		public IEnumerable<Item> RetrieveAll()
		{
			var entities = new List<Item>();

			foreach (var file in Directory.EnumerateFiles(root, "*" + extension))
				try
				{
					entities.Add(JsonConvert.DeserializeObject<Item>(File.ReadAllText(file)));
				}
				catch { throw; }

			return entities;
		}

		public Item Retrieve(string guid)
		{
			var file = root + "/" + guid + extension;
			return File.Exists(file) ? JsonConvert.DeserializeObject<Item>(File.ReadAllText(file)) : null;
		}

		public void Update(Item entity)
		{
			try
			{
				var file = root + "/" + entity.Guid + extension;
				File.WriteAllText(file, JsonConvert.SerializeObject(entity, Formatting.Indented));
			}
			catch { throw; }
		}

		#endregion
	}
}
