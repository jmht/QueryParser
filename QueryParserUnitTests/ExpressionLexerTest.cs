using System;
using System.Collections.Generic;
using System.Linq;
using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Lexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HigginsThomas.QueryParser.UnitTests
{
    /// <summary>
    ///This is a test class for ExpressionLexerTest and is intended
    ///to contain all ExpressionLexerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExpressionLexerTest
    {
        private struct ExpectedOutcome
        {
            public Token token;
            public StreamLocation location;
        };

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionLexerConstructorTest()
        {
            ISet<char>   expectedIdentifierInitialSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz".ToList<char>());
            ISet<char>   expectedIdentifierFollowSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz_-0123456789".ToList<char>());
            ISet<char>   expectedSymbolInitialSet = new HashSet<char>("~!@#$%^&*()-+=/<>,.[]{}|\\:;\u2208".ToList<char>());
            ISet<string> expectedKeywordSet = new HashSet<string>();

            ExpressionLexer sut = new ExpressionLexer(new TestScanner());

            Assert.IsTrue(expectedIdentifierInitialSet.SetEquals(sut.IdentifierInitialSet), "IdentifierInitialSet");
            Assert.IsTrue(expectedIdentifierFollowSet.SetEquals(sut.IdentifierFollowSet), "IdentifierFollowSet");
            Assert.IsTrue(expectedSymbolInitialSet.SetEquals(sut.SymbolInitialSet), "SymbolInitialSet");
            Assert.IsTrue(expectedKeywordSet.SetEquals(sut.KeywordSet), "KeywordSet");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionLexerSetKeywordListTest_BeforeStart()
        {
            IList<string> testKeywordList = new List<string>() { "keyword1", "keyword2" };
            ISet<char> expectedIdentifierInitialSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz".ToList<char>());
            ISet<char> expectedIdentifierFollowSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz_-0123456789".ToList<char>());
            ISet<char> expectedSymbolInitialSet = new HashSet<char>("~!@#$%^&*()-+=/<>,.[]{}|\\:;\u2208".ToList<char>());
            ISet<string> expectedKeywordSet = new HashSet<string>(testKeywordList);
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());

            sut.SetKeywordList(testKeywordList);

            Assert.IsTrue(expectedIdentifierInitialSet.SetEquals(sut.IdentifierInitialSet), "IdentifierInitialSet");
            Assert.IsTrue(expectedIdentifierFollowSet.SetEquals(sut.IdentifierFollowSet), "IdentifierFollowSet");
            Assert.IsTrue(expectedSymbolInitialSet.SetEquals(sut.SymbolInitialSet), "SymbolInitialSet");
            Assert.IsTrue(expectedKeywordSet.SetEquals(sut.KeywordSet), "KeywordSet");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll"), ExpectedException(typeof(ParserException))]
        public void ExpressionLexerSetKeywordListTest_AfterStart()
        {
            IList<string> testKeywordList = new List<string>() { "keyword1", "keyword2" };
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());
            sut.Start();

            sut.SetKeywordList(testKeywordList);
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionLexerSetIdentifierCharactersTest_BeforeStart()
        {
            IList<char> testInitialChars = new List<char>() { 'i', 'x' };
            IList<char> testFollowChars = new List<char>() { '0', '1', '2' };
            ISet<char> expectedIdentifierInitialSet = new HashSet<char>(testInitialChars);
            ISet<char> expectedIdentifierFollowSet = new HashSet<char>(testFollowChars);
            ISet<char> expectedSymbolInitialSet = new HashSet<char>("~!@#$%^&*()-+=/<>,.[]{}|\\:;\u2208".ToList<char>());
            ISet<string> expectedKeywordSet = new HashSet<string>();
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());

            sut.SetIdentifierCharacters(testInitialChars, testFollowChars);

            Assert.IsTrue(expectedIdentifierInitialSet.SetEquals(sut.IdentifierInitialSet), "IdentifierInitialSet");
            Assert.IsTrue(expectedIdentifierFollowSet.SetEquals(sut.IdentifierFollowSet), "IdentifierFollowSet");
            Assert.IsTrue(expectedSymbolInitialSet.SetEquals(sut.SymbolInitialSet), "SymbolInitialSet");
            Assert.IsTrue(expectedKeywordSet.SetEquals(sut.KeywordSet), "KeywordSet");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll"), ExpectedException(typeof(ParserException))]
        public void ExpressionLexerSetIdentifierCharactersTest_AfterStart()
        {
            IList<char> testInitialChars = new List<char>() { 'i', 'x' };
            IList<char> testFollowChars = new List<char>() { '0', '1', '2' };
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());
            sut.Start();

            sut.SetIdentifierCharacters(testInitialChars, testFollowChars);
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionLexerSetSymbolListTest_BeforeStart()
        {
            IList<string> testSymbols = new List<string>() { "+", "-", "*", "/", "=", "<", ">", "<=", ">=", "<>" };
            ISet<char> expectedIdentifierInitialSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz".ToList<char>());
            ISet<char> expectedIdentifierFollowSet = new HashSet<char>("abcedfghijklmnopqrstuvwxyz_-0123456789".ToList<char>());
            ISet<char> expectedSymbolInitialSet = new HashSet<char>(testSymbols.Select(sym => sym[0]).ToList<char>());
            ISet<string> expectedKeywordSet = new HashSet<string>();
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());

            sut.SetSymbolList(testSymbols);

            Assert.IsTrue(expectedIdentifierInitialSet.SetEquals(sut.IdentifierInitialSet), "IdentifierInitialSet");
            Assert.IsTrue(expectedIdentifierFollowSet.SetEquals(sut.IdentifierFollowSet), "IdentifierFollowSet");
            Assert.IsTrue(expectedSymbolInitialSet.SetEquals(sut.SymbolInitialSet), "SymbolInitialSet");
            Assert.IsTrue(expectedKeywordSet.SetEquals(sut.KeywordSet), "KeywordSet");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll"), ExpectedException(typeof(ParserException))]
        public void ExpressionLexerSetSymbolListTest_AfterStart()
        {
            IList<string> testSymbols = new List<string>() { "+", "-", "*", "/", "=", "<", ">", "<=", ">=", "<>" };
            ExpressionLexer sut = new ExpressionLexer(new TestScanner());
            sut.Start();

            sut.SetSymbolList(testSymbols);
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void testExpressionLexerNext() 
        {
            /*                  0         1        x 2    x     3         4
                                0....|....0....|...x.0....x|....0....|....0....| */
            string testInput = "ident 45 id2 & >= \"hello\" id_3 3.1456 5E45";
            ExpectedOutcome[] expected = { new ExpectedOutcome() { token= new Token(Token.Type.IDENTIFIER, "ident"), location= new StreamLocation(0, 1, 1)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.NUMBER, "45"), location= new StreamLocation(6, 1, 7)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.IDENTIFIER, "id2"), location= new StreamLocation(9, 1, 10)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.SYMBOL, "&"), location= new StreamLocation(13, 1, 14)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.SYMBOL, ">="), location= new StreamLocation(15, 1, 16)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.STRING, "hello"), location= new StreamLocation(18, 1, 19)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.IDENTIFIER, "id_3"), location= new StreamLocation(26, 1, 27)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.NUMBER, "3.1456"), location= new StreamLocation(31, 1, 32)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.NUMBER, "5E45"), location= new StreamLocation(38, 1, 39)},
                                           new ExpectedOutcome() { token= new Token(Token.Type.EOF, null), location= TestScanner.EOFLoc},
                                           new ExpectedOutcome() { token= new Token(Token.Type.EOF, null), location= TestScanner.EOFLoc}
                                         };

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();

            int index = 0;
            foreach ( ExpectedOutcome it in expected)
            {
                StreamLocation loc = sut.Location;
                Token token = sut.Consume();

                Assert.AreEqual(it.token.TokenType, token.TokenType, String.Format("Unexpected Type @ ${0}: ", index));
                Assert.AreEqual(it.token.Value, token.Value, String.Format("Unexpected tokenValue @ ${0}: ", index));
                Assert.AreEqual(it.location, token.Location);
                Assert.AreEqual(loc, token.Location, "lexer location should match token location. ");

                index++;
            }
        }

        [TestMethod]
        public void testPeek() 
        {
            string testInput = "a b  c d e f ";
            ExpectedOutcome expected = new ExpectedOutcome() {token= new Token(Token.Type.IDENTIFIER, "e"), location= new StreamLocation(9, 1, 10) };

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            Assert.AreEqual(expected.token.TokenType, sut.Peek().TokenType);
            Assert.AreEqual(expected.token.Value, sut.Peek().Value);
            Assert.AreEqual(expected.location, sut.Location);

            Assert.AreEqual(expected.token.TokenType, sut.Peek().TokenType, "unexpected change!");
            Assert.AreEqual(expected.token.Value, sut.Peek().Value, "unexpected change!");
            Assert.AreEqual(expected.location, sut.Location, "unexpected change!");
        }

        [TestMethod]
        public void testMatch_Success() 
        {
            string testInput = "a b  c d e f ";
            Token expected = new Token(Token.Type.IDENTIFIER, "e");

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            sut.Match(expected);
        }

        [TestMethod, ExpectedException(typeof(ParseException))]
        public void testMatch_Failure_Type() {
            string testInput = "a b  c d e f ";
            Token expected = new Token(Token.Type.SYMBOL, "e");

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            sut.Match(expected);
        }

        [TestMethod, ExpectedException(typeof(ParseException))]
        public void testMatch_Failure_Value() {
            string testInput = "a b  c d e f ";
            Token expected = new Token(Token.Type.IDENTIFIER, "x");

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            sut.Match(expected);
        }

        [TestMethod]
        public void testReset() 
        {
            var testInput = "a b  c d e f ";
            bool expectedResp = false;

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();
            bool resp = sut.Reset();

            Assert.AreEqual(expectedResp, resp);
        }

        [TestMethod]
        public void testEOF_Negative() {
            string testInput = "a";
            bool expectEOF = false;
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();

            Assert.AreEqual(expectEOF, sut.IsEOF());
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        [TestMethod]
        public void testEOF_Positive() {
            string testInput = "a";
            bool expectEOF = true;
            StreamLocation expectedLocation = TestScanner.EOFLoc;

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();
            sut.Consume();

            Assert.AreEqual(expectEOF, sut.IsEOF());
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        [TestMethod]
        public void testEOF_Immediate() {
            string testInput = "";
            bool expectEOF = true;
            StreamLocation expectedLocation = TestScanner.EOFLoc;

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(testInput));
            sut.Start();

            Assert.AreEqual(expectEOF, sut.IsEOF());
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        [TestMethod]
        public void testStringRecognition_Internal_SingleQuotes() {
            String input = "'hello world' : is a simple string delimited by single quotes";

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(input));
            sut.Start();
            Token result = sut.Peek();

            Assert.AreEqual(Token.Type.STRING, result.TokenType);
            Assert.AreEqual("hello world", result.Value);
        }

        [TestMethod]
        public void testStringRecognition_Internal_DoubleQuotes() {
            string input = "\"hello world\" : is a simple string delimited by double quotes";

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(input));
            sut.Start();
            Token result = sut.Peek();

            Assert.AreEqual(Token.Type.STRING, result.TokenType);
            Assert.AreEqual("hello world", result.Value);
        }

        [TestMethod]
        public void testStringRecognition_Internal_SingleQuotesWithEscapes() {
            string input = "'hello\\tto all the world\\'s people' : is a simple string delimited by single quotes";


            ExpressionLexer sut = new ExpressionLexer(new TestScanner(input));
            sut.Start();
            Token result = sut.Peek();

            Assert.AreEqual(Token.Type.STRING, result.TokenType);
            Assert.AreEqual("hello\tto all the world's people", result.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void testStringRecognition_EOF()
        {
            string input = "'No closing quote";

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(input));
            sut.Start();
            Token result = sut.Peek();
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void testStringRecognition_EOLN()
        {
            string input = "'No closing quote\nbefore EOLN'";

            ExpressionLexer sut = new ExpressionLexer(new TestScanner(input));
            sut.Start();
            Token result = sut.Peek();
        }

        #region Helpers
        private class TestScanner : L1Parser<char>
        {
            public class TestData
            {
                public char ch = ' ';
                public int offset = 0;
                public int line = 0;
                public int column = 0;
            }
            private struct StreamElement
            {
                public char ch;
                public StreamLocation loc;
            }
            public static StreamLocation EOFLoc = new StreamLocation(9999, 99, 99);
            private IList<StreamElement> input;
            private int current;

            #region Constructors
            public TestScanner(string testInput)
            {
                int offset = 0;
                int line = 1;
                int col = 1;

                input = new List<StreamElement>();
                foreach (char c in testInput)
                {
                    input.Add(new StreamElement() { ch= c, loc= new StreamLocation(offset++, line, col++) });
                    if (c == '\n')
                    {
                        line++;
                        col = 1;
                    }
                }
            }

            public TestScanner(TestData[] testData)
            {
                input = new List<StreamElement>();
                foreach(TestData elem in testData)
                {
                    input.Add(new StreamElement() { ch= elem.ch, loc= new StreamLocation(elem.offset, elem.line, elem.column) });
                }
                current = 0;
            }

            public TestScanner() : this(new TestScanner.TestData[0]) { }
            #endregion

            #region L1Parser<char> Members
            public void Start() { }

            public char Peek()
            {
                return (current < input.Count) ? input[current].ch : EOF;
            }

            public char Consume()
            {
                return (current < input.Count) ? input[current++].ch : EOF; ;
            }

            public void Match(char t)
            {
                if (t.Equals(input[current].ch))
                {
                    Consume();
                }
                else
                {
                    throw new ParseException("Match fail", Location);
                }
            }

            public bool HasNext()
            {
                return current < input.Count;
            }

            public bool IsEOF()
            {
                return !HasNext();
            }

            public bool Reset()
            {
                throw new NotImplementedException();
            }

            public bool Close()
            {
                throw new NotImplementedException();
            }

            public StreamLocation Location
            {
                get { return (current < input.Count) ? input[current].loc : EOFLoc; }
            }

            public char EOF
            {
                get { return '\uFFFF'; }
            }
            #endregion
        }
        #endregion
    }
}
