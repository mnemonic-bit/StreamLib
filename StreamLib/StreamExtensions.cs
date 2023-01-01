using System.IO;

namespace StreamLib
{
    public static class StreamExtensions
    {

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

            int numberOfBytesRead = 0;
            while ((numberOfBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, numberOfBytesRead);
            }

            return memoryStream.ToArray();
        }

    }
}
