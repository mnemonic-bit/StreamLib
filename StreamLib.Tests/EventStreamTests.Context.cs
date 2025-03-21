using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Tests
{
    public partial class EventStreamTests
    {

        private EphemeralStream BuildEphemeralStream(int size)
        {
            var buffer = CreateBuffer(size);
            var ephemeralStream = new EphemeralStream(1, size);
            ephemeralStream.Write(buffer, 0, size);
            ephemeralStream.Position = 0;

            return ephemeralStream;
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

        private EventStream DebugStream(int size = 100)
        {
            var ephemeralStream = BuildEphemeralStream(size);
            var debugStream = new EventStream(ephemeralStream);

            return debugStream;
        }

    }
}
