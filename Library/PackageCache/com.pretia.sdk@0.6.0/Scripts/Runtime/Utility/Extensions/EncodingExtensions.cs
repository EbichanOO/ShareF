using System;
using System.Text;

namespace PretiaArCloud.Networking
{
    internal static class EncodingExtensions
    {
        internal unsafe static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty) return "";
            fixed (byte* ptr = bytes)
            {
                return encoding.GetString(ptr, bytes.Length);
            }
        }
    }
}
