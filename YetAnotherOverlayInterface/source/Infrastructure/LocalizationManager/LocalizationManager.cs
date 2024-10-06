using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHW_Yet_Another_Overlay_Interface;
internal sealed class LocalizationManager
{
    private static readonly Lazy<LocalizationManager> _lazy = new(() => new LocalizationManager());

    public static LocalizationManager Instance => _lazy.Value;

    private LocalizationManager()
    {

    }
}
