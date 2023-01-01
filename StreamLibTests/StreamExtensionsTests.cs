using FluentAssertions;
using StreamLib;
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
            byte[] result = stringStream.ReadToEnd();

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
            byte[] result = stringStream.ReadToEnd();

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
            byte[] result = await stringStream.ReadToEndAsync();

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
            byte[] result = await stringStream.ReadToEndAsync();

            // Assert
            result.Should().BeEquivalentTo(textAsBytes);
        }

    }
}
