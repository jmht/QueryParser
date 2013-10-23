using System;
using HigginsThomas.QueryParser.Core;

namespace HigginsThomas.QueryParser.Lexer
{
    public struct Token {
        public enum Type {
            EOF,
            KEYWORD,
            IDENTIFIER,
            SYMBOL,
            NUMBER,
            STRING
        };
        public Type TokenType { get; private set; }
        public string Value { get; private set; }
        public StreamLocation Location { get; private set; }
        
        #region Constructors
        public Token(Type type) : this(type, null, new StreamLocation()) {}

        public Token(Type type, StreamLocation location) : this(type, null, location) {}

        public Token(Type type, String value) : this(type, value, new StreamLocation()) {}

        public Token(Type type, String value, StreamLocation location) : this()
        {
            TokenType = type;
            Value = value;
            Location = location;
        }

        public Token(Token that) : this(that.TokenType, that.Value) {}
        #endregion
        
        #region Standard methods
        public override int GetHashCode() {
            return HashCodeHelper.HashCode()
                                 .Add(TokenType)
                                 .Add(Value)
                                 .Hash;
        }

        public override bool Equals(Object x) {
            if ( x == null ) return false;
            if ( x.GetType() != typeof(Token) ) return false;
            Token that = (Token)x;
            return ( (this.TokenType == that.TokenType) && (this.Value == that.Value) );
        }

        public override string ToString() {
            return String.Format("{0}[{1}] @ [{2}, {3}]", TokenType, (Value != null) ? Value : "", Location.Line, Location.Column);
        }
        #endregion
    }
}
