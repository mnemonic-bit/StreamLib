using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace StreamLib.Tests
{
    public partial class MeteringStreamTests
    {

        [Theory]
        [InlineData(100, 10)]
        [InlineData(100, 20)]
        [InlineData(100, 50)]
        [InlineData(100, 100)]
        [InlineData(1000, 20)]
        [InlineData(1000, 100)]
        public void Read_WhenCalledOnThrottledStream_ShouldReturnNumberOfBytesRead(int bufferSize, int bytesPerSecond)
        {
            var meteringStream = BuildStream(bufferSize, bytesPerSecond);

            var measuredTimings = new List<MeteringEventArgs>();
            meteringStream.AddReadEventListener(mea => { measuredTimings.Add(mea); });

            byte[] resultBuffer = new byte[bufferSize];
            int result = meteringStream.Read(resultBuffer, 0, bufferSize);

            result.Should().Be(bufferSize);
        }

    }
}
