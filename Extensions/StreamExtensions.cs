using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hermodr.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// Reads <paramref name="count"/> bytes from the stream into <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">The receiving buffer</param>
    /// <param name="offset">The offset into the buffer, in bytes</param>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>
    /// A Task that, when completed, reports the total number of bytes read, which is either
    /// <paramref name="count"/> or less if the end of this stream is reached.
    /// </returns>
    public static async Task<int> TryReadExactAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        var total = 0;
        while (total < count)
        {
            var result = await stream.ReadAsync(buffer, offset + total, count - total);
            if (result <= 0) break;
            total += result;
        }

        return total;
    }

    /// <summary>
    /// Reads <paramref name="count"/> bytes from the stream into <paramref name="buffer"/>,
    /// throwing an exception if not all bytes can be read.
    /// </summary>
    /// <param name="buffer">The receiving buffer</param>
    /// <param name="offset">The offset into the buffer, in bytes</param>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>
    /// A Task that, when completed, signifies all bytes are read into the buffer.
    /// </returns>
    public static async Task ReadExactAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        var result = await stream.TryReadExactAsync(buffer, offset, count);
        if (result != count)
            throw new EndOfStreamException($"Expected {count} bytes but only {result} were available.");
    }
}
