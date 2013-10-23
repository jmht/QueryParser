namespace HigginsThomas.QueryParser.Core
{
    /// <summary>
    /// This interface describes the essential behaviors expected of a L1 parser.
    /// The parser will scan an input appropriate to the specific parser type, returning
    /// "tokens" of type <code>T</code>
    /// </summary>
    /// <typeparam name="T">The "token" type</typeparam>
    public interface L1Parser<T>
    {
        /// <summary>
        /// Initialize the parser.
        /// This operation should be called after any implementation specific configuration
        /// and before any other parse methods.
        /// </summary>
        void Start();

        /// <summary>
        /// Return the next token in the sequence without consuming it.
        /// </summary>
        /// <returns>Token</returns>
        T Peek();

        /// <summary>
        /// Consumes the next token in the sequence.
        /// </summary>
        /// <returns>The consumed token</returns>
        T Consume();

        /// <summary>
        /// Match the next token in the sequence against a given token.
        /// If the tokens match, then the matched token is consumed, else
        /// a <code>ParseException</code> will be thrown.
        /// </summary>
        /// <param name="t">The token to be matched</param>
        void Match(T t);

        /// <summary>
        /// Test if there are additional tokens in the stream.
        /// </summary>
        /// <returns>True if there is another token available.</returns>
        bool HasNext();

        /// <summary>
        /// Test if the stream is at the end (there are no more tokens).
        /// </summary>
        /// <returns>True if all tokens have been consumed</returns>
        bool IsEOF();

        /// <summary>
        /// If possible, reset the stream to the beginning.
        /// </summary>
        /// <returns>True if the stream is reset, false otherwise</returns>
        bool Reset();

        /// <summary>
        /// If possible, close the stream.
        /// </summary>
        /// <returns>True if the stream is closed, false otherwise</returns>
        bool Close();

        /// <summary>
        /// The current location in the stream (the location of the next
        /// token).
        /// </summary>
        StreamLocation Location { get; }

        /// <summary>
        /// The EOF sentinel token for the parser.  This should be a constant
        /// value which reflects the value which will be returned by Peek()
        /// and Consume() when IsEOF() is true and at no other time.
        /// </summary>
        T EOF { get; }
    }
}
