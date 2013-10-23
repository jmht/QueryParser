using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Lexer;


namespace HigginsThomas.QueryParser.Parser
{
    public class ExpressionParser<T>
    {
        #region Class constants
        static private readonly List<string> KEYWORDS = 
            new List<string>() { "and", "or", "not", "eq", "ne", "lt", "gt", "le", "ge", "in", "like", "null" };
        static private readonly List<string> SYMBOLS = 
            new List<string>() { "=", "==", "!=", "<>", "<", ">", "<=", ">=", "\u2208", "~", "[", "]", 
                                 ",", "{", "}", "(", ")", ":", "&", "&&", "|", "||", "-", "!" };
        static private readonly IList<char> IDENTIFIER_INITIAL_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToList<char>();
        static private readonly IList<char> IDENTIFIER_FOLLOW_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_0123456789".ToList<char>();

        static private readonly IDictionary<string, RelationNode.Operator> OP_MAP = 
            new Dictionary<string, RelationNode.Operator>() 
            {
                { "=" , RelationNode.Operator.EQ },
                { "==", RelationNode.Operator.EQ },
                { "eq", RelationNode.Operator.EQ },
                { "!=", RelationNode.Operator.NE },
                { "<>", RelationNode.Operator.NE },
                { "ne", RelationNode.Operator.NE },
                { "<" , RelationNode.Operator.LT },
                { "lt", RelationNode.Operator.LT },
                { ">" , RelationNode.Operator.GT },
                { "gt", RelationNode.Operator.GT },
                { "<=", RelationNode.Operator.LE },
                { "le", RelationNode.Operator.LE },
                { ">=", RelationNode.Operator.GE },
                { "ge", RelationNode.Operator.GE },
                { "in", RelationNode.Operator.IN },
                { "\u2208",RelationNode.Operator.IN },
                { "~" , RelationNode.Operator.LIKE },
                { "like", RelationNode.Operator.LIKE }
            };
        #endregion

        #region Public properties
        /// <summary>
        /// Definition of a mapping function to map an identifier to a <c>Field&lt;T&gt;</c>
        /// </summary>
        /// <param name="identifier">Name of the (potential) identifier</param>
        /// <returns>
        /// <c  cref="Field<T>">Field&lt;T&gt;</c> defining the identifier or <c>null</c> if the identifier is unrecognized.
        /// </returns>
        public delegate Field<T> IdentifierMap(string identifier);
        /// <summary>
        /// Established identifier mapping function for the parser.
        /// </summary>
        public IdentifierMap Identifiers { get; private set; }
        /// <summary>
        /// Established <c>ParameterExpression</c> to be used by the parser for the resulting Expression Tree.
        /// </summary>
        public ParameterExpression ExpressionParameter { get; private set; }
        #endregion

        #region Internal attributes
        protected L1Parser<Token> InputStream { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create an instance of an <c>ExpressionParser</c>.
        /// Requires a suitable lexer to provide the input token stream.
        /// </summary>
        /// <param name="lexer">an instance of a <c>L1Lexer&lt;Token&gt;</c></param>
        public ExpressionParser(L1Lexer<Token> lexer) 
        {
            lexer.SetKeywordList(KEYWORDS);
            lexer.SetSymbolList(SYMBOLS);
            lexer.SetIdentifierCharacters(IDENTIFIER_INITIAL_CHARSET, IDENTIFIER_FOLLOW_CHARSET);
            InputStream = lexer;
            ExpressionParameter = Expression.Parameter(typeof(T), "arg");
            Identifiers = ident => null;
        }
        #endregion
        
        #region Public methods
        /// <summary>
        /// Defines the recognized identifiers for the parser.
        /// </summary>
        /// <param name="map">
        /// An instance of <c>IdentifierMap</c> which serves as a mapping function from
        /// an identifier name to a <c>HigginsThomas.QueryParser.Field&lt;T&gt;</c>.
        /// The mapping function should return <c>null</c> for unrecognized identifiers.
        /// </param>
        /// <returns><c>this</c> (allows for method chaining)</returns>
        public ExpressionParser<T> SetIdentifierMap(IdentifierMap map)
        {
            Identifiers = map;
            return this;
        }

        /// <summary>
        /// Defines the Expression Parameter to use in constructing the expression
        /// tree.
        /// </summary>
        /// <param name="parm">An instance of <c>ParameterExpression</c></param>
        /// <returns><c>this</c> (allows for method chaining)</returns>
        public ExpressionParser<T> SetExpressionParameter(ParameterExpression parm)
        {
            ExpressionParameter = parm;
            return this;
        }

        /// <summary>
        /// Executes the parse.
        /// </summary>
        /// <returns>A <c>LambdaExpression</c> representing the parsed expression.</returns>
        /// <exception cref="ParserExpression">If the parse fails, an instance of <c>ParserExpression</c> will be thrown.</exception>
        public LambdaExpression Parse()
        {
            InputStream.Start();
            return ConvertToLambdaExpression(RecognizeExpression());
        }

        /// <summary>
        /// If possible, reset the input stream to the beginning.
        /// (This would allow for a second parsing run with different parameters.)
        /// </summary>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise</returns>
        public bool Reset() 
        {
            return InputStream.Reset();
        }
        #endregion

        #region Private methods
        #region Grammar Rules
        protected virtual Node RecognizeExpression() 
        {
            /* <expr> -> <orExpr> */
            Token EOF = new Token(Token.Type.EOF);
            if (!InputStream.Peek().Equals(EOF))
            {
                Node expr = RecognizeOrExpr();
                InputStream.Match(EOF);
                return expr;
            }
            else
            {
                /* no expression, result is "true" */
                return new TrueNode(InputStream.Location);
            }
        }

        protected virtual Node RecognizeOrExpr() 
        {
            /* <orExpr> -> <andExpr> ... */
            Node left = RecognizeAndExpr();
            while ( IsOR() ) 
            {
                StreamLocation location = InputStream.Location;
                /* ... (OR <andExpr>)* {left associative} */
                InputStream.Consume(); // consume 'or'
                Node right = RecognizeAndExpr();
                left = new ConjunctionNode(ConjunctionNode.Conjunction.OR, left, right, location);
            }
            return left;
        }

        protected virtual Node RecognizeAndExpr() 
        {
            /* <andExpr> -> <condExpr> ... */
            Node left = RecognizeNotExpr();
            while ( IsAND() ) 
            {
                StreamLocation location = InputStream.Location;
                /* ... (AND <condExpr>)* {left associative} */
                InputStream.Consume(); // consume 'and'
                Node right = RecognizeNotExpr();
                left = new ConjunctionNode(ConjunctionNode.Conjunction.AND, left, right, location);
            }
            return left;
        }

        protected virtual Node RecognizeNotExpr()
        {
            StreamLocation location = InputStream.Location;
            bool negate = false;
            while (IsNOT())
            {
                /* <notExpr> -> NOT <notExpr> */
                InputStream.Consume();
                negate = !negate;
            }
            /* <notExpr> -> <condExpr> */
            Node expr = RecognizeCondExpr();
            if (negate)
            {
                expr = new NegationNode(expr, location);
            }
            return expr;
        }

        protected virtual Node RecognizeCondExpr() 
        {
            StreamLocation location = InputStream.Location;
            if ( IsSymbol("(") ) 
            {
                /* <condExpr> -> ( <expr> ) */
                InputStream.Consume();
                Node expr = RecognizeOrExpr();
                if ( IsSymbol(")") ) 
                {
                    InputStream.Consume();
                } 
                else 
                {
                    throw new ParseException(String.Format("Expected ')' at end of expression.  Found {0}", InputStream.Peek()), InputStream.Location);
                }
                return new IdentityNode(expr, location);
            } else {
                /* <condExpr> -> <relation> */
                return RecognizeRelation();
            }
        }

        protected virtual Node RecognizeRelation() 
        {
            /* <relation> -> <operand> <operator> <operand> */
            ValueNode left = RecognizeOperand();
            StreamLocation location = InputStream.Location;   // consider the location of the operator as the location of the relation
            RelationNode.Operator op = RecognizeOperator();
            ValueNode right = RecognizeOperand();
            return new RelationNode(op, left, right, location);
        }

        protected virtual ValueNode RecognizeOperand() 
        {
            if ( IsIdentifier() ) 
            {
                return RecognizeIdentifier();
            } 
            else 
            {
                /* <operand> -> <value> */
                return RecognizeValue();
            }
        }

        protected virtual RelationNode.Operator RecognizeOperator() 
        {
            if ( IsSymbol() ) 
            {
                string value = InputStream.Peek().Value;
                if ( OP_MAP.ContainsKey(value) ) 
                {
                    /* <operator> -> = | == | != | <> | < | > | <= | >= | \u2208 | ~ */
                    InputStream.Consume(); // consume it
                    return OP_MAP[value];
                } 
                else 
                {
                    throw new ParseException(String.Format("Expected relational operator.  Found {0}", InputStream.Peek()), InputStream.Location);
                }
            } 
            else if ( IsKeyword() ) 
            {
                String value = InputStream.Peek().Value.ToLower();
                if ( OP_MAP.ContainsKey(value) ) 
                {
                    /* <operator> -> eq | ne | lt | gt | le | ge | in | like */
                    InputStream.Consume(); // consume it
                    return OP_MAP[value];
                } 
                else 
                {
                    throw new ParseException(String.Format("Expected relational operator.  Found {0}", InputStream.Peek()), InputStream.Location);
                }
            } 
            else 
            {
                throw new ParseException(String.Format("Expected relational operator.  Found {0}", InputStream.Peek()), InputStream.Location);
            }
        }

        protected virtual ValueNode RecognizeIdentifier()
        {
            /* <operand> -> <identifier> */
            String id = InputStream.Peek().Value.ToLower();
            Field<T> fieldRetriever = Identifiers(id);
            if (fieldRetriever != null)
            {
                StreamLocation location = InputStream.Location;
                InputStream.Consume();
                return new IdentifierNode<T>(id, fieldRetriever, ExpressionParameter, location);
            }
            else
            {
                throw new ParseException(String.Format("Unrecognized identifier '{0}'.", InputStream.Peek()), InputStream.Location);
            }
        }

        protected virtual ValueNode RecognizeValue() 
        {
            StreamLocation location = InputStream.Location;

            if ( IsSymbol("[") || IsSymbol(">") ) 
            {
                /* <value> -> <range> */
                return RecognizeRange();
            } 
            else if ( IsSymbol("{") ) 
            {
                /* <value> -> <set> */
                return RecognizeSet();
            } 
            else 
            {
                /* <value> -> <simpleValue> */
                return RecognizeSimpleValue();
            }
        }

        protected virtual SimpleValueNode RecognizeSimpleValue()
        {
            StreamLocation location = InputStream.Location;

            if (IsNumber() || IsSymbol("-"))
            {
                /* <simpleValue> -> <numberOrDate> */
                return RecognizeNumberOrDate(location);
            }
            else if (IsString())
            {
                /* <simpleValue> -> <string> */
                return new StringNode(InputStream.Consume().Value, location);
            }
            else
            {
                throw new ParseException(String.Format("Expected value.  Found {0}", InputStream.Peek()), InputStream.Location);
            }
        }

        protected virtual SimpleValueNode RecognizeNumberOrDate(StreamLocation location) 
        {
            if ( IsSymbol("-") ) 
            {
                /* <numberOrDate> -> - <number> */
                InputStream.Consume(); // consume '-'
                if (!IsNumber()) throw new ParseException(String.Format("Expected number following '-'. Found {0}", InputStream.Peek()), InputStream.Location);
                return new NumberNode("-" + InputStream.Consume().Value, location);
            } 
            else 
            {
                /* <numberOrDate> -> <number> [ <dateTail> ] */
                String number = InputStream.Consume().Value;
                if ( IsSymbol("-") ) 
                {
                    return RecognizeDateTail(number, location);
                } 
                else 
                {
                    return new NumberNode(number, location);
                }
            }
        }

        protected virtual SimpleValueNode RecognizeDateTail(String year, StreamLocation location) 
        {
            /* <dateTail> -> - <number> - <number> */
            InputStream.Consume(); // the first '-'
            if (!IsNumber()) throw new ParseException(String.Format("Expected month (of date).  Found {0}", InputStream.Peek()), InputStream.Location);
            String month = InputStream.Consume().Value;
            if ( !IsSymbol("-") ) throw new ParseException(String.Format("Expected day (of date).  Found {0}", InputStream.Peek()), InputStream.Location);
            InputStream.Consume(); // the second '-'
            if ( !IsNumber() ) throw new ParseException(String.Format("Expected day (of date).  Found {0}", InputStream.Peek()), InputStream.Location);
            String day = InputStream.Consume().Value;
            return new DateNode(year, month, day, location);
        }

        protected virtual ValueNode RecognizeRange() 
        {
            /* <range> -> [ | > <value> .. <value> < | ] */
            StreamLocation location = InputStream.Location;
            bool inclusiveLeft = InputStream.Consume().Value.Equals("[");
            SimpleValueNode left = RecognizeSimpleValue();
            if ( !IsSymbol(":") )  throw new ParseException(String.Format("Expected ':'.  Found {0}", InputStream.Peek()), InputStream.Location);
            InputStream.Consume();   // consume ':'
            SimpleValueNode right = RecognizeSimpleValue();
            if (!(IsSymbol("<") || IsSymbol("]"))) throw new ParseException(String.Format("Expected '<' or ']'.  Found {0}", InputStream.Peek()), InputStream.Location);
            bool inclusiveRight = InputStream.Consume().Value.Equals("]");
            return new RangeNode(left, inclusiveLeft, right, inclusiveRight, location);
        }

        protected virtual ValueNode RecognizeSet() 
        {
            /* <set> -> { ... */
            ISet<SimpleValueNode> s = new HashSet<SimpleValueNode>();
            StreamLocation location = InputStream.Location;
            InputStream.Consume();    // consume initial '{'
            if ( !IsSymbol("}") ) 
            {
                /* ... <value> <setTail> ... */
                s.Add(RecognizeSimpleValue());
                RecognizeSetTail(s);
            }
            /* ... } */
            if ( !IsSymbol("}") ) throw new ParseException(String.Format("Expected '}}'.  Found {0}", InputStream.Peek()), InputStream.Location);
            InputStream.Consume();   // consume final '}'
            return new SetNode(s, location);
        }

        protected virtual void RecognizeSetTail(ISet<SimpleValueNode> s) 
        {
            while ( IsSymbol(",") ) 
            {
                /* <setTail> ->  , <value> <setTail>* */
                InputStream.Consume(); // consume ','
                s.Add(RecognizeSimpleValue());
            }
        }
        #endregion

        #region Token type helper methods
        protected virtual bool IsOR() 
        {
            return ( IsKeyword("OR") || IsSymbol("|") || IsSymbol("||") );
        }

        protected virtual bool IsAND() 
        {
            return ( IsKeyword("AND") || IsSymbol("&") || IsSymbol("&&") );
        }

        protected virtual bool IsNOT() 
        {
            return ( IsKeyword("NOT") || IsSymbol("!") );
        }

        protected virtual bool IsKeyword(string kw = null)
        {
            if (!InputStream.Peek().TokenType.Equals(Token.Type.KEYWORD)) return false;
            return ((kw == null) || InputStream.Peek().Value.Equals(kw, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual bool IsIdentifier(string id = null) 
        {
            if ( !InputStream.Peek().TokenType.Equals(Token.Type.IDENTIFIER) ) return false;
            return ((id == null) || InputStream.Peek().Value.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual bool IsSymbol(string symbol = null) 
        {
            if ( !InputStream.Peek().TokenType.Equals(Token.Type.SYMBOL) ) return false;
            return ( (symbol == null) || InputStream.Peek().Value.Equals(symbol) );
        }

        protected virtual bool IsNumber() 
        {
            return ( InputStream.Peek().TokenType.Equals(Token.Type.NUMBER) );
        }

        protected virtual bool IsString() 
        {
            return ( InputStream.Peek().TokenType.Equals(Token.Type.STRING) );
        }
        #endregion

        #region Other private methods
        private LambdaExpression ConvertToLambdaExpression(Node expr)
        {
            return Expression.Lambda(expr.SubExpression, ExpressionParameter);
        }
        #endregion
        #endregion
    }
}
