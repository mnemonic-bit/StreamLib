using FluentAssertions;
using System.Text;
using Xunit;

namespace StreamLib.Tests
{
    public class StringStreamTests
    {

        [Fact]
        public void Read_WhenRequestedLengthEqualsLengthinBytes_ShouldReturnCompleteBufferInOneGo()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[lengthInBytes];
            int readByteCount = stringStream.Read(buffer, 0, lengthInBytes);

            // Assert
            readByteCount.Should().Be(lengthInBytes);
            text.Should().Be(encoding.GetString(buffer));
        }

        [Fact]
        public void Read_WhenRequestedLengthIsLargerThanLengthInBytes_ShouldReturnCompleteBufferInOneGo()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[lengthInBytes];
            int readByteCount = stringStream.Read(buffer, 0, lengthInBytes + 10);

            // Assert
            readByteCount.Should().Be(lengthInBytes);
            stringStream.Position.Should().Be(readByteCount);
            text.Should().Be(encoding.GetString(buffer));
        }

        [Fact]
        public void Read_WhenOnlyTwoBytesAreRequested_ShouldReturnOneCharacter()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;
            int requestedBytes = 2;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, requestedBytes);

            // Assert
            readByteCount.Should().Be(requestedBytes);
            stringStream.Position.Should().Be(readByteCount);
            text.Substring(0, 1).Should().Be(encoding.GetString(buffer, 0, requestedBytes));
        }

        [Fact]
        public void Read_WhenOnlyTwoBytesAndThenAnotherTwoBytesAreRequested_ShouldReturnOneCharacter()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;
            int requestedBytes = 2;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, requestedBytes);
            readByteCount += stringStream.Read(buffer, 0, requestedBytes);

            // Assert
            readByteCount.Should().Be(2 * requestedBytes);
            stringStream.Position.Should().Be(readByteCount);
        }

        [Fact]
        public void Read_WhenCountIsZero_ShouldReturnNoCharacter()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, 0);

            // Assert
            readByteCount.Should().Be(0);
            stringStream.Position.Should().Be(0);
        }

        [Fact]
        public void Read_WhenCountIsOneAndEncodingIsUtf16_ShouldReturnNoCharacter()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, 1);

            // Assert
            readByteCount.Should().Be(0);
            stringStream.Position.Should().Be(0);
        }

        [Fact]
        public void Read_WhenCountIsMuchBiggerThanByteRepresentation_ShouldReturnAllBytes()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 100;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, bufferSize);

            // Assert
            readByteCount.Should().Be(lengthInBytes);
            stringStream.Position.Should().Be(readByteCount);
            text.Should().Be(encoding.GetString(buffer, 0, readByteCount));
        }

        [Fact]
        public void Read_WhenNumberOfRequestedBytesIsNotBigEnoughToRepresentCharacter_ShouldReturnNoCharacter()
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(0x1F642);
            string text = $"{smileyAsString}abc";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, 1);

            // Assert
            readByteCount.Should().Be(0);
            stringStream.Position.Should().Be(0);
        }

        [Theory]
        [InlineData(0x1F642, 4)] // smiley
        [InlineData(0x03a0, 2)] // PI
        public void Read_WhenNumberOfRequestedBytesIsBigEnoughToRepresentCharacter_ShouldReturnOneCharacter(int characterUnicode, int encodedCharacterLength)
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(characterUnicode);
            string text = $"{smileyAsString}abc";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, encodedCharacterLength);

            // Assert
            readByteCount.Should().Be(encodedCharacterLength);
            stringStream.Position.Should().Be(readByteCount);
            encoding.GetString(buffer, 0, readByteCount).Should().Be(smileyAsString);
        }

        [Theory]
        [InlineData(0x1F642, 4)] // smiley
        [InlineData(0x03a0, 2)] // PI
        public void Read_WhenNumberOfRequestedBytesIsBigEnoughToRepresentCharacterRespectively_ShouldReturnOneCharacterEach(int characterUnicode, int encodedCharacterLength)
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(characterUnicode);
            string text = $"{smileyAsString}{smileyAsString}";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            using StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount1 = stringStream.Read(buffer, 0, encodedCharacterLength);
            int readByteCount2 = stringStream.Read(buffer, readByteCount1, encodedCharacterLength);

            // Assert
            readByteCount1.Should().Be(encodedCharacterLength);
            readByteCount2.Should().Be(encodedCharacterLength);
            stringStream.Position.Should().Be(readByteCount1 + readByteCount1);
            text.Should().Be(encoding.GetString(buffer, 0, readByteCount1 + readByteCount1));
        }

    }
}
