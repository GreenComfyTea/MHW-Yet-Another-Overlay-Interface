using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class Config
{
	public string Name { get; set; } = Constants.DEFAULT_CONFIG;

	public string Localization { get; set; } = Constants.DEFAULT_LOCALIZATION;
}
