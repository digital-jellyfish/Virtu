using System;
using System.Threading;

namespace Jellyfish.Library
{
    public class Lazy<T> where T : class
    {
        public Lazy(Func<T> initializer)
        {
            _initializer = initializer;
        }

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    T value = _initializer();
                    if (Interlocked.CompareExchange(ref _value, value, null) != null)
                    {
                        IDisposable disposable = value as IDisposable; // dispose preempted instance
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }

                return _value;
            }
        }

        private Func<T> _initializer;
        private T _value;
    }
}
