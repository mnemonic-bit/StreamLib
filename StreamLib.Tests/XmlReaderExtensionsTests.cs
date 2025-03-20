using FluentAssertions;
using StreamLib;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace StreamLib.Tests
{
    public class XmlReaderExtensionsTests
    {

        [Fact]
        public void ToStream_ShouldCopyRootAndChildren_WhenXmlReaderContainsRootNode()
        {
            // Arrange
            string text = @"<A><B>hello</B><C>world</C></A>";
            XmlReader xmlReader = CreateXmlReader(text);

            // Act
            Stream result = xmlReader.ToStream(omitXmlDeclaration: true);

            // Assert
            result.ReadString().Should().Be(text);
        }

        private XmlReader CreateXmlReader(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(buffer);
            XmlReader xmlReader = XmlReader.Create(stream);
            return xmlReader;
        }

    }
}
