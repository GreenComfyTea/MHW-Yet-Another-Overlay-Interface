using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;
internal sealed class LocalizationManager
{
    private static readonly Lazy<LocalizationManager> _lazy = new(() => new LocalizationManager());

    public static LocalizationManager Instance => _lazy.Value;

    private LocalizationManager()
    {

    }
}
