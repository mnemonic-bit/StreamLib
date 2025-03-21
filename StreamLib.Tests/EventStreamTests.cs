using FluentAssertions;
using System.IO;
using Xunit;

namespace StreamLib.Tests
{
    public partial class EventStreamTests
    {

        [Fact]
        public void CanRead_WhenCalled_ShouldPostEventOnCanReadWithSameFlag()
        {
            var debugStream = DebugStream();

            var canRead = false;
            var onCanReadWasCalled = false;
            debugStream.OnCanRead += (cr) => { canRead = cr; onCanReadWasCalled = true; };

            var result = debugStream.CanRead;

            result.Should().Be(canRead);
            onCanReadWasCalled.Should().BeTrue();
        }

        [Fact]
        public void CanSeek_WhenCalled_ShouldPostEventOnCanSeekWithSameFlag()
        {
            var debugStream = DebugStream();

            var canSeek = false;
            var onCanSeekWasCalled = false;
            debugStream.OnCanSeek += (cr) => { canSeek = cr; onCanSeekWasCalled = true; };

            var result = debugStream.CanSeek;

            result.Should().Be(canSeek);
            onCanSeekWasCalled.Should().BeTrue();
        }

        [Fact]
        public void CanWrite_WhenCalled_ShouldPostEventOnCanWriteWithSameFlag()
        {
            var debugStream = DebugStream();

            var canWrite = false;
            var onCanWriteWasCalled = false;
            debugStream.OnCanSeek += (cr) => { canWrite = cr; onCanWriteWasCalled = true; };

            var result = debugStream.CanWrite;

            result.Should().Be(canWrite);
            onCanWriteWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(LengthTestData))]
        public void Length_WhenCalled_ShouldPostEventOnLengthWithCorrectLength(int length)
        {
            var debugStream = DebugStream(length);

            var lengthParam = 0L;
            var lengthWasCalled = false;
            debugStream.OnLength += (l) => { lengthParam = l; lengthWasCalled = true; };

            var result = debugStream.Length;

            result.Should().Be(length);
            lengthParam.Should().Be(length);
            lengthWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(SetLengthTestData))]
        public void SetLength_WhenCalled_ShouldPostEventOnLengthWithCorrectLength(int oldLength, int newLength)
        {
            var debugStream = DebugStream(oldLength);

            var oldLengthParam = 0L;
            var newLengthParam = 0L;
            var setLengthWasCalled = false;
            debugStream.OnSetLength += (ol, nl) => { oldLengthParam = ol; newLengthParam = nl; setLengthWasCalled = true; };

            debugStream.SetLength(newLength);

            oldLengthParam.Should().Be(oldLength);
            newLengthParam.Should().Be(newLength);
            setLengthWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(SetPositionTestData))]
        public void SetPosition_WhenCalled_ShouldPostEventOnLengthWithCorrectLength(long position)
        {
            var debugStream = DebugStream();

            var oldPositionParam = 0L;
            var newPositionParam = 0L;
            var setPositionWasCalled = false;
            debugStream.OnSetPosition += (op, np) => { oldPositionParam = op; newPositionParam = np; setPositionWasCalled = true; };

            debugStream.Position = position;

            oldPositionParam.Should().Be(0);
            newPositionParam.Should().Be(position);
            setPositionWasCalled.Should().BeTrue();
        }

        [Fact]
        public void Flush_WhenCalled_ShouldPostEventOnFlush()
        {
            var debugStream = DebugStream();

            var onFlushWasCalled = false;
            debugStream.OnFlush += () => { onFlushWasCalled = true; };

            debugStream.Flush();

            onFlushWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(WriteTestData))]
        public void Read_WhenCalled_ShouldPostEventOnWriteReflectingParametersThatWerePassed(byte[] buffer, int offset, int count)
        {
            var debugStream = DebugStream();

            var bufferParam = new byte[] { };
            var offsetParam = 0;
            var countParam = 0;
            var resultParam = 0;
            var onReadWasCalled = false;
            debugStream.OnRead += (b, o, c, r) => { bufferParam = b; offsetParam = o; countParam = c; resultParam = r; onReadWasCalled = true; };

            var result = debugStream.Read(buffer, offset, count);

            bufferParam.Should().BeEquivalentTo(buffer);
            offsetParam.Should().Be(offset);
            countParam.Should().Be(count);
            resultParam.Should().Be(result);
            onReadWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(SeekTestData))]
        public void Seek_WhenCalled_ShouldPostEventOnSeekReflectingParametersThatWerePassed(long offset, SeekOrigin seekOrigin)
        {
            var debugStream = DebugStream();

            var offsetParam = 0L;
            var seekOriginParam = SeekOrigin.Begin;
            var positionParam = 0L;
            var onSeekWasCalled = false;
            debugStream.OnSeek += (o, s, p) => { offsetParam = o; seekOriginParam = s; positionParam = p; onSeekWasCalled = true; };

            var result = debugStream.Seek(offset, seekOrigin);

            offsetParam.Should().Be(offset);
            seekOriginParam.Should().Be(seekOrigin);
            positionParam.Should().Be(result);
            onSeekWasCalled.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(WriteTestData))]
        public void Write_WhenCalled_ShouldPostEventOnWriteReflectingParametersThatWerePassed(byte[] buffer, int offset, int count)
        {
            var debugStream = DebugStream();

            var bufferParam = new byte[]{ };
            var offsetParam = 0;
            var countParam = 0;
            var onWriteWasCalled = false;
            debugStream.OnWrite += (b, o, c) => { bufferParam = b; offsetParam = o; countParam = c; onWriteWasCalled = true; };

            debugStream.Write(buffer, offset, count);

            bufferParam.Should().BeEquivalentTo(buffer);
            offsetParam.Should().Be(offset);
            countParam.Should().Be(count);
            onWriteWasCalled.Should().BeTrue();
        }

    }
}
