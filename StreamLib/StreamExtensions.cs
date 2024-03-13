using System.IO;
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
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding to use while reading from the stream. If null, the default Encoding.UTF8 is used.</param>
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
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding to use while reading from the stream. If null, the default Encoding.UTF8 is used.</param>
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
        public static byte[] ReadAll(this Stream stream)
        {
            byte[] buffer = new byte[DefaultBufferSize];

            using var memoryStream = new MemoryStream();

            int numberOfBytesRead = 0;
            while ((numberOfBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, numberOfBytesRead);
            }

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
        public static async Task<byte[]> ReadAllAsync(this Stream stream)
        {
            byte[] buffer = new byte[DefaultBufferSize];

            using var memoryStream = new MemoryStream();

            int numberOfBytesRead = 0;
            while ((numberOfBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, numberOfBytesRead);
            }

            return memoryStream.ToArray();
        }


        public static void WriteAllTo(this Stream stream, Stream outputStream)
        {
            byte[] buffer = new byte[DefaultBufferSize];

            int numberOfBytesRead;
            while ((numberOfBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, numberOfBytesRead);
            }

            stream.CopyTo(outputStream);
        }


        /// <summary>
        /// The size of the buffer used when copying anything between streams.
        /// </summary>
        private const int DefaultBufferSize = 1024;


    }
}
