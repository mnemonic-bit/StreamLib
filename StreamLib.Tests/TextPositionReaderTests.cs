using System.IO;
using Xunit;

namespace StreamLib.Tests
{
    public class TextPositionReaderTests
    {

        [Fact]
        public void Peek_ShouldNotConsumeAnyCharacter()
        {
            // Arrange
            string text = "1\n2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Peek();
            char ch2 = (char)textPositionReader.Peek();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('1', ch2);

            Assert.Equal(TextPositionReader.FIRST_LINE_NUMBER, textPositionReader.LineNumber);
            Assert.Equal(TextPositionReader.FIRST_COLUMN_NUMBER, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached1()
        {
            // Arrange
            string text = "1\n2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\n', ch2);
            Assert.Equal('2', ch3);

            Assert.Equal(2, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached2()
        {
            // Arrange
            string text = "1\r2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\r', ch2);
            Assert.Equal('2', ch3);

            Assert.Equal(2, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached3()
        {
            // Arrange
            string text = "1\n\n2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();
            char ch4 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\n', ch2);
            Assert.Equal('\n', ch3);
            Assert.Equal('2', ch4);

            Assert.Equal(3, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached4()
        {
            // Arrange
            string text = "1\n\r2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();
            char ch4 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\n', ch2);
            Assert.Equal('\r', ch3);
            Assert.Equal('2', ch4);

            Assert.Equal(3, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached5()
        {
            // Arrange
            string text = "1\r\n2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();
            char ch4 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\r', ch2);
            Assert.Equal('\n', ch3);
            Assert.Equal('2', ch4);

            Assert.Equal(2, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Read_ShouldAdjustPosition_WhenLineEndingIsReached6()
        {
            // Arrange
            string text = "1\r\r2";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act
            char ch1 = (char)textPositionReader.Read();
            char ch2 = (char)textPositionReader.Read();
            char ch3 = (char)textPositionReader.Read();
            char ch4 = (char)textPositionReader.Read();

            // Assert
            Assert.Equal('1', ch1);
            Assert.Equal('\r', ch2);
            Assert.Equal('\r', ch3);
            Assert.Equal('2', ch4);

            Assert.Equal(3, textPositionReader.LineNumber);
            Assert.Equal(2, textPositionReader.ColumnNumber);
        }

        [Fact]
        public void Test()
        {
            // Arrange
            string text = "";
            using StringReader reader = new StringReader(text);
            using TextPositionReader textPositionReader = new TextPositionReader(reader);

            // Act


            // Assert

        }

    }
}
