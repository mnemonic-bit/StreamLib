using FluentAssertions;
using StreamLib;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StreamLibTests
{
    public class StreamExtensionsTests
    {

        [Theory]
        [InlineData("SOME-TEST-TEXT")]
        [InlineData("SOME-TEST-TEXT SOME-TEST-TEXT SOME-TEST-TEXT")]
        public void ReadString_ShouldReturnSameBytes_WhenSourceStreamContainsBytes(string text)
        {
            // Arrange
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            string result = stringStream.ReadString(encoding);

            // Assert
            result.Should().BeEquivalentTo(text);
        }

        [Fact]
        public void ReadToEnd_ShouldReturnSameBytes_WhenSourceStreamContainsNoBytes_()
        {
            // Arrange
            string text = "";
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] result = stringStream.ReadAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(textAsBytes);
        }

        [Theory]
        [InlineData("SOME-TEST-TEXT")]
        [InlineData("SOME-TEST-TEXT SOME-TEST-TEXT SOME-TEST-TEXT")]
        public void ReadToEnd_ShouldReturnSameBytes_WhenSourceStreamContainsBytes(string text)
        {
            // Arrange
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] result = stringStream.ReadAll();

            // Assert
            result.Should().BeEquivalentTo(textAsBytes);
        }

        [Fact]
        public async Task ReadToEndAsync_ShouldReturnSameBytes_WhenSourceStreamContainsNoBytes_()
        {
            // Arrange
            string text = "";
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] result = await stringStream.ReadAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(textAsBytes);
        }

        [Theory]
        [InlineData("SOME-TEST-TEXT")]
        [InlineData("SOME-TEST-TEXT SOME-TEST-TEXT SOME-TEST-TEXT")]
        public async Task ReadToEndAsync_ShouldReturnSameBytes_WhenSourceStreamContainsBytes(string text)
        {
            // Arrange
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] result = await stringStream.ReadAllAsync();

            // Assert
            result.Should().BeEquivalentTo(textAsBytes);
        }

        [Theory]
        [InlineData("SOME-TEST-TEXT")]
        [InlineData("SOME-TEST-TEXT SOME-TEST-TEXT SOME-TEST-TEXT")]
        public void ReadToEnd_ShouldReturnSameBytes_When_(string text)
        {
            // Arrange
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            // compression: gzip -> deflate
            using MemoryStream destStream = new MemoryStream();
            using DeflateStream deflateStream = new DeflateStream(destStream, CompressionMode.Compress, true);
            using GZipStream gzipStream = new GZipStream(deflateStream, CompressionMode.Compress, true);

            using StringStream sourceStream = new StringStream(text, encoding);

            // Act
            sourceStream.CopyTo(gzipStream);
            gzipStream.Flush();

            // Assert
            byte[] result = destStream.ToArray();
            result.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("SOME-TEST-TEXT")]
        [InlineData("SOME-TEST-TEXT SOME-TEST-TEXT SOME-TEST-TEXT")]
        public void ReadToEnd_ShouldReturnSameBytes_When__(string text)
        {
            // Arrange
            Encoding encoding = Encoding.Unicode;
            byte[] textAsBytes = encoding.GetBytes(text);

            // Compress: gzip -> deflate
            using MemoryStream destStream = new MemoryStream();
            using DeflateStream deflateStream = new DeflateStream(destStream, CompressionMode.Compress, true);
            using GZipStream gzipStream = new GZipStream(deflateStream, CompressionMode.Compress, true);
            using BrotliStream brotliStream = new BrotliStream(gzipStream, CompressionMode.Compress, true);

            using StringStream sourceStream = new StringStream(text, encoding);
            sourceStream.CopyTo(brotliStream);
            brotliStream.Flush();

            byte[] compressedData = destStream.ToArray();

            // Decompress: deflate -> gzip
            using MemoryStream compressedStream = new MemoryStream(compressedData);
            using DeflateStream deflateDecompressStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true);
            using GZipStream gzipDecompressStream = new GZipStream(deflateDecompressStream, CompressionMode.Decompress, true);
            using BrotliStream brotliDecompressStream = new BrotliStream(gzipDecompressStream, CompressionMode.Decompress, true);

            using MemoryStream resultDecompressStream = new MemoryStream();
            brotliDecompressStream.CopyTo(resultDecompressStream);
            resultDecompressStream.Flush();

            // Act
            byte[] result = resultDecompressStream.ToArray();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(textAsBytes);
        }

    }
}
