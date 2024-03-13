//using System;
//using System.IO;

//namespace StreamLib
//{
//    public class ChunkedWriteStream : Stream
//    {

//        public override bool CanRead => false;

//        public override bool CanSeek => false;

//        public override bool CanWrite => _baseStream.CanWrite;

//        public override long Length => _baseStream.Length;

//        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public ChunkedWriteStream(Stream baseStream, int chunkLength = 0)
//        {
//            _baseStream = baseStream;
//            _chunkLength = chunkLength;
//            _bytesWrittenInChunk = 0;
//        }

//        public override void Flush()
//        {
//            _baseStream.Flush();
//        }

//        public override int Read(byte[] buffer, int offset, int count)
//        {
//            throw new NotImplementedException();
//        }

//        public override long Seek(long offset, SeekOrigin origin)
//        {
//            throw new NotImplementedException();
//        }

//        public override void SetLength(long value)
//        {
//            throw new NotImplementedException();
//        }

//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            byte[] chunks = ChunkBuffer(buffer.AsSpan<byte>(offset, count), _chunkLength - _bytesWrittenInChunk);


//            _bytesWrittenInChunk += count;


//            if (_bytesWrittenInChunk > _chunkLength)
//            {
//                _baseStream.Write(_chunkSeparatorBytes, 0, _chunkSeparatorBytes.Length);
//                _bytesWrittenInChunk = _chunkLength;
//            }

//            _baseStream.Write(buffer, offset, count);
//        }


//        private readonly Stream _baseStream;
//        private readonly int _chunkLength;

//        private bool _newChunk = true;
//        private int _bytesWrittenInChunk;

//        private static readonly byte[] _chunkSeparatorBytes = new byte[] { (byte)'\r', (byte)'\n' };
//        private static readonly byte[] _finalChunkMarkerBytes = new byte[] { (byte)'0', (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n', };


//        private byte[] ChunkBuffer(ReadOnlySpan<byte> buffer, int bytesLeftInChunk = 0)
//        {
//            int pos = 0;
//            while (pos < count)
//            {

//            }
//        }

//    }
//}
