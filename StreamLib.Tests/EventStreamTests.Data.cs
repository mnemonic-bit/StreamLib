using System.Collections.Generic;
using System.IO;

namespace StreamLib.Tests
{
    public partial class EventStreamTests
    {

        public static IEnumerable<object[]> SeekTestData()
        {
            yield return new object[] { 0, SeekOrigin.Begin };
            yield return new object[] { 0, SeekOrigin.Current };
            yield return new object[] { 0, SeekOrigin.End };
            yield return new object[] { 10, SeekOrigin.Begin };
            yield return new object[] { 10, SeekOrigin.Current };
            yield return new object[] { 10, SeekOrigin.End };
        }

        public static IEnumerable<object[]> WriteTestData()
        {
            yield return new object[] { new byte[] { }, 0, 0 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5 }, 0, 0 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5 }, 0, 1 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5 }, 0, 4 };
            yield return new object[] { new byte[] { 1, 2, 3, 4, 5 }, 0, 5 };
        }

        public static IEnumerable<object[]> LengthTestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 10 };
        }

        public static IEnumerable<object[]> SetLengthTestData()
        {
            yield return new object[] { 0, 0 };
            yield return new object[] { 0, 1 };
            yield return new object[] { 1, 1 };
            yield return new object[] { 10, 20 };
        }

        public static IEnumerable<object[]> SetPositionTestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 10 };
        }

    }
}
