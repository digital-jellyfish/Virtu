using System.Text;

namespace Jellyfish.Library
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendWithoutGarbage(this StringBuilder builder, int number)
        {
            if (number < 0)
            {
                builder.Append('-');
            }

            int index = builder.Length;
            do
            {
                builder.Insert(index, _digits, (number % 10) + 9, 1);
                number /= 10;
            }
            while (number != 0);

            return builder;
        }

        private static readonly char[] _digits = new char[] { '9', '8', '7', '6', '5', '4', '3', '2', '1', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    }
}
