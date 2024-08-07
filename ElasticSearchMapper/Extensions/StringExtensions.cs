using System.Runtime.CompilerServices;
using System.Text;
using ElasticSearchMapper.Pools;

namespace ElasticSearchMapper.Extensions;

internal static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var writer = BufferWriterPool.Rent();
        try
        {
            writer.GetSpan(1)[0] = (byte)char.ToLowerInvariant(str[0]);
            writer.Advance(1);

            for (var i = 1; i < str.Length; i++)
                if (char.IsUpper(str[i]))
                {
                    writer.GetSpan(2)[0] = (byte)'_';
                    writer.GetSpan(2)[1] = (byte)char.ToLowerInvariant(str[i]);
                    writer.Advance(2);
                }
                else
                {
                    writer.GetSpan(1)[0] = (byte)str[i];
                    writer.Advance(1);
                }

            return Encoding.UTF8.GetString(writer.WrittenSpan);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to convert string to snake case", e);
        }
        finally
        {
            BufferWriterPool.Return(writer);
        }
    }
}