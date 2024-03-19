using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace StreamLib
{
    public static class XmlReaderExtensions
    {

        /// <summary>
        /// Provides a stream with the content of the XmlReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Stream ToStream(this XmlReader reader)
        {
            MemoryStream stream = new MemoryStream();
            
            using XmlWriter writer = XmlWriter.Create(stream);

            writer.WriteNode(reader, true);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Provides a stream with the content of the XmlReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static async Task<Stream> ToStreamAsync(this XmlReader reader)
        {
            MemoryStream stream = new MemoryStream();

            using XmlWriter writer = XmlWriter.Create(stream);

            await writer.WriteNodeAsync(reader, true);
            await writer.FlushAsync();
            stream.Position = 0;

            return stream;
        }

    }
}
