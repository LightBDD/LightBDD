using System;

namespace LightBDD.Reporting.Progressive.UI.Utils
{
    public class ObservableProperty<T> : IDisposable where T : IObservableStateChange
    {
        private readonly Action _onChange;
        private T _value;

        public ObservableProperty(Action onChange)
        {
            _onChange = onChange;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (ReferenceEquals(_value, value))
                    return;
                Detach();

                _value = value;

                if (_value != null)
                    _value.OnChange += _onChange;

                _onChange.Invoke();
            }
        }

        public void Dispose()
        {
            Detach();
        }

        private void Detach()
        {
            if (_value != null)
                _value.OnChange -= _onChange;
        }
    }
}
