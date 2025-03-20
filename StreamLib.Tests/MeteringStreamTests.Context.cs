namespace StreamLib.Tests
{
    public partial class MeteringStreamTests
    {

        private EphemeralStream BuildEphemeralStream(int size)
        {
            var buffer = CreateBuffer(size);
            var ephemeralStream = new EphemeralStream(1, size);
            ephemeralStream.Write(buffer, 0, size);
            ephemeralStream.Position = 0;

            return ephemeralStream;
        }

        private MeteringStream BuildStream(int size, int bytesPerSecond = 10, int intervalLength = 1000)
        {
            var ephemeralStream = BuildEphemeralStream(size);

            var throttledStream = new ThrottledStream(ephemeralStream, bytesPerSecond, true, true, intervalLength);
            var meteringStream = new MeteringStream(throttledStream, intervalLength);

            return meteringStream;
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
