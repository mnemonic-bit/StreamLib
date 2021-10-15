using System;
using System.IO;
using System.Text;

namespace StreamLib
{
    /// <summary>
    /// This class wrapps a stream around a string without duplicating
    /// the memory that is occupied by the string. It iterates over the
    /// characters of the source string value and returns a buffer which
    /// is always filled with bytes that can be put back through an encoding
    /// and result in valid characters which match the original source
    /// string content.
    /// </summary>
    public class StringStream : Stream
    {

        public StringStream(string source)
            : this(source, Encoding.UTF8)
        {
        }

        public StringStream(string source, Encoding encoding)
        {
            _source = source;
            _encoding = encoding;
            _lengthInBytes = encoding.GetByteCount(source);
            _maxBytesPerChar = encoding.GetMaxByteCount(1);

            _currentBytePosition = 0;
            _currentCharPosition = 0;
        }

        private readonly string _source;

        private int _currentBytePosition;
        private int _currentCharPosition;

        private Encoding _encoding;
        private readonly int _lengthInBytes;
        private readonly int _maxBytesPerChar;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public Encoding Encoding { get { return _encoding; } }

        public override void Flush() { }

        public override long Length => _lengthInBytes;

        public override long Position { get => _currentBytePosition; set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int usableSpaceInBuffer = Math.Min(buffer.Length - offset, count);

            int numberOfCharsToRead = GetNumberOfCharsThatFit(usableSpaceInBuffer);
            int totalBytesRead = _encoding.GetBytes(_source, _currentCharPosition, numberOfCharsToRead, buffer, offset);

            _currentCharPosition += numberOfCharsToRead;

            return totalBytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        public override void SetLength(long value) { throw new NotSupportedException(); }

        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

        /// <summary>
        /// Calculates the number of chars that will fit into the number
        /// of bytes given with the encoding that is used in this stream.
        /// </summary>
        /// <param name="numberOfBytes">The maximum number of bytes that can be used for the encoding.</param>
        /// <returns>Returns the number of chars that will fit into the given number of bytes.</returns>
        private int GetNumberOfCharsThatFit(int numberOfBytes)
        {
            int numberOfChars = 0;
            int charPosition = _currentCharPosition;
            int bytePosition = 0;

            while (_currentCharPosition < _source.Length && bytePosition < numberOfBytes)
            {
                int remainingBytesCount = numberOfBytes - bytePosition;
                int additionalCharsCount = CalculateCharsToRead(remainingBytesCount);

                int nextByteCount = _encoding.GetByteCount(_source, charPosition, additionalCharsCount);

                if (nextByteCount > remainingBytesCount)
                {
                    return numberOfChars;
                }

                bytePosition += nextByteCount;
                charPosition += additionalCharsCount;
                numberOfChars += additionalCharsCount;
            }

            return numberOfChars;
        }

        private int CalculateCharsToRead(int bytesToRead)
        {
            int charsToRead = bytesToRead / _maxBytesPerChar;

            if (charsToRead == 0)
            {
                charsToRead = 1;
            }

            return charsToRead;
        }

    }
}
