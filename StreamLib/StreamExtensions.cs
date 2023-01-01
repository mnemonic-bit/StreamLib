﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib
{
    public static class StreamExtensions
    {

        /// <summary>
        /// Reads the contents of a stream and creates a string from
        /// that contents.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadString(this Stream stream, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            using StreamReader reader = new StreamReader(
                stream,
                encoding: encoding,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            string value = reader.ReadToEnd();

            return value;
        }

        /// <summary>
        /// Reads asynchronously the contents of a stream and creates
        /// a string from that contents.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<string> ReadStringAsync(this Stream stream, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            using StreamReader reader = new StreamReader(
                stream,
                encoding: encoding,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            string value = await reader.ReadToEndAsync();

            return value;
        }

        /// <summary>
        /// Reads the whole contents of a stream and returns it
        /// as a byte array. This method never returns null. An
        /// exception is thrown if the stream throws an exception
        /// during reading. If not enough memory is available,
        /// an OutOfMemoryException is thrown by the runtime.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Returns the stream's contents as byte array.</returns>
        public static byte[] ReadToEnd(this Stream stream)
        {
            byte[] buffer = new byte[16*1024];

            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Reads asynchronous the whole contents of a stream and returns
        /// it as a byte array. This method never returns null. An exception
        /// is thrown if the stream throws an exception during reading.
        /// If not enough memory is available, an OutOfMemoryException
        /// is thrown by the runtime.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Returns the stream's contents as byte array.</returns>
        public static async Task<byte[]> ReadToEndAsync(this Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];

            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

    }
}
