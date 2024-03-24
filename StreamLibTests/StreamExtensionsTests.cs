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

        [Fact]
        public void Compress_ShouldReturnTheSameStream_WhenNoCompressionIsGiven()
        {
            // Arrange
            string input = "HELLO-WORLD";
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(buffer);

            // Act
            var result = stream.Compress();

            // Assert
            result.Should().BeSameAs(stream);
        }

        [Fact]
        public void Compress_ShouldReturnTheSameStream_WhenNoCompressionIsGiven_RENAME1()
        {
            // Arrange
            string input = "HELLO-WORLD";
            StringStream sourceStream = new StringStream(input);
            MemoryStream destinationStream = new MemoryStream();

            // Act
            var compressionStream = destinationStream.Compress("gzip");
            sourceStream.CopyToAsync(compressionStream);
            compressionStream.Flush();

            destinationStream.Position = 0;
            var resultBuffer = destinationStream.ReadAll();

            // Assert
            resultBuffer.Should().NotBeNull();
            resultBuffer.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Decompress_ShouldReturnTheSameStream_WhenNoCompressionIsGiven()
        {
            // Arrange
            string input = "HELLO-WORLD";
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(buffer);

            // Act
            var result = stream.Decompress();

            // Assert
            result.Should().BeSameAs(stream);
        }

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
