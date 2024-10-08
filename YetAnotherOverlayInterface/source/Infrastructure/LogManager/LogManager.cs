using SharpPluginLoader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal static class LogManager
{

	public static void Info(object value)
	{
		Log.Info($"[{Constants.MOD_NAME_ABBREVIATION}] {value}");
	}

	public static void Warn(object value)
	{
		Log.Warn($"[{Constants.MOD_NAME_ABBREVIATION}] {value}");
	}

	public static void Error(object value)
	{
		Log.Error($"[{Constants.MOD_NAME_ABBREVIATION}] {value}");
	}

	public static void Debug(object value)
	{
		Log.Debug($"[{Constants.MOD_NAME_ABBREVIATION}] {value}");
	}
}
