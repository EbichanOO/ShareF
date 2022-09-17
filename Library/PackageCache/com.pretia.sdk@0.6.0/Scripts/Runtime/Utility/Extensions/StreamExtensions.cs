using System;
using System.Buffers;
using System.IO;

namespace PretiaArCloud.Networking
{
    internal static class StreamExtensions
    {
        internal static int ReadExactly(this Stream stream, byte[] buffer, int offset, int size)
        {
            try
            {
                int readBytes = stream.SafeRead(buffer, offset, size);
                if (readBytes == 0) return 0;

                while (readBytes < size)
                {
                    readBytes += stream.SafeRead(buffer, readBytes, size - readBytes);
                }

                return readBytes;
            }
            catch
            {
                return 0;
            }
        }

        private static int SafeRead(this Stream stream, byte[] buffer, int offset, int size)
        {
            try
            {
                return stream.Read(buffer, offset, size);
            }
            catch
            {
                return 0;
            }
        }

        internal static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}