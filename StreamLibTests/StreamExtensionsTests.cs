using FluentAssertions;
using StreamLib;
using System.Text;
using Xunit;

namespace StreamLibTests
{
    public class StreamExtensionsTests
    {

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

    }
}
