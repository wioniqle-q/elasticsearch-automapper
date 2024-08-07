using System.Buffers;
using Microsoft.Extensions.ObjectPool;

namespace ElasticSearchMapper.Pools;

public static class BufferWriterPool
{
    private static readonly ObjectPool<ArrayBufferWriter<byte>> Pool;

    static BufferWriterPool()
    {
        var provider = new DefaultObjectPoolProvider { MaximumRetained = 1000 };
        Pool = provider.Create(new BufferWriterPoolPolicy());
    }

    public static ArrayBufferWriter<byte> Rent()
    {
        return Pool.Get();
    }

    public static void Return(ArrayBufferWriter<byte> writer)
    {
        writer.Clear();
        Pool.Return(writer);
    }

    private class BufferWriterPoolPolicy : IPooledObjectPolicy<ArrayBufferWriter<byte>>
    {
        public ArrayBufferWriter<byte> Create() => new(1024);

        public bool Return(ArrayBufferWriter<byte> obj)
        {
            obj.Clear();
            return true;
        }
    }
}