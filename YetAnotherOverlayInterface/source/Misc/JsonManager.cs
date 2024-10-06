using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;
internal static class JsonManager
{
	public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS_INSTANCE = new()
	{
		WriteIndented = true,
		AllowTrailingCommas = true,
		Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	public static string Serialize(object obj)
	{
		try
		{
			return JsonSerializer.Serialize(obj, JSON_SERIALIZER_OPTIONS_INSTANCE).Replace("  ", "\t");
		}
		catch (Exception e)
		{
			LogManager.Instance.Error($"JsonManager: Error while serializing {obj} - {e}");
			return null;
		}
	}

	public static void WriteToFile(string filePathName, string json)
	{
		try
		{
			//File.WriteAllText(filePathName, json);
			Directory.CreateDirectory(Path.GetDirectoryName(filePathName));
			var file = File.Open(filePathName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
			var streamWriter = new StreamWriter(file);
			streamWriter.AutoFlush = true;
			file.SetLength(0);
			streamWriter.WriteLine(json);

			streamWriter.Close();
		}
		catch(Exception e)
		{
			LogManager.Instance.Error($"JsonManager: Error while writing {filePathName} - {e}");
		}
	}

	private static async Task WriteToFileAsync(string filePathName, string json)
	{
		try
		{
			//File.WriteAllText(filePathName, json);

			Directory.CreateDirectory(Path.GetDirectoryName(filePathName));
			var file = File.Open(filePathName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
			var streamWriter = new StreamWriter(file);
			streamWriter.AutoFlush = true;
			file.SetLength(0);
			await streamWriter.WriteLineAsync(json);

			streamWriter.Close();
		}
		catch(Exception e)
		{
			LogManager.Instance.Error($"JsonManager: Error while writing {filePathName} - {e}");
		}
	}

	public static void SearializeToFile(string filePathName, object obj)
	{
		WriteToFile(filePathName, Serialize(obj));
	}

	private static async Task SearializeToFileAsync(string filePathName, object obj)
	{
		await WriteToFileAsync(filePathName, Serialize(obj));
	}

	public static string ReadFromFile(string filePathName)
	{
		//return File.ReadAllText(filePathName);
		try
		{
			var file = File.Open(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var streamReader = new StreamReader(file);
			var content = streamReader.ReadToEnd();

			streamReader.Close();
			return content;
		}
		catch(Exception e)
		{
			LogManager.Instance.Error($"JsonManager: Error while reading {filePathName} - {e}");
			return null;
		}


	}

	public static T ReadFromFile<T>(string filePathName) where T : class
	{
		//return File.ReadAllText(filePathName);

		var file = File.Open(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		var streamReader = new StreamReader(file);
		var content = streamReader.ReadToEnd();

		streamReader.Close();

		try
		{
			return JsonSerializer.Deserialize<T>(content, JSON_SERIALIZER_OPTIONS_INSTANCE);
		}
		catch (Exception e)
		{
			LogManager.Instance.Error($"JsonManager: Error while deserializing {filePathName} - {e}");
			return null;
		}
	}

	private static async Task<string> ReadFromFileAsync(string filePathName)
	{
		//return File.ReadAllText(filePathName);

		var file = File.Open(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		var streamReader = new StreamReader(file);
		var content = await streamReader.ReadToEndAsync();

		streamReader.Close();

		return content;
	}
}