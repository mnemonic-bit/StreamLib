using StreamLib;
using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace ThrottledStreamTests
{
    public class ThrottledStreamTests
    {

        [Fact]
        public void Test()
        {
            // Arrange
            int bytesPerSecond = 3;
            int bufferSize = 20;
            byte[] buffer = new byte[bufferSize];

            Random rnd = new Random(0xc0de);
            rnd.NextBytes(buffer);

            MemoryStream memoryStream = new MemoryStream(buffer);

            ThrottledStream throttledStream = new ThrottledStream(memoryStream, bytesPerSecond, true, false);

            // Act
            Stopwatch watch = Stopwatch.StartNew();

            byte[] resultBuffer = new byte[bufferSize];
            int result = throttledStream.Read(resultBuffer, 0, bufferSize);

            watch.Stop();

            // Assert
            Assert.Equal(bufferSize, result);
            Assert.Equal(buffer, resultBuffer);

            int minimumExpectedRuntime = (bufferSize / bytesPerSecond - ((bufferSize % bytesPerSecond == 0) ? 1 : 0)) * 1000;
            Assert.True(watch.ElapsedMilliseconds >= minimumExpectedRuntime);
        }

    }
}
