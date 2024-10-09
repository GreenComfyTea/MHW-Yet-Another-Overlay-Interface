using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class FileSync
{
	public string PathFileName { get; } = "";
	public Encoding FileEncoding { get; } = Encoding.UTF8;


	public FileSync(string pathFileName)
	{
		PathFileName = pathFileName;
	}

	public FileSync(string pathFileName, Encoding fileEncoding)
	{
		PathFileName = pathFileName;
		FileEncoding = fileEncoding;
	}

	public string Read()
	{
		if(File.Exists(PathFileName)) return ReadFromFile();
			
		WriteToFile(Constants.EMPTY_JSON);
		return Constants.EMPTY_JSON;
	}

	public bool Write(string json)
	{
		return WriteToFile(json);
	}

	private bool WriteToFile(string json)
	{
		try {
			Directory.CreateDirectory(Path.GetDirectoryName(PathFileName));
			var file = File.Open(PathFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
			
			var streamWriter = new StreamWriter(file, FileEncoding);
			streamWriter.AutoFlush = true;

			file.SetLength(0);

			streamWriter.Write(json);
			streamWriter.Close();

			return true;
		}
		catch(Exception exception)
		{
			LogManager.Error(exception.Message);
			return false;
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
			LogManager.Error(exception.Message);
			return Constants.EMPTY_JSON;
		}
		
	}

}
