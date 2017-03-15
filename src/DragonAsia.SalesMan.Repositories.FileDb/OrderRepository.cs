using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories.FileDb.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DragonAsia.SalesMan.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		#region fields & constants

		private string root = Settings.Default.db_path + "/orders";
		private string extension = ".json";

		#endregion

		public OrderRepository()
		{
			if (!Directory.Exists(root))
				Directory.CreateDirectory(root);
		}

		public void Create(Order entity)
		{
			try
			{
				var newGuid = entity.Timestamp.ToString("yyyyMMddHHmmss");

				entity.Guid = newGuid;

				var dir = root + "/" + entity.Timestamp.ToString("yyyyMMdd");
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				var file = dir + "/" + newGuid + extension;
				File.WriteAllText(file, JsonConvert.SerializeObject(entity, Formatting.Indented));
			}
			catch { throw; }
		}

		public void Delete(string guid)
		{
			var file = root + "/" + guid.Substring(8) + "/" + guid + extension;

			try
			{
				File.Delete(file);
			}
			catch { throw; }
		}

		public Order Retrieve(string guid)
		{
			var file = root + "/" + guid.Substring(8) + "/" + guid + extension;
			return File.Exists(file) ? JsonConvert.DeserializeObject<Order>(File.ReadAllText(file)) : null;
		}

		public IEnumerable<Order> RetrieveAll(DateTime fr, DateTime to)
		{
			var entities = new List<Order>();

			string dateFrStr = fr.ToString("yyyyMMdd");
			string dateToStr = fr.ToString("yyyyMMdd");
			string timeFrPattern = fr.ToString("yyyyMMddHHmmss");
			string timeToPattern = to.ToString("yyyyMMddHHmmss");

			for (DateTime date = fr.Date; date <= to.Date; date = date.AddDays(1))
			{
				var dateDir = root + "/" + date.ToString("yyyyMMdd");

				if (!Directory.Exists(dateDir))
					continue;

				foreach (var file in Directory.EnumerateFiles(dateDir, "*" + extension))
				{
					try
					{
						var timePattern = Path.GetFileNameWithoutExtension(file);
						if (timePattern.CompareTo(timeFrPattern) >= 0 && timePattern.CompareTo(timeToPattern) <= 0)
						{
							var entity = JsonConvert.DeserializeObject<Order>(File.ReadAllText(file));
							entities.Add(entity);
						}
					}
					catch { throw; }
				}
			}

			return entities;
		}

		public void Update(Order entity)
		{
			throw new NotImplementedException();
		}
	}
}
