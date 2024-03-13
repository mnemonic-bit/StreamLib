//using System;
//using System.IO;

//namespace StreamLib
//{
//    public class ChunkedReadStream : Stream
//    {

//        public override bool CanRead => _baseStream.CanRead;

//        public override bool CanSeek => _baseStream.CanSeek;

//        public override bool CanWrite => _baseStream.CanWrite;

//        public override long Length => throw new NotImplementedException();

//        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        /// <summary>
//        /// Initializes a new chunked stream which can read from
//        /// a base stream chunked data, and de-chunk its content.
//        /// </summary>
//        /// <param name="baseStream"></param>
//        /// <param name="chunkLength"></param>
//        public ChunkedReadStream(Stream baseStream)
//        {
//            _baseStream = baseStream;
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
//            throw new NotImplementedException();
//        }


//        private Stream _baseStream;


//    }
//}
