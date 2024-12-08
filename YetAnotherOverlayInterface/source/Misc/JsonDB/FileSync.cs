using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class FileSync
{
	public string PathFileName { get; } = string.Empty;

	public FileSync(string pathFileName)
	{
		PathFileName = pathFileName;
	}

	public string Read()
	{
		if(File.Exists(PathFileName)) return ReadFromFile();
			
		return Constants.EMPTY_JSON;
	}

	public bool Write(string json)
	{
		return WriteToFile(json);
	}

	public void Delete()
	{
		try
		{
			File.Delete(PathFileName);
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
		}
	}

	private string ReadFromFile()
	{
		try
		{
			var file = File.Open(PathFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var streamReader = new StreamReader(file);
			var content = streamReader.ReadToEnd();

			streamReader.Close();

			return content;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			return Constants.EMPTY_JSON;
		}

	}

	private bool WriteToFile(string json)
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(PathFileName));
			var file = File.Open(PathFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

			var streamWriter = new StreamWriter(file, Encoding.UTF8);
			streamWriter.AutoFlush = true;

			file.SetLength(0);

			streamWriter.Write(json);
			streamWriter.Close();

			return true;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception);
			return false;
		}
	}
}
