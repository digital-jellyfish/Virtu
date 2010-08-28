using System;

namespace Jellyfish.Library
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class AssemblyCommentAttribute : Attribute
    {
        public AssemblyCommentAttribute(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; private set; }
    }
}
