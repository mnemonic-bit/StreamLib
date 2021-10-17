using StreamLib;
using System.Text;
using Xunit;

namespace StreamLibTests
{
    public class StringStreamTests
    {

        [Fact]
        public void Read_ShouldReturnCompleteBufferInOneGo_WhenRequestedLengthEqualsLengthinBytes()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[lengthInBytes];
            int readByteCount = stringStream.Read(buffer, 0, lengthInBytes);

            // Assert
            Assert.Equal(lengthInBytes, readByteCount);
            Assert.Equal(text, encoding.GetString(buffer));
        }

        [Fact]
        public void Read_ShouldReturnCompleteBufferInOneGo_WhenRequestedLengthIsLargerThanLengthInBytes()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[lengthInBytes];
            int readByteCount = stringStream.Read(buffer, 0, lengthInBytes + 10);

            // Assert
            Assert.Equal(lengthInBytes, readByteCount);
            Assert.Equal(text, encoding.GetString(buffer));
        }

        [Fact]
        public void Read_ShouldReturnOneCharacter_WhenOnlyOneByteIsRequested()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;
            int requestedBytes = 2;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, requestedBytes);

            // Assert
            Assert.Equal(requestedBytes, readByteCount);
            Assert.Equal(text.Substring(0, 1), encoding.GetString(buffer, 0, requestedBytes));
        }

        [Fact]
        public void Read_ShouldReturnNoCharacter_WhenCountIsZero()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 3;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, 0);

            // Assert
            Assert.Equal(0, readByteCount);
        }

        [Fact]
        public void Read_ShouldReturnAllBytes_WhenCountIsMuchBiggerThanByteRepresentation()
        {
            // Arrange
            string text = "SOME-TEST-TEXT";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 100;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, bufferSize);

            // Assert
            Assert.Equal(lengthInBytes, readByteCount);
            Assert.Equal(text, encoding.GetString(buffer, 0, readByteCount));
        }

        [Fact]
        public void Read_ShouldReturnNoCharacter_WhenNumberOfRequestedBytesIsNotBigEnoughToRepresentCharacter()
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(0x1F642); 
            string text = $"{smileyAsString}abc";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, 1);

            // Assert
            Assert.Equal(0, readByteCount);
        }

        [Theory]
        [InlineData(0x1F642, 2)] // smiley
        [InlineData(0x03a0, 2)] // PI
        public void Read_ShouldReturnOneCharacter_WhenNumberOfRequestedBytesIsBigEnoughToRepresentCharacter(int characterUnicode, int encodedCharacterLength)
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(characterUnicode);
            string text = $"{smileyAsString}abc";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount = stringStream.Read(buffer, 0, encodedCharacterLength);

            // Assert
            Assert.Equal(encodedCharacterLength, readByteCount);
        }

        [Theory]
        [InlineData(0x1F642, 2)] // smiley
        [InlineData(0x03a0, 2)] // PI
        public void Read_ShouldReturnOneCharacterEach_WhenNumberOfRequestedBytesIsBigEnoughToRepresentCharacterRespectively(int characterUnicode, int encodedCharacterLength)
        {
            // Arrange
            string smileyAsString = char.ConvertFromUtf32(characterUnicode);
            string text = $"{smileyAsString}{smileyAsString}";
            Encoding encoding = Encoding.Unicode;
            int lengthInBytes = encoding.GetByteCount(text);
            int maxLengthOfOneChar = encoding.GetMaxByteCount(1);
            int bufferSize = 10;

            StringStream stringStream = new StringStream(text, encoding);

            // Act
            byte[] buffer = new byte[bufferSize];
            int readByteCount1 = stringStream.Read(buffer, 0, encodedCharacterLength);
            int readByteCount2 = stringStream.Read(buffer, readByteCount1, encodedCharacterLength);

            // Assert
            Assert.Equal(encodedCharacterLength, readByteCount1);
            Assert.Equal(encodedCharacterLength, readByteCount2);
            Assert.Equal(text, encoding.GetString(buffer, 0, readByteCount1 + readByteCount1));
        }

    }
}
