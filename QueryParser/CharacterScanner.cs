using System;
using System.IO;
using HigginsThomas.QueryParser.Core;

namespace HigginsThomas.QueryParser.Scanner
{
    public class CharacterScanner : L1Parser<char>, IDisposable
    {
        private TextReader inputStream;
        private char lookAhead;
        private bool started = false;
        private bool atEOF = false;
        private StreamLocationTracker currentLocation = new StreamLocationTracker();


        #region Constructors
        public CharacterScanner(String s) : this(new StringReader(s)) { }

        public CharacterScanner(TextReader s)
        {
            this.inputStream = s;
        }
        #endregion

        #region Public methods
        #region L1Parser<> methods
        public void Start()
        {
            if (!started)
            {
                readNext();
                started = true;
            }
        }

        public bool Close()
        {
            Dispose();
            return true;
        }

        public char Peek()
        {
            if (!started) Start();     // guard: insure that we're started
            return lookAhead;
        }

        public char Consume()
        {
            if (IsEOF()) return EOF;
            char ch = Peek();
            currentLocation.Advance(ch == '\n');
            readNext();
            return ch;
        }

        public void Match(char x)
        {
            if (Peek() != x) throw new ParseException(String.Format("Error: expected {0} ({1}).  Found {2} ({3})", x, (int)x, Peek(), (int)Peek()), Location);
            Consume();
        }

        public bool HasNext()
        {
            return (Peek() != EOF);
        }

        public bool IsEOF()
        {
            return !HasNext();
        }

        public bool Reset()
        {
            return false;
        }

        public StreamLocation Location
        {
            get
            {
                Peek();
                return currentLocation.Location;
            }
        }

        public char EOF
        {
            get
            {
                return '\uFFFF';
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            inputStream.Dispose();
            lookAhead = EOF;
            atEOF = true;
        }
        #endregion

        #region Testing Members
        public void SetStartLocation(StreamLocation start)
        {
            if (started) throw new ParserException("Unable to set start location - parsing in progress.");
            currentLocation = new StreamLocationTracker(start);
        }
        #endregion
        #endregion

        #region Private methods
        private void readNext()
        {
            int ch = EOF;
            if (!atEOF)
            {
                ch = inputStream.Read();
                while (ch == EOF)               // if we actually read the EOF sentinel, skip it
                {
                    currentLocation.Advance();
                    ch = inputStream.Read();
                }
                if (ch == -1)                   // when the end is reached, return sentinel and set flag
                {
                    ch = EOF;
                    atEOF = true;
                }
            }
            lookAhead = (char)ch;
        }
        #endregion
    }
}
