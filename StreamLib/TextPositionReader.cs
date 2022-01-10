using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace StreamLib
{
    /// <summary>
    /// The StreamPositionReader is a TextReader implementation which
    /// gives information about the current reading position in the
    /// text that is being read.
    /// </summary>
    public class TextPositionReader : TextReader
    {

        public const int FIRST_LINE_NUMBER = 1;
        public const int FIRST_COLUMN_NUMBER = 1;

        /// <summary>
        /// Initializes the reader with a given base-reader. If the line endings
        /// is given and not null, either line ending string is used in determining
        /// whether a current line ended. All line ending strings must not be longer
        /// than 2 characters, otherwise an ArgumentException is thrown.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="lineEndings"></param>
        public TextPositionReader(TextReader reader)
        {
            _reader = reader;
            _lineNumber = FIRST_LINE_NUMBER;
            _columnNumber = FIRST_COLUMN_NUMBER;
            _awaitingSlashNewline = false;
        }

        public int LineNumber => _lineNumber;

        public int ColumnNumber => _columnNumber;

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override int Peek()
        {
            return _reader.Peek();
        }

        public override int Read()
        {
            int ch = _reader.Read();
            AdjustPosition(ch);
            return ch;
        }

        private readonly TextReader _reader;

        private int _lineNumber;
        private int _columnNumber;
        private bool _awaitingSlashNewline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdjustPosition(int ch)
        {
            switch(IsLineEnding(ch))
            {
                case ELineBreak.NewLine:
                    {
                        _lineNumber++;
                        _columnNumber = FIRST_COLUMN_NUMBER;
                        break;
                    }
                case ELineBreak.Ignore:
                    break;
                case ELineBreak.SameLine:
                    {
                        _columnNumber++;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException($"The line-ending detection returned an unknown state. Please contact the maintainer of this library.");
                    }
            }
        }

        private enum ELineBreak { SameLine = 1, NewLine = 2, Ignore = 3 }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ELineBreak IsLineEnding(int ch)
        {
            if (ch == '\n')
            {
                if (_awaitingSlashNewline)
                {
                    _awaitingSlashNewline = false;
                    return ELineBreak.Ignore;
                }
                else
                {
                    return ELineBreak.NewLine;
                }
            }
            else if (ch == '\r')
            {
                _awaitingSlashNewline = true;
                return ELineBreak.NewLine;
            }
            return ELineBreak.SameLine;
        }

    }
}
