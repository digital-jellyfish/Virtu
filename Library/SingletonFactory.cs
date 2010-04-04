using System.Diagnostics.CodeAnalysis;

namespace Jellyfish.Library
{
    public static class SingletonFactory<T> where T : class, new()
    {
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static T Create()
        {
            return _instance;
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static T Instance { get { return _instance; } }

        private static readonly T _instance = new T();
    }
}
