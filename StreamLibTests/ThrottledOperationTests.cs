using System;
using System.Collections.Generic;
using System.Diagnostics;
using StreamLib.Implementation;
using Xunit;

namespace ThrottledStreamTests
{
    public class ThrottledOperationTests
    {

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 0, 0, 1000)]
        [InlineData(10, 0, 0, 1000)]
        public void Throttle_ShouldDoNothing_WhenCountIsZero(int bufferSize, int offset, int count, int bytesPerInterval)
        {
            // Arrange
            int baseTicksInterval = 1000;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            ThrottledOperation throttledOperation = new ThrottledOperation(
                baseTicksInterval,
                bytesPerInterval,
                (b, o, c) =>
                {
                    operationCountParameter.Add(c);
                    return c;
                },
                (a, b) => false);

            // Act
            int result = throttledOperation.Throttle(buffer, offset, count);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(operationCountParameter);
        }

        [Theory]
        [InlineData(1, 0, 1, 1000)]
        [InlineData(10, 0, 10, 1000)]
        public void Throttle_ShouldReturnImmediately_WhenOperationFulfillsAllAtOnce(int bufferSize, int offset, int count, int bytesPerInterval)
        {
            // Arrange
            int baseTicksInterval = 1000;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            ThrottledOperation throttledOperation = new ThrottledOperation(
                baseTicksInterval,
                bytesPerInterval,
                (b, o, c) =>
                {
                    operationCountParameter.Add(c);
                    return c;
                },
                (a, b) => a < b);

            // Act
            int result = throttledOperation.Throttle(buffer, offset, count);

            //Assert
            Assert.Equal(count, result);
            Assert.Equal(operationCountParameter, new List<int>() { count });
        }

        [Theory]
        [InlineData(10, 0, 10, 2)]
        [InlineData(10, 0, 9, 2)]
        [InlineData(10, 0, 9, 3)]
        [InlineData(10, 0, 9, 5)]
        public void Throttle_ShouldWorkUpTilTheEnd_WhenOperationProvidesUnthrottledData(int bufferSize, int offset, int count, int bytesPerInterval)
        {
            // Arrange
            int baseTicksInterval = 1000;
            int total = 0;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();
            int numberOfOperationCalls = 0;

            ThrottledOperation throttledOperation = new ThrottledOperation(
                baseTicksInterval,
                bytesPerInterval,
                (b, o, c) =>
                {
                    numberOfOperationCalls++;
                    int chunkSize = Math.Min(c, bufferSize - total);
                    total += chunkSize;
                    operationCountParameter.Add(chunkSize);
                    return chunkSize;
                },
                (a, b) => a < b);

            // Act
            int result = throttledOperation.Throttle(buffer, offset, count);

            //Assert
            Assert.Equal(count, result);
            Assert.Equal((int)Math.Ceiling((double)count / bytesPerInterval), numberOfOperationCalls);
        }

        [Theory]
        [InlineData(10, 0, 10, 2)]
        [InlineData(10, 0, 9, 2)]
        [InlineData(10, 0, 9, 3)]
        [InlineData(10, 0, 9, 5)]
        [InlineData(100, 0, 8, 20)]
        public void Throttle_ShouldThrottle_WhenOperationIsCalledTooFastFromClient(int bufferSize, int offset, int count, int bytesPerInterval)
        {
            // Arrange
            int baseTicksInterval = 1000;
            int total = 0;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();
            int numberOfOperationCalls = 0;

            ThrottledOperation throttledOperation = new ThrottledOperation(
                baseTicksInterval,
                bytesPerInterval,
                (b, o, c) =>
                {
                    numberOfOperationCalls++;
                    int chunkSize = Math.Min(c, bufferSize - total);
                    total += chunkSize;
                    operationCountParameter.Add(chunkSize);
                    return chunkSize;
                },
                (a, b) => a < b);

            // Act
            Stopwatch watch = Stopwatch.StartNew();

            int result = throttledOperation.Throttle(buffer, offset, 2);
            result += throttledOperation.Throttle(buffer, offset, 2);
            result += throttledOperation.Throttle(buffer, offset, 2);
            result += throttledOperation.Throttle(buffer, offset, count - result);

            watch.Stop();

            //Assert
            Assert.Equal(count, result);
            //Assert.Equal(4, numberOfOperationCalls);
            int minimumExpectedRuntime = (count / bytesPerInterval - ((count % bytesPerInterval == 0) ? 1 : 0)) * baseTicksInterval;
            Assert.True(watch.ElapsedMilliseconds >= minimumExpectedRuntime);
        }

    }
}
