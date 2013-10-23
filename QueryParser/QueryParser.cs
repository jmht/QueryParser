using System;
using System.IO;
using System.Linq.Expressions;

namespace HigginsThomas.QueryParser
{
    /// <summary>
    /// Implements a query parser over a data store containing <code>DataRecord</code>s.
    /// </summary>
    /// <typeparam name="DataRecord">
    /// Type of the content of the data store which is being queried.  Effectively, this
    /// is the "record" type of the data store being queried.
    /// </typeparam>
    public class QueryParser<DataRecord>
    {
        private Parser.ExpressionParser<DataRecord>.IdentifierMap identifierMap;

        #region Constructors
        /// <summary>
        /// Creates an instance of the parser with a specified set of field identifiers.
        /// </summary>
        /// <param name="map">
        /// A table of field identifiers, each paired with an associated delegate to 
        /// retrieve the field value from a provided <code>DataRecord</code> instance.
        /// </param>
        public QueryParser(Parser.ExpressionParser<DataRecord>.IdentifierMap map)
        {
            identifierMap = map;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parse a query string and return a delegate which implements the specification.
        /// The resulting delegate will return "true" if a provided <code>DataRecord</code>
        /// satisfies the query criteria and "false" otherwise.
        /// </summary>
        /// <param name="queryString">The query specification</param>
        /// <returns>A delegate which implements the query specification over a <code>DataRecord</code></returns>
        public Expression<Func<DataRecord, bool>> parse(TextReader queryString)
        {
            var parser = new Parser.ExpressionParser<DataRecord>(new Lexer.ExpressionLexer(new Scanner.CharacterScanner(queryString)));
            parser.SetIdentifierMap(identifierMap);

            LambdaExpression expr = parser.Parse();

            return (Expression<Func<DataRecord, bool>>)expr;
        }

        /// <summary>
        /// Parse a query string and return a delegate which implements the specification.
        /// The resulting delegate will return "true" if a provided <code>DataRecord</code>
        /// satisfies the query criteria and "false" otherwise.
        /// </summary>
        /// <param name="queryString">The query specification</param>
        /// <returns>A delegate which implements the query specification over a <code>DataRecord</code></returns>
        public Expression<Func<DataRecord, bool>> parse(string queryString)
        {
            return parse(new StringReader(queryString));
        }
        #endregion
    }
}
