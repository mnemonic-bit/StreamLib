using FluentAssertions;
using System;
using Xunit;

namespace StreamLib.Tests
{
    public class EphemeralStreamTests
    {

        [Fact]
        public void Read_ShouldOnlyReadAsManyBytesAsAreStored_WhenCountIsBigger1()
        {
            // Arrange
            byte[] buffer = CreateBuffer(4);
            EphemeralStream ephemeralStream = new EphemeralStream(1, 2);
            ephemeralStream.Write(buffer, 0, 4);
            ephemeralStream.Position = 0;

            // Act
            byte[] resultBuffer = new byte[10];
            int result = ephemeralStream.Read(resultBuffer, 0, 10);

            // Assert
            result.Should().Be(buffer.Length);
            Span<byte> bufferSpan = resultBuffer;
            bufferSpan.Slice(0, 4).ToArray().Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void Read_ShouldOnlyReadAsManyBytesAsAreStored_WhenCountIsBigger()
        {
            // Arrange
            byte[] buffer = CreateBuffer(16);
            EphemeralStream ephemeralStream = new EphemeralStream(1, 256);
            ephemeralStream.Write(buffer, 0, 16);
            ephemeralStream.Position = 0;

            // Act
            byte[] resultBuffer1 = new byte[10];
            int result1 = ephemeralStream.Read(resultBuffer1, 0, 10);
            byte[] resultBuffer2 = new byte[10];
            int result2 = ephemeralStream.Read(resultBuffer2, 0, 10);

            // Assert
            result1.Should().Be(resultBuffer1.Length);
            Span<byte> bufferSpan1 = buffer;
            resultBuffer1.Should().BeEquivalentTo(bufferSpan1.Slice(0, 10).ToArray());

            result2.Should().Be(6);
            Span<byte> bufferSpan2 = buffer;
            Span<byte> resultBuffer2Span = resultBuffer2;
            resultBuffer2Span.Slice(0, 6).ToArray().Should().BeEquivalentTo(bufferSpan2.Slice(10, 6).ToArray());
        }

        [Fact]
        public void Read_ShouldReturnStoredBytes_WhenCompleteBufferIsUtilized()
        {
            // Arrange
            byte[] buffer = CreateBuffer(256);
            EphemeralStream ephemeralStream = new EphemeralStream(16, 16);
            ephemeralStream.Write(buffer, 0, 256);
            ephemeralStream.Position = 0;

            // Act
            byte[] resultBuffer = new byte[256];
            int result = ephemeralStream.Read(resultBuffer, 0, 256);

            // Assert
            result.Should().Be(buffer.Length);
            resultBuffer.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void ToArray_ShouldReturnWholeBufferContent()
        {
            // Arrange
            EphemeralStream ephemeralStream = new EphemeralStream(10, 2);
            ephemeralStream.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            ephemeralStream.Write(new byte[] { 6, 7, 8, 9, 10 }, 0, 5);

            // Act
            var result = ephemeralStream.ToArray();

            // Assert
            result.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }

        [Fact]
        public void Write_ShouldResizeTheChunk()
        {
            // Arrange
            byte[] buffer = CreateBuffer(128);
            EphemeralStream ephemeralStream = new EphemeralStream(1, 2);

            // Act
            ephemeralStream.Write(buffer, 0, buffer.Length);

            // Assert
            var result = ephemeralStream.ToArray();
            result.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void Write_ShouldNotThrowException_WhenStreamSizeIsFixed()
        {
            // For this to test correctly, we need to have a slightly
            // bigger amount of data to be written to the stream, because
            // ArrayBuffer sometimes returns slightly bigger array than
            // originally requested.

            // Arrange
            byte[] buffer = CreateBuffer(256);
            EphemeralStream ephemeralStream = new EphemeralStream(16, 16, true);

            // Act
            ephemeralStream.Write(CreateBuffer(256), 0, 256);

            // Assert
            var result = ephemeralStream.ToArray();
            result.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void Write_ShouldThrowException_WhenCountExceedsBufferLength()
        {
            // Arrange
            EphemeralStream ephemeralStream = new EphemeralStream(16, 16, true);

            // Act
            Action writeAction = () => ephemeralStream.Write(CreateBuffer(256), 0, 512);

            // Assert
            writeAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Write_ShouldThrowException_WhenStreamSizeIsFixed()
        {
            // For this to test correctly, we need to have a slightly
            // bigger amount of data to be written to the stream, because
            // ArrayBuffer sometimes returns slightly bigger array than
            // originally requested.

            // Arrange
            EphemeralStream ephemeralStream = new EphemeralStream(16, 16, true);

            // Act
            Action writeAction = () => ephemeralStream.Write(CreateBuffer(512), 0, 512);

            // Assert
            writeAction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Write_ShouldThrowException_WhenWritingWindowsIsNotWithinBounds()
        {
            // Arrange
            EphemeralStream ephemeralStream = new EphemeralStream(16, 16, true);

            // Act
            Action writeAction = () => ephemeralStream.Write(CreateBuffer(256), 1, 256);

            // Assert
            writeAction.Should().Throw<ArgumentException>();
        }


        private byte[] InitBuffer(params byte[] values)
        {
            return values;
        }

        private byte[] CreateBuffer(int capacity)
        {
            byte[] buffer = new byte[capacity];
            for (int i = 0; i < capacity; i++)
            {
                buffer[i] = (byte)(i % 256);
            }
            return buffer;
        }

    }
}
