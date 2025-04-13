using SalesMan.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesMan.Repositories;

public class FsCustomerRepository : ICustomerRepository
{
	#region fields

	private readonly string root = $"{ConfigurationManager.AppSettings["dbFsRoot"]}/customers";
	private readonly string extension = "json";
	private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

	#endregion

	public FsCustomerRepository()
	{
		if (!Directory.Exists(root))
		{
			Directory.CreateDirectory(root);
		}
	}

	#region public methods

	public async Task<string> CreateAsync(Customer? entity)
	{
		if (entity == null)
		{
			return string.Empty;
		}

		var id = Guid.NewGuid().ToString().Replace("-", "");
		entity.Id = id;

		var file = $"{root}/{id}.{extension}";
		await using (FileStream fileStream = File.Open(file, FileMode.Create, FileAccess.Write))
		{
			await JsonSerializer.SerializeAsync(fileStream, entity, jsonSerializerOptions);
		}

		return entity.Id;
	}

	public async Task<bool> DeleteAsync(string id)
	{
		var file = $"{root}/{id}.{extension}";
		bool result = await Task.Run(() =>
		{
			File.Delete(file);
			return true;
		});

		return result;
	}

	public async Task<Customer?> RetrieveAsync(string id)
	{
		var file = $"{root}/{id}.{extension}";
		if (!File.Exists(file))
		{
			return null;
		}

		await using (FileStream fileStream = File.OpenRead(file))
		{
			var entity = await JsonSerializer.DeserializeAsync<Customer>(fileStream);
			return entity;
		}
	}

	public async Task<IEnumerable<Customer>> RetrieveAllAsync()
	{
		List<Customer> entities = [];

		foreach (var file in Directory.EnumerateFiles(root, "*." + extension))
		{
			await using (FileStream fileStream = File.OpenRead(file))
			{
				var entity = await JsonSerializer.DeserializeAsync<Customer>(fileStream);
				if (entity != null)
				{
					entities.Add(entity);
				}
			}
		}

		return entities;
	}

	public async Task<bool> UpdateAsync(string id, Customer? entity)
	{
		if (entity == null)
		{
			return false;
		}

		var file = $"{root}/{id}.{extension}";
		await using (FileStream fileStream = File.Open(file, FileMode.Create, FileAccess.Write))
		{
			await JsonSerializer.SerializeAsync(fileStream, entity, jsonSerializerOptions);
			return true;
		}
	}

	#endregion
}
