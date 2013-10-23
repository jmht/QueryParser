using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HigginsThomas.QueryParser.Core;

namespace HigginsThomas.QueryParser.Lexer
{
    public class ExpressionLexer : L1Lexer<Token> 
    {
        // default values
        private static readonly HashSet<string> DefaultKeywordSet = new HashSet<string>();
        private static readonly List<string> DefaultSymbolSet = new List<string>()
            {"~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "+", "=", "/",
             "<", ">", ",", ".", "[", "]", "{", "}", "|", "\\", ":", ";", "\u2208",
             "!=", "&&", "==", "<=", "<>", ">=", "||"};
        private static readonly HashSet<char> DefaultIdentifierInitialSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz".ToList<char>());
        private static readonly HashSet<char> DefaultIdentifierFollowSet  = new HashSet<char>("abcedfghijklmnopqrstuvwxyz0123456789_-".ToList<char>());
        private static readonly List<char> Quotes = "\"'".ToList<char>();
        private static readonly char Escape = '\\';

        // Active configuration
        public ISet<char> IdentifierInitialSet { get; private set; }
        public ISet<char> IdentifierFollowSet { get; private set; }
        public IDictionary<string, bool> SymbolMap { get; private set; }
        public ISet<char> SymbolInitialSet { get; private set; }
        public ISet<string> KeywordSet { get; private set; }

        // parser data
        private L1Parser<char> inputStream;
        private Token lookAhead;
        private bool started = false;


        #region Constructors
        public ExpressionLexer(L1Parser<char> parser) 
        {
            inputStream = parser;
            IdentifierInitialSet = new HashSet<char>(DefaultIdentifierInitialSet);
            IdentifierFollowSet = new HashSet<char>(DefaultIdentifierFollowSet);
            computeSymbolMap(DefaultSymbolSet);
            KeywordSet = new HashSet<string>();
        }
        #endregion

        #region Public methods
        #region L1Lexer<Token> methods
        public L1Lexer<Token> SetKeywordList(IList<string> keywords)
        {
            if (started) throw new ParserException("Unable to set keyword list - parsing already started.");
            KeywordSet = new HashSet<string>();
            foreach ( string keyword in keywords )
            {
                KeywordSet.Add(keyword.ToLower());
            }
            return this;
        }

        public L1Lexer<Token> SetIdentifierCharacters(IList<char> initial, IList<char> subsequent)
        {
            if (started) throw new ParserException("Unable to set identifier character set - parsing already started.");
            IdentifierInitialSet = new HashSet<char>();
            foreach (char ch in initial)
            {
                IdentifierInitialSet.Add(Char.ToLower(ch));
            }
            IdentifierFollowSet = new HashSet<char>();
            foreach (char ch in subsequent)
            {
                IdentifierFollowSet.Add(Char.ToLower(ch));
            }
            return this;
        }

        public L1Lexer<Token> SetSymbolList(IList<string> symbols)
        {
            if (started) throw new ParserException("Unable to set symbol list - parsing already started.");
            computeSymbolMap(symbols);
            return this;
        }
        #endregion

        #region L1Parser<Token> methods
        public void Start()
        {
            initialize();
            started = true;
        }

        public bool Close()
        {
            return inputStream.Close();
        }

        public Token Peek() 
        {
            if (!started) Start();
            return lookAhead;
        }

        public Token Consume() 
        {
            Token token = Peek();
            readNext();
            return token;
        }

        public void Match(Token t) 
        {
            if ( !Peek().Equals(t) ) throw new ParseException(String.Format("Error: expected {0}. Found {1}.", t, Peek()), inputStream.Location);
            Consume();
        }

        public bool HasNext()
        {
            return (!Peek().TokenType.Equals(Token.Type.EOF));
        }

        public bool IsEOF()
        {
            return !HasNext();
        }

        public bool Reset()
        {
            try
            {
                bool resp = inputStream.Reset();
                if ( resp ) initialize();   // Only if underlying stream was able to reset
                return resp;
            }
            catch ( Exception )
            {
                return false;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return Peek().Location;
            }
        }

        public Token EOF
        {
            get
            {
                return new Token(Token.Type.EOF);
            }
        }
        #endregion
        #endregion

        #region Private methods
        private void initialize()
       {
           inputStream.Start();
           readNext();
        }

        private void computeSymbolMap(IList<string> symbols)
        {
            SymbolInitialSet = new HashSet<char>();
            SymbolMap = new Dictionary<string, bool>();

            foreach (string symbol in symbols)
            {
                SymbolInitialSet.Add(symbol[0]);
                for( int i = 1; i <= symbol.Length; ++i )
                {
                    string s = symbol.Substring(0, i);
                    bool f;
                    SymbolMap.TryGetValue(s, out f);
                    SymbolMap[s] = (f || (i == symbol.Length));
                }
            }
        }

        private void readNext()
        {
            Token token;

            if ( inputStream.HasNext() && Char.IsWhiteSpace(inputStream.Peek()) ) skipWhitespace();

            if ( inputStream.IsEOF() ) token = new Token(Token.Type.EOF, inputStream.Location);
            else if (IdentifierInitialSet.Contains(Char.ToLower(inputStream.Peek()))) token = recognizeIdentifier();
            else if ( Char.IsDigit(inputStream.Peek()) ) token = recognizeNumber();
            else if ( SymbolInitialSet.Contains(inputStream.Peek()) ) token = recognizeSymbol();
            else if ( Quotes.Contains(inputStream.Peek()) ) token = recognizeString();
            else throw new ParseException(String.Format("Unrecognized symbol: {0} ({1})", inputStream.Peek(), (int)inputStream.Peek()), inputStream.Location);
            lookAhead = token;
        }

        private void skipWhitespace()
        {
            while ( Char.IsWhiteSpace(inputStream.Peek()) && inputStream.HasNext() ) inputStream.Consume();
        }

        private Token recognizeIdentifier()
        {
            StreamLocation location = new StreamLocation(inputStream.Location);
            StringBuilder buf = new StringBuilder();
            while ( IdentifierFollowSet.Contains(Char.ToLower(inputStream.Peek())) ) 
            {
                buf.Append(inputStream.Consume());
            }
            string ident = buf.ToString().ToLower();
            if (KeywordSet.Contains(ident))
            {
                return new Token(Token.Type.KEYWORD, buf.ToString(), location);
            }
            else
            {
                return new Token(Token.Type.IDENTIFIER, buf.ToString(), location);
            }
        }

        private Token recognizeNumber()
        {
            StreamLocation location = new StreamLocation(inputStream.Location);
            StringBuilder buf = new StringBuilder();
            while ( Char.IsDigit(inputStream.Peek()) ) buf.Append(inputStream.Consume());
            if ( inputStream.Peek().Equals('.') )
            {
                buf.Append(inputStream.Consume());
                while ( Char.IsDigit(inputStream.Peek()) ) buf.Append(inputStream.Consume());
            }
            if ( inputStream.Peek().Equals('e') || inputStream.Peek().Equals('E') )
            {
                buf.Append(inputStream.Consume());
                if ( inputStream.Peek().Equals('+') || inputStream.Peek().Equals('-') )
                {
                    buf.Append(inputStream.Consume());
                }
                if ( !Char.IsDigit(inputStream.Peek()) ) throw new ParseException(String.Format("Malformed number: {0}", buf), inputStream.Location);
                while ( Char.IsDigit(inputStream.Peek()) ) buf.Append(inputStream.Consume());
            }
            return new Token(Token.Type.NUMBER, buf.ToString(), location);
        }

        private Token recognizeSymbol()
        {
            StreamLocation location = new StreamLocation(inputStream.Location);
            StringBuilder buf = new StringBuilder();
            buf.Append(inputStream.Consume());
            bool match = false;
            while ( SymbolMap.TryGetValue(buf.ToString() + inputStream.Peek(), out match) )
            {
                buf.Append(inputStream.Consume());
            }
            //match = false;
            if (SymbolMap.TryGetValue(buf.ToString(), out match))
            {
                if (match)
                {
                    return new Token(Token.Type.SYMBOL, buf.ToString(), location);
                }
            }
            throw new ParseException(String.Format("Unexpected symbol '{0}' following '{1}'", inputStream.Peek(), buf), inputStream.Location);
        }

        private Token recognizeString()
        {
            const char EOLN = '\n';
            StreamLocation location = new StreamLocation(inputStream.Location);
            char delim = inputStream.Peek();
            StringBuilder buf = new StringBuilder();
            inputStream.Consume();       // skip leading delimiter
            while ( !inputStream.Peek().Equals(delim) )
            {
                if ( !inputStream.HasNext() ) throw new ParseException("Unexpected EOF (unterminated string)", inputStream.Location);
                if (inputStream.Peek().Equals(EOLN)) throw new ParseException("Unexpected EOLN (unterminated string)", inputStream.Location);
                if ( inputStream.Peek().Equals(Escape) )
                {
                    buf.Append(recognizeEscapeCharacter());
                }
                else
                {
                    buf.Append(inputStream.Consume());
                }
            }
            inputStream.Consume();    // skip trailing delimiter
            return new Token(Token.Type.STRING, buf.ToString(), location);
        }

        private char recognizeEscapeCharacter()
        {
            inputStream.Consume();     // consume escape character itself
            switch ( inputStream.Peek() ) 
            {
                case 'b': inputStream.Consume();
                        return '\b';

                case 'f': inputStream.Consume();
                        return '\f';

                case 'n': inputStream.Consume();
                        return '\n';

                case 'r': inputStream.Consume();
                        return '\r';

                case 't': inputStream.Consume();
                        return '\t';

                case 'u': inputStream.Consume();
                        StringBuilder code = new StringBuilder();
                        for ( int i = 0; i < 4; ++i )
                        {
                            if ( Char.IsDigit(inputStream.Peek()) )
                            {
                                code.Append(inputStream.Consume());
                            }
                            else
                            {
                                throw new ParseException(String.Format("Malformed unicode sequence: \\u{0}{1}", code, inputStream.Peek()), inputStream.Location);
                            }
                        }
                        return Convert.ToChar(Convert.ToInt16(code));

                default:
                        if ( inputStream.IsEOF() ) throw new ParseException("Malformed escape sequence: \\<EOF>", inputStream.Location);
                        return inputStream.Consume();
            }
        }
        #endregion
    }
}
