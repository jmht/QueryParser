using System.Collections.Generic;

namespace HigginsThomas.QueryParser.Core
{
    public interface L1Lexer<T> : L1Parser<T>
    {
        /// <summary>
        /// Establish a list of keywords to be recognized by the lexer.  These generally
        /// must be identifiers (per the Lexer rules).  Values are matched without respect
        /// to case.  Recognized keywords are identified using <code>TokenType KEYWORD</code>
        /// </summary>
        /// <param name="keywords">List of keywords</param>
        /// <returns>this (allows chaining)</returns>
        L1Lexer<T> SetKeywordList(IList<string> keywords);

        /// <summary>
        /// Establish the list of valid identifier symbols.  The first list is the list of
        /// valid initial characters (typically letters), the second list is the list of
        /// valid non-initial characters.  Letters are matched without respect to case.
        /// </summary>
        /// <param name="initial">List of valid initial characters</param>
        /// <param name="subsequent">List of valid non-initial characters</param>
        /// <returns>this (allows chaining)</returns>
        L1Lexer<T> SetIdentifierCharacters(IList<char> initial, IList<char> subsequent);

        /// <summary>
        /// Establish a list of symbols to be recognized by the lexer.  These are returned
        /// as <code>TokenType SYMBOL</code>.
        /// </summary>
        /// <param name="symbols">List of symbols</param>
        /// <returns>this (allows chaining)</returns>
        L1Lexer<T> SetSymbolList(IList<string> symbols);
    }
}
