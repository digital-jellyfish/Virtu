using System;

namespace Jellyfish.Library
{
    public sealed class Lazy<T> where T : class
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
                    lock (_lock)
                    {
                        if (_value == null)
                        {
                            _value = _initializer();
                        }
                    }
                }

                return _value;
            }
        }

        private Func<T> _initializer;
        private readonly object _lock = new object();
        private volatile T _value;
    }
}
