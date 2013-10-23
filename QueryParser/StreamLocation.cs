using System;

namespace HigginsThomas.QueryParser.Core
{
    public struct StreamLocation
    {
        public int Offset { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }


        #region Constructors
        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="offset">Initial offset value</param>
        /// <param name="line">Initial line value</param>
        /// <param name="column">Initial column value</param>
        public StreamLocation(int offset, int line, int column) : this()
        {
            Offset = offset;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="that">Source instance</param>
        public StreamLocation(StreamLocation that) : this()
        {
            Offset = that.Offset;
            Line = that.Line;
            Column = that.Column;
        }
        #endregion

        #region Standard methods
        public override string ToString() {
            return String.Format("[{0}, {1}] (@{2})", Line, Column, Offset);
        }
        #endregion
    }
}
