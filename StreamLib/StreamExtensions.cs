using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib
{
    public static class StreamExtensions
    {

        /// <summary>
        /// Creates a stream which applies the compression methods as given in
        /// the <code>contentEncoding</code>. which is a comma-separated list
        /// of encodings to be applied. All compressions are applied in
        /// the order given in that comma-separated string.
        /// </summary>
        /// <param name="stream">The destination stream which will be filled with the compressed contents</param>
        /// <param name="compressionNames">The list of encodings. Valid values are br, chunked, deflate, exi, gzip, and identity.</param>
        /// <returns>Returns a stream that represents the compressed encodings.</returns>
        public static Stream Compress(this Stream stream, params string[] compressionNames)
        {
            // Apply each compression algorithm to the stream, and
            // create a nested stream of compressions.
            Stream compressionStream = (compressionNames ?? new string[0])
                .Select(contentEncoding => contentEncoding.Trim())
                .Reverse()
                .Where(x => !string.IsNullOrEmpty(x))
                .Aggregate(stream, (stream, compressionName) => CreateSingleCompressionStream(stream, compressionName, CompressionMode.Compress));

            return compressionStream;
        }

        /// <summary>
        /// Creates a stream which applies the de-compression methods as given in
        /// the <code>contentEncoding</code>. which is a comma-separated list
        /// of encodings to be applied. All compressions are applied in
        /// the order given in that comma-separated string.
        /// </summary>
        /// <param name="stream">The destination stream which will be filled with the compressed contents</param>
        /// <param name="compressionNames">The list of encodings. Valid values are br, chunked, deflate, exi, gzip, and identity.</param>
        /// <returns>Returns a stream that represents the compressed encodings.</returns>
        public static Stream Decompress(this Stream stream, params string[] compressionNames)
        {
            // Apply each compression algorithm to the stream, and
            // create a nested stream of compressions.
            Stream compressionStream = (compressionNames ?? new string[] { string.Empty })
                .Select(contentEncoding => contentEncoding.Trim())
                .Reverse()
                .Where(x => !string.IsNullOrEmpty(x))
                .Aggregate(stream, (stream, compressionName) => CreateSingleCompressionStream(stream, compressionName, CompressionMode.Decompress));

            return compressionStream;
        }

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

            using var memoryStream = new EphemeralStream();

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

            using var memoryStream = new EphemeralStream();

            int numberOfBytesRead = 0;
            while ((numberOfBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, numberOfBytesRead);
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Writes the whole contents of one stream to another.
        /// </summary>
        /// <param name="stream">The source stream, where the data is copied from.</param>
        /// <param name="outputStream">The destination stream where the data is copied to.</param>
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
        private const int DefaultBufferSize = 4096;


        private static Stream CreateSingleCompressionStream(Stream destStream, string compressionName, CompressionMode mode)
        {
            // for content types and encodings, please refer to
            // https://learn.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-7.0#response-compression

            switch (compressionName)
            {
                case "br":
                    // Brotli compression
                    destStream = new BrotliStream(destStream, mode, true);
                    break;
                case "chunked":
                    //TODO: implement a chunked-stream to wrap the contents stream into.
                    break;
                case "compress":
                    // LZW compression - not implemented, because is has not been in
                    // use until 2003 because of patent law reasons.
                    break;
                case "deflate":
                    // zlib structure with deflate compression
                    destStream = new DeflateStream(destStream, mode, true);
                    break;
                case "exi":
                    //TODO: is this a real-life encoding? Its listed in the URL above
                    break;
                case "gzip":
                    // LZ77 compression
                    destStream = new GZipStream(destStream, mode, true);
                    break;
                case "identity":
                    // just return destStream as it is, since identity means
                    // 'leave the content as it is'
                    break;
                default:
                    throw new Exception($"Compression '{ compressionName }' not supported by this library");
            }

            return destStream;
        }


    }
}
