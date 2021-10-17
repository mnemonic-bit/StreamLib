using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using StreamLib.Implementation;
using Xunit;

namespace StreamLibTests
{
    public class MeteringOperationTests
    {

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(10, 0, 0)]
        public void MeterOperation_ShouldDoNothing_WhenCountIsZero(int bufferSize, int offset, int count)
        {
            // Arrange
            int baseTicksInterval = 1000;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            MeteringOperation meteringOperation = new MeteringOperation(
                baseTicksInterval,
                (b, o, c) =>
                {
                    operationCountParameter.Add(c);
                    return c;
                },
                (a, b) => false);

            // Act
            int result = meteringOperation.MeterOperation(buffer, offset, count);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(operationCountParameter);
        }

        [Theory]
        [InlineData(1000, 0, 100, 10)]
        [InlineData(10000, 0, 500, 10)]
        public void MeterOperation_Should_When(int bufferSize, int offset, int count, int chunkSize)
        {
            // Arrange
            int baseTicksInterval = 1000;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            MeteringOperation meteringOperation = new MeteringOperation(
                baseTicksInterval,
                (b, o, c) =>
                {
                    operationCountParameter.Add(c);
                    Thread.Sleep(10);
                    return chunkSize;
                },
                (a, b) => false);

            // Act
            int result = meteringOperation.MeterOperation(buffer, offset, count);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(operationCountParameter);
        }

        [Theory]
        [InlineData(10000, 0, 100, 1000)]
        [InlineData(10000, 0, 500, 1000)]
        public void MeterOperation_ShouldKeepRateConstant_WhenOperationFunctionHasTheSameSpeedThanTheStartSpeed(int bufferSize, int offset, int count, int chunkSize)
        {
            // Arrange
            int baseTicksInterval = 500;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            MeteringOperation meteringOperation = new MeteringOperation(
                baseTicksInterval,
                (b, o, c) =>
                {
                    int actualCount = count / 10;
                    operationCountParameter.Add(actualCount);
                    Thread.Sleep(10 * actualCount);
                    return actualCount;
                },
                (a, b) => false);

            // Act
            int result = meteringOperation.MeterOperation(buffer, offset, count);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(operationCountParameter);
        }

        [Theory]
        [InlineData(10000, 0, 100, 1000)]
        [InlineData(10000, 0, 500, 1000)]
        public void MeterOperation_ShouldIncreaseRate_When(int bufferSize, int offset, int count, int chunkSize)
        {
            TextReader ert;
            StringReader sdfsdf;
            StreamReader sdfsfdsdf;
            // Arrange
            int baseTicksInterval = 500;

            byte[] buffer = new byte[bufferSize];

            List<int> operationCountParameter = new List<int>();

            MeteringOperation meteringOperation = new MeteringOperation(
                baseTicksInterval,
                (b, o, c) =>
                {
                    int actualCount = count / 10;
                    operationCountParameter.Add(actualCount);
                    Thread.Sleep(5 * actualCount);
                    return actualCount;
                },
                (a, b) => false);

            // Act
            int result = meteringOperation.MeterOperation(buffer, offset, count);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(operationCountParameter);
        }

    }
}
