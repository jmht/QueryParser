using HigginsThomas.QueryParser.Core;

namespace HigginsThomas.QueryParser.Scanner
{
    public class StreamLocationTracker
    {
        private int offset = 0;
        private int line = 1;
        private int column = 1;


        /// <summary>
        /// Default constructor.  Sets starting state to beginning
        /// of stream (offset=0, line=1, column=1)
        /// </summary>
        public StreamLocationTracker() { }

        /// <summary>
        /// This constructor instance sets a starting state.  This exists
        /// primarily to support testing.
        /// </summary>
        /// <param name="start">Initial location</param>
        public StreamLocationTracker(StreamLocation start)
        {
            offset = start.Offset;
            line = start.Line;
            column = start.Column;
        }

        /// <summary>
        /// Return <code>StreamLocation</code> representing the current
        /// location as recorded by the tracker.
        /// </summary>
        public StreamLocation Location {
            get
            {
                return new StreamLocation(offset, line, column);
            }
        }

        /// <summary>
        /// Advance the stream location by one position.
        /// If newLine is set, then set to beginning of next line
        /// </summary>
        /// <param name="newLine">Advance line</param>
        public void Advance(bool newLine)
        {
            if (newLine)
            {
                NewLine();
            }
            else
            {
                Advance();
            }
        }

        /// <summary>
        /// Advance the stream location by one position.
        /// </summary>
        public void Advance()
        {
            offset++;
            column++;
        }

        /// <summary>
        /// Advance the stream location by one and set to 
        /// the start of the next line.
        /// </summary>
        public void NewLine()
        {
            offset++;
            line++;
            column = 1;
        }
    }
}
