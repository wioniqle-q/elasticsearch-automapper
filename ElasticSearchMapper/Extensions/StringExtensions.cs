using System.Runtime.CompilerServices;

namespace ElasticSearchMapper.Extensions;

internal static class StringExtensions
{
    private const int StackAllocThreshold = 1024;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var len = str.Length;
        var maxResultLength = len * 2;

        return maxResultLength <= StackAllocThreshold
            ? ToSnakeCaseInternal(str, stackalloc char[maxResultLength])
            : ToSnakeCaseInternal(str, new char[maxResultLength]);
    }

    private static unsafe string ToSnakeCaseInternal(string str, Span<char> buffer)
    {
        var resultIndex = 0;

        fixed (char* strPtr = str)
        {
            var src = strPtr;

            buffer[resultIndex++] = (char)(*src | 0x20);
            src++;

            for (var i = 1; i < str.Length; i++, src++)
            {
                var c = *src;
                if ((uint)(c - 'A') <= 'Z' - 'A')
                {
                    if ((uint)(buffer[resultIndex - 1] - 'a') <= 'z' - 'a')
                        buffer[resultIndex++] = '_';
                    buffer[resultIndex++] = (char)(c | 0x20);
                }
                else
                {
                    buffer[resultIndex++] = c;
                }
            }
        }

        return new string(buffer[..resultIndex]);
    }
}