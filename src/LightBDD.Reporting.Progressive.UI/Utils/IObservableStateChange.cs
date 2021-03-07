using System;

namespace LightBDD.Reporting.Progressive.UI.Utils
{
    public interface IObservableStateChange
    {
        event Action OnChange;
    }
}