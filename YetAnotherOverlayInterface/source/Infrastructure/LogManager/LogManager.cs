using SharpPluginLoader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class LogManager
{
	private static readonly Lazy<LogManager> _lazy = new(() => new LogManager());

	public static LogManager Instance => _lazy.Value;

	private LogManager()
	{

	}

	public void Info(object value)
	{
		Log.Info($"[{Constants.MOD_NAME}] {value}");
	}

	public void Warn(object value)
	{
		Log.Warn($"[{Constants.MOD_NAME}] {value}");
	}

	public void Error(object value)
	{
		Log.Error($"[{Constants.MOD_NAME}] {value}");
	}

	public void Debug(object value)
	{
		Log.Debug($"[{Constants.MOD_NAME}] {value}");
	}
}
