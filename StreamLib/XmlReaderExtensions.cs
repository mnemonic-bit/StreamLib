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
        public static Stream ToStream(this XmlReader reader, bool omitXmlDeclaration = false)
        {
            var stream = new EphemeralStream();

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = omitXmlDeclaration,
                CloseOutput = false,
            };

            using XmlWriter writer = XmlWriter.Create(stream, settings);

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
            var stream = new EphemeralStream();

            using XmlWriter writer = XmlWriter.Create(stream);

            await writer.WriteNodeAsync(reader, true);
            await writer.FlushAsync();
            stream.Position = 0;

            return stream;
        }

    }
}
