using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHW_Yet_Another_Overlay_Interface;
internal class ConfigManager
{
	private static readonly Lazy<ConfigManager> _lazy = new(() => new ConfigManager());

	public static ConfigManager Instance => _lazy.Value;

	private ConfigManager()
	{

	}
}
