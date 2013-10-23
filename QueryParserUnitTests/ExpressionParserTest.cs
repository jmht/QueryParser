using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HigginsThomas.QueryParser.UnitTests
{
    /// <summary>
    ///This is a test class for ExpressionParserTest and is intended
    ///to contain all ExpressionParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExpressionParserTest
    {
        #region Public method tests
        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserConstructorTest()
        {
            SimpleTestLexer testLexer = new SimpleTestLexer();

            ExpressionParser<string> sut = new ExpressionParser<string>(testLexer);

            Assert.IsNotNull(sut.Identifiers, "Failed to initialize Identifier Map");
            Assert.IsNotNull(sut.ExpressionParameter, "Failed to initialize Expression parameter");
            Assert.IsNotNull(testLexer.Keywords, "Failed to set keyword list");
            Assert.IsNotNull(testLexer.IdentifierInitialCharset, "Failed to set identifier initial charset");
            Assert.IsNotNull(testLexer.IdentifierFollowCharset, "Failed to set identifier follow charset");
            Assert.IsNotNull(testLexer.Symbols, "Failed to set symbol list");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserSetIdentifierMapTest()
        {
            ExpressionParser<string>.IdentifierMap map = x => new IntegerField<string>(s => s.Length);
            SimpleTestLexer testLexer = new SimpleTestLexer();
            ExpressionParser<string> sut = new ExpressionParser<string>(testLexer);

            sut.SetIdentifierMap(map);

            Assert.AreEqual(map, sut.Identifiers, "Expected save of passed argument");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserSetExpressionParameterTest()
        {
            ParameterExpression parm = Expression.Parameter(typeof(int), "i");
            SimpleTestLexer testLexer = new SimpleTestLexer();
            ExpressionParser<string> sut = new ExpressionParser<string>(testLexer);

            sut.SetExpressionParameter(parm);

            Assert.AreEqual(parm, sut.ExpressionParameter, "Expected save of passed argument");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserParseTest()
        {
            ParameterExpression expectedArg = Expression.Parameter(typeof(int), "test");
            SimpleTestLexer testLexer = new SimpleTestLexer();
            ExpressionParser<string> sut = new ParseTestHarness(testLexer);
            sut.SetExpressionParameter(expectedArg);

            LambdaExpression result = sut.Parse();

            Assert.AreEqual(typeof(bool), result.ReturnType, "return type should be bool");
            Assert.AreEqual(1, result.Parameters.Count, "should take exactly one argument");
            Assert.AreEqual(expectedArg, result.Parameters[0], "argument should match the one supplied");
            Assert.AreEqual(typeof(ConstantExpression), result.Body.GetType(), "Should return constant \"true\" (result of mocked recognizer)");
            Assert.AreEqual(typeof(bool), ((ConstantExpression)result.Body).Type, "Should return constant \"true\" (result of mocked recognizer)");
            Assert.AreEqual(true, (bool)((ConstantExpression)result.Body).Value, "Should return constant \"true\" (result of mocked recognizer)");
            Assert.IsTrue(testLexer.StartInvoked, "Parse must initiate lexer");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserResetTest_True()
        {
            SimpleTestLexer testLexer = new SimpleTestLexer().SetReset(true);
            ExpressionParser<string> sut = new ExpressionParser<string>(testLexer);

            bool result = sut.Reset();

            Assert.IsTrue(testLexer.ResetInvoked, "Parse must initiate lexer");
            Assert.IsTrue(result, "Must return lexer response");
        }

        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ExpressionParserResetTest_False()
        {
            SimpleTestLexer testLexer = new SimpleTestLexer().SetReset(false);
            ExpressionParser<string> sut = new ExpressionParser<string>(testLexer);

            bool result = sut.Reset();

            Assert.IsTrue(testLexer.ResetInvoked, "Parse must initiate lexer");
            Assert.IsFalse(result, "Must return lexer response");
        }
        #endregion

        #region Grammar tests
        #region RecognizeExpression tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeExpressionTest_EmptyInput()
        {
            TNode expectedResultType = TNode.TRUE;
            var sut = new RecognizeExpressionTestHarness(new List<Lexer.Token>());

            Node result = sut.InvokeRecognizeExpression();

            Assert.AreEqual(expectedResultType, result.NodeType, "return should be TrueNode");
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeExpressionTest_NonEmptyInput()
        {
            Node expectedResult = RecognizeExpressionTestHarness.OrExprResult;
            var sut = new RecognizeExpressionTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "OR Expression") 
                          });

            Node result = sut.InvokeRecognizeExpression();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), 
                          String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeExpressionTest_SyntaxError()
        {
            var sut = new RecognizeExpressionTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "OR Expression"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Unrecognized remainder")
                          });

            Node result = sut.InvokeRecognizeExpression();
        }
        #endregion

        #region RecognizeOrExpr tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOrExprTest_Simple()
        {
            Node expectedResult = RecognizeOrExprTestHarness.AndExprResult;
            var sut = new RecognizeOrExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression")
                          });

            Node result = sut.InvokeRecognizeOrExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOrExprTest_Compound()
        {
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.OR,
                                                                 RecognizeOrExprTestHarness.AndExprResult,
                                                                 RecognizeOrExprTestHarness.AndExprResult,
                                                                 new StreamLocation());
            var sut = new RecognizeOrExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression 2")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeOrExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOrExprTest_Multiple()
        {
            // Verify left associative
            ConjunctionNode expectedLeftSubexpression = new ConjunctionNode(ConjunctionNode.Conjunction.OR,
                                                                            RecognizeOrExprTestHarness.AndExprResult,
                                                                            RecognizeOrExprTestHarness.AndExprResult,
                                                                            new StreamLocation());
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.OR, 
                                                                 expectedLeftSubexpression,
                                                                 RecognizeOrExprTestHarness.AndExprResult,
                                                                 new StreamLocation());
            var sut = new RecognizeOrExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression 2"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "AND Expression 3")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeOrExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
//            ConjunctionNode actualLeftSubexpression = (ConjunctionNode)result.GetLeft();
//            Assert.IsTrue(IsExpectedNode(expectedLeftSubexpression.GetLeft(), actualLeftSubexpression.GetLeft()), String.Format("expected: {0}, got: {1}", expectedLeftSubexpression.GetLeft(), actualLeftSubexpression.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedLeftSubexpression.GetRight(), actualLeftSubexpression.GetRight()), String.Format("expected: {0}, got: {1}", expectedLeftSubexpression.GetRight(), actualLeftSubexpression.GetRight()));
        }
        #endregion

        #region RecognizeAndExpr tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeAndExprTest_Simple()
        {
            Node expectedResult = RecognizeOrExprTestHarness.AndExprResult;
            var sut = new RecognizeAndExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression")
                          });

            Node result = sut.InvokeRecognizeAndExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeAndExprTest_Compound()
        {
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.AND,
                                                                 RecognizeAndExprTestHarness.NotExprResult,
                                                                 RecognizeAndExprTestHarness.NotExprResult,
                                                                 new StreamLocation());
            var sut = new RecognizeAndExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 2")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeAndExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeAndExprTest_Multiple()
        {
            // Verify left associative
            ConjunctionNode expectedLeftSubexpression = new ConjunctionNode(ConjunctionNode.Conjunction.AND,
                                                                            RecognizeAndExprTestHarness.NotExprResult,
                                                                            RecognizeAndExprTestHarness.NotExprResult,
                                                                            new StreamLocation());
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.AND,
                                                                 expectedLeftSubexpression,
                                                                 RecognizeAndExprTestHarness.NotExprResult,
                                                                 new StreamLocation());
            var sut = new RecognizeAndExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 2"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 3")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeAndExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
//            ConjunctionNode actualLeftSubexpression = (ConjunctionNode)result.GetLeft();
//            Assert.IsTrue(IsExpectedNode(expectedLeftSubexpression.GetLeft(), actualLeftSubexpression.GetLeft()), String.Format("expected: {0}, got: {1}", expectedLeftSubexpression.GetLeft(), actualLeftSubexpression.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedLeftSubexpression.GetRight(), actualLeftSubexpression.GetRight()), String.Format("expected: {0}, got: {1}", expectedLeftSubexpression.GetRight(), actualLeftSubexpression.GetRight()));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOrAndExprTest_Precedence()
        {
            // Verify AND over OR precedence
            ConjunctionNode expectedRightSubexpression = new ConjunctionNode(ConjunctionNode.Conjunction.AND,
                                                                             RecognizeAndExprTestHarness.NotExprResult,
                                                                             RecognizeAndExprTestHarness.NotExprResult,
                                                                             new StreamLocation());
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.OR,
                                                                 RecognizeAndExprTestHarness.NotExprResult,
                                                                 expectedRightSubexpression,
                                                                 new StreamLocation());
            var sut = new RecognizeAndExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 2"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "NOT Expression 3")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeOrExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
//            ConjunctionNode actualRightSubexpression = (ConjunctionNode)result.GetRight();
//            Assert.IsTrue(IsExpectedNode(expectedRightSubexpression.GetLeft(), actualRightSubexpression.GetLeft()), String.Format("expected: {0}, got: {1}", expectedRightSubexpression.GetLeft(), actualRightSubexpression.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedRightSubexpression.GetRight(), actualRightSubexpression.GetRight()), String.Format("expected: {0}, got: {1}", expectedRightSubexpression.GetRight(), actualRightSubexpression.GetRight()));
        }
        #endregion

        #region RecognizeNotExpr tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeNotExprTest_PassThrough()
        {
            Node expectedResult = RecognizeNotExprTestHarness.CondExprResult;
            var sut = new RecognizeNotExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "Cond Expression")
                          });

            Node result = sut.InvokeRecognizeNotExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeNotExprTest_Negate()
        {
            NegationNode expectedResult = new NegationNode(RecognizeNotExprTestHarness.CondExprResult, new StreamLocation());
            var sut = new RecognizeNotExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Cond Expression")
                          });

            NegationNode result = (NegationNode)sut.InvokeRecognizeNotExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetChild(), result.GetChild()), String.Format("expected: {0}, got: {1}", expectedResult.GetChild(), result.GetChild()));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeAndNotExprTest_Precedence()
        {
            // Verify NOT over AND precedence
            NegationNode expectedLeftSubexpression = new NegationNode(RecognizeNotExprTestHarness.CondExprResult,
                                                                      new StreamLocation());
            ConjunctionNode expectedResult = new ConjunctionNode(ConjunctionNode.Conjunction.AND,
                                                                 expectedLeftSubexpression,
                                                                 RecognizeNotExprTestHarness.CondExprResult,
                                                                 new StreamLocation());
            var sut = new RecognizeNotExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Cond Expression 1"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Cond Expression 2")
                          });

            ConjunctionNode result = (ConjunctionNode)sut.InvokeRecognizeAndExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetLeft(), result.GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult.GetLeft(), result.GetLeft()));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetRight(), result.GetRight()), String.Format("expected: {0}, got: {1}", expectedResult.GetRight(), result.GetRight()));
//            NegationNode actualLeftSubexpression = (NegationNode)result.GetLeft();
//            Assert.IsTrue(IsExpectedNode(expectedLeftSubexpression.GetChild(), actualLeftSubexpression.GetChild()), String.Format("expected: {0}, got: {1}", expectedLeftSubexpression.GetChild(), actualLeftSubexpression.GetChild()));
        }
        #endregion

        #region RecognizeCondExpr tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeCondExprTest_Relation()
        {
            Node expectedResult = RecognizeCondExprTestHarness.RelationResult;
            var sut = new RecognizeCondExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "Relation")
                          });

            Node result = sut.InvokeRecognizeCondExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeCondExprTest_SubExpression()
        {
            IdentityNode expectedResult = new IdentityNode(RecognizeCondExprTestHarness.OrExprResult, new StreamLocation());
            var sut = new RecognizeCondExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "("),
                              new Lexer.Token(Lexer.Token.Type.STRING, "OR Expression"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ")"),
                          });

            IdentityNode result = (IdentityNode)sut.InvokeRecognizeCondExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetChild(), result.GetChild()), String.Format("expected: {0}, got: {1}", expectedResult.GetChild(), result.GetChild()));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeCondExprTest_MalformedSubExpression()
        {
            IdentityNode expectedResult = new IdentityNode(RecognizeCondExprTestHarness.OrExprResult, new StreamLocation());
            var sut = new RecognizeCondExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "("),
                              new Lexer.Token(Lexer.Token.Type.STRING, "OR Expression"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Not ')'"),
                          });

            IdentityNode result = (IdentityNode)sut.InvokeRecognizeCondExpr();
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeNotCondExprTest_Precedence()
        {
            // Verify NOT over CondExpr precedence
            IdentityNode expectedChildSubexpression = new IdentityNode(RecognizeCondExprTestHarness.OrExprResult,
                                                                       new StreamLocation());
            NegationNode expectedResult = new NegationNode(expectedChildSubexpression,
                                                           new StreamLocation());
            var sut = new RecognizeCondExprTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "("),
                              new Lexer.Token(Lexer.Token.Type.STRING, "OR Expression"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ")"),
                          });

            NegationNode result = (NegationNode)sut.InvokeRecognizeNotExpr();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
//            Assert.IsTrue(IsExpectedNode(expectedResult.GetChild(), result.GetChild()), String.Format("expected: {0}, got: {1}", expectedResult.GetChild(), result.GetChild()));
//            IdentityNode actualChildSubexpression = (IdentityNode)result.GetChild();
//            Assert.IsTrue(IsExpectedNode(expectedChildSubexpression.GetChild(), actualChildSubexpression.GetChild()), 
//                          String.Format("expected: {0}, got: {1}", expectedChildSubexpression.GetChild(), actualChildSubexpression.GetChild()));
        }
        #endregion

        #region RecognizeRelation tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeRelationTest()
        {
            Node expectedResult = new RelationNode(RecognizeRelationTestHarness.OperationResult, 
                                                   RecognizeRelationTestHarness.OperandResult[0], 
                                                   RecognizeRelationTestHarness.OperandResult[1], 
                                                   new StreamLocation());
            var sut = new RecognizeRelationTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, "Operand 1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),
                              new Lexer.Token(Lexer.Token.Type.STRING, "Operand 2")
                          });

            Node result = sut.InvokeRecognizeRelation();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }
        #endregion

        #region RecognizeOperand tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOperandTest_Identifier()
        {
            Node expectedResult = RecognizeOperandTestHarness.IdentifierResult;
            var sut = new RecognizeOperandTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "Identifier")
                          });

            Node result = sut.InvokeRecognizeOperand();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOperandTest_Value()
        {
            Node expectedResult = RecognizeOperandTestHarness.ValueResult;
            var sut = new RecognizeOperandTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1") // !IDENTIFIER
                          });

            Node result = sut.InvokeRecognizeOperand();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }
        #endregion

        #region RecognizeOperator tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOperatorTest()
        {
            RelationNode.Operator[] expectedResultList = 
                {
                    RelationNode.Operator.EQ,
                    RelationNode.Operator.NE,
                    RelationNode.Operator.LT,
                    RelationNode.Operator.GE,
                    RelationNode.Operator.GT,
                    RelationNode.Operator.LE,
                    RelationNode.Operator.IN,
                    RelationNode.Operator.LIKE,
                    RelationNode.Operator.EQ,
                    RelationNode.Operator.EQ,
                    RelationNode.Operator.NE,
                    RelationNode.Operator.NE,
                    RelationNode.Operator.LT,
                    RelationNode.Operator.GE,
                    RelationNode.Operator.GT,
                    RelationNode.Operator.LE,
                    RelationNode.Operator.IN,
                    RelationNode.Operator.LIKE
                };
            var sut = new RecognizeOperatorTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),
                              new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "\u2208"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "~")
                          });

            for ( int i = 0; i < expectedResultList.Length; ++i )
            {
                RelationNode.Operator expectedOp = expectedResultList[i];
                RelationNode.Operator result = sut.InvokeRecognizeOperator();
                Assert.AreEqual(expectedOp, result, String.Format("Iteration {0}: expected: {1}, got: {2}", i, expectedOp, result));
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeOperatorTest_NotRecognized()
        {
            IList<Lexer.Token> testInput = new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),
                    new Lexer.Token(Lexer.Token.Type.SYMBOL, "*"),
                    new Lexer.Token(Lexer.Token.Type.NUMBER, "1")
                };
            var sut = new RecognizeOperatorTestHarness(testInput);

            for (int i = 0; i < testInput.Count; ++i)
            {
                try
                {
                    RelationNode.Operator result = sut.InvokeRecognizeOperator();
                    Assert.Fail(String.Format("Iteration {0}: Expected ParseException, got {1}", i, result));
                }
                catch (ParseException) { /* expected exception */ }
            }
        }
        #endregion

        #region RecognizeIdentifier tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeIdentifierTest()
        {
            var sut = new RecognizeIdentifierTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "Identifier")
                          });
            sut.SetIdentifierMap(new ExpressionParser<string>.IdentifierMap(s => string.Compare(s, "Identifier", true) == 0 ? new IntegerField<string>(x => 1) : null));
            Node expectedResult = sut.IdentifierResult;

            Node result = sut.InvokeRecognizeIdentifier();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeIdentifierTest_NotRecognized()
        {
            var sut = new RecognizeIdentifierTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "Unrecognized")
                          });
            sut.SetIdentifierMap(new ExpressionParser<string>.IdentifierMap(s => string.Compare(s, "Identifier", true) == 0 ? new IntegerField<string>(x => 1) : null));
            Node expectedResult = sut.IdentifierResult;

            Node result = sut.InvokeRecognizeIdentifier();
        }
        #endregion

        #region RecognizeValue tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeValueTest_Range_Inclusive()
        {
            Node expectedResult = RecognizeValueTestHarness.RangeResult;
            var sut = new RecognizeValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "["),
                          });

            Node result = sut.InvokeRecognizeValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeValueTest_Range_Exclusive()
        {
            Node expectedResult = RecognizeValueTestHarness.RangeResult;
            var sut = new RecognizeValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),
                          });

            Node result = sut.InvokeRecognizeValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeValueTest_Set()
        {
            Node expectedResult = RecognizeValueTestHarness.SetResult;
            var sut = new RecognizeValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "{"),
                          });

            Node result = sut.InvokeRecognizeValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeValueTest_Simple()
        {
            Node expectedResult = RecognizeValueTestHarness.SimpleResult;
            var sut = new RecognizeValueTestHarness(new List<Lexer.Token>() 
                          {
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                          });

            Node result = sut.InvokeRecognizeValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }
        #endregion

        #region RecognizeSimpleValue tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSimpleValueTest_Number_Positive()
        {
            Node expectedResult = RecognizeSimpleValueTestHarness.NumberResult;
            var sut = new RecognizeSimpleValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1")
                          });

            Node result = sut.InvokeRecognizeSimpleValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSimpleValueTest_Number_Negative()
        {
            Node expectedResult = RecognizeSimpleValueTestHarness.NumberResult;
            var sut = new RecognizeSimpleValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                          });

            Node result = sut.InvokeRecognizeSimpleValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSimpleValueTest_String()
        {
            string testString = "hello";
            Node expectedResult = new StringNode(testString, new StreamLocation());
            var sut = new RecognizeSimpleValueTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.STRING, testString),
                          });

            Node result = sut.InvokeRecognizeSimpleValue();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeSimpleValueTest_Unrecognized()
        {
            var sut = new RecognizeSimpleValueTestHarness(new List<Lexer.Token>() 
                          {
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*"),
                          });

            Node result = sut.InvokeRecognizeSimpleValue();
        }
        #endregion

        #region RecognizeDateTail tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeDateTailTest()
        {
            string testYear = "2011";
            string testMonth = "1";
            string testDay = "2";
            StreamLocation testLocation = new StreamLocation(5, 1, 3);
            Node expectedResult = new DateNode(testYear, testMonth, testDay, new StreamLocation());
            var sut = new RecognizeDateTailTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testMonth),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testDay)
                          });

            Node result = sut.InvokeRecognizeDateTail(testYear, testLocation);

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeDateTailTest_MissingDay()
        {
            string testYear = "2011";
            string testMonth = "1";
            string testDay = "2";
            StreamLocation testLocation = new StreamLocation(5, 1, 3);
            Node expectedResult = new DateNode(testYear, testMonth, testDay, new StreamLocation());
            var sut = new RecognizeDateTailTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testMonth),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*")
                          });

            Node result = sut.InvokeRecognizeDateTail(testYear, testLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeDateTailTest_MissingSecondDash()
        {
            string testYear = "2011";
            string testMonth = "1";
            string testDay = "2";
            StreamLocation testLocation = new StreamLocation(5, 1, 3);
            Node expectedResult = new DateNode(testYear, testMonth, testDay, new StreamLocation());
            var sut = new RecognizeDateTailTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testMonth),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testDay)
                          });

            Node result = sut.InvokeRecognizeDateTail(testYear, testLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeDateTailTest_MissingMonth()
        {
            string testYear = "2011";
            string testMonth = "1";
            string testDay = "2";
            StreamLocation testLocation = new StreamLocation(5, 1, 3);
            Node expectedResult = new DateNode(testYear, testMonth, testDay, new StreamLocation());
            var sut = new RecognizeDateTailTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "*"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "-"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, testDay)
                          });

            Node result = sut.InvokeRecognizeDateTail(testYear, testLocation);
        }
        #endregion

        #region RecognizeRange tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeRangeTest_Inclusive()
        {
            Node expectedResult = new RangeNode(RecognizeRangeTestHarness.valueResult[0], true, RecognizeRangeTestHarness.valueResult[1], true, new StreamLocation());
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "["),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ":"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "]")
                          });

            Node result = sut.InvokeRecognizeRange();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetLeft(), ((RangeNode)result).GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetRight(), ((RangeNode)result).GetRight()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveLeft(), ((RangeNode)result).IsInclusiveLeft());
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveRight(), ((RangeNode)result).IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeRangeTest_ExclusiveLeftInclusiveRight()
        {
            Node expectedResult = new RangeNode(RecognizeRangeTestHarness.valueResult[0], false, RecognizeRangeTestHarness.valueResult[1], true, new StreamLocation());
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ":"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "]")
                          });

            Node result = sut.InvokeRecognizeRange();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetLeft(), ((RangeNode)result).GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetRight(), ((RangeNode)result).GetRight()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveLeft(), ((RangeNode)result).IsInclusiveLeft());
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveRight(), ((RangeNode)result).IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeRangeTest_InclusiveLeftExclusiveRight()
        {
            Node expectedResult = new RangeNode(RecognizeRangeTestHarness.valueResult[0], true, RecognizeRangeTestHarness.valueResult[1], false, new StreamLocation());
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "["),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ":"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "<")
                          });

            Node result = sut.InvokeRecognizeRange();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetLeft(), ((RangeNode)result).GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetRight(), ((RangeNode)result).GetRight()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveLeft(), ((RangeNode)result).IsInclusiveLeft());
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveRight(), ((RangeNode)result).IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeRangeTest_Exclusive()
        {
            Node expectedResult = new RangeNode(RecognizeRangeTestHarness.valueResult[0], false, RecognizeRangeTestHarness.valueResult[1], false, new StreamLocation());
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ":"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "<")
                          });

            Node result = sut.InvokeRecognizeRange();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetLeft(), ((RangeNode)result).GetLeft()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.IsTrue(IsExpectedNode(((RangeNode)expectedResult).GetRight(), ((RangeNode)result).GetRight()), String.Format("expected: {0}, got: {1}", expectedResult, result));
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveLeft(), ((RangeNode)result).IsInclusiveLeft());
            Assert.AreEqual(((RangeNode)expectedResult).IsInclusiveRight(), ((RangeNode)result).IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeRangeTest_MissingClose()
        {
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ":"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*")
                          });

            Node result = sut.InvokeRecognizeRange();
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeRangeTest_MissingSeparator()
        {
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "["),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "1"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "]")
                          });

            Node result = sut.InvokeRecognizeRange();
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeRangeTest_MissingStartValue()
        {
            var sut = new RecognizeRangeTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "["),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "*"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*"),
                              new Lexer.Token(Lexer.Token.Type.NUMBER, "2"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "]")
                          });

            Node result = sut.InvokeRecognizeRange();
        }
        #endregion

        #region RecognizeSet tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSetAndSetTailTest_EmptySet()
        {
            Node expectedResult = new SetNode(new HashSet<SimpleValueNode>(), new StreamLocation());
            var sut = new RecognizeSetTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "{"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "}")
                          });

            Node result = sut.InvokeRecognizeSet();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            CollectionAssert.AreEquivalent(((SetNode)expectedResult).GetSet().ToList(), ((SetNode)result).GetSet().ToList());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSetAndSetTailTest_SingleElementSet()
        {
            Node expectedResult = new SetNode(new HashSet<SimpleValueNode>() { RecognizeSetTestHarness.valueResult[0] }, new StreamLocation());
            var sut = new RecognizeSetTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "{"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "A"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "}")
                          });

            Node result = sut.InvokeRecognizeSet();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            CollectionAssert.AreEquivalent(((SetNode)expectedResult).GetSet().ToList(), ((SetNode)result).GetSet().ToList());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RecognizeSetAndSetTailTest_MultipleElementSet()
        {
            Node expectedResult = new SetNode(new HashSet<SimpleValueNode>() { RecognizeSetTestHarness.valueResult[0], RecognizeSetTestHarness.valueResult[1] }, new StreamLocation());
            var sut = new RecognizeSetTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "{"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "A"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, ","),
                              new Lexer.Token(Lexer.Token.Type.STRING, "B"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "}")
                          });

            Node result = sut.InvokeRecognizeSet();

            Assert.IsTrue(IsExpectedNode(expectedResult, result), String.Format("expected: {0}, got: {1}", expectedResult, result));
            CollectionAssert.AreEquivalent(((SetNode)expectedResult).GetSet().ToList(), ((SetNode)result).GetSet().ToList());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RecognizeSetTest_MissingClose()
        {
            var sut = new RecognizeSetTestHarness(new List<Lexer.Token>() 
                          { 
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "{"),
                              new Lexer.Token(Lexer.Token.Type.STRING, "A"),
                              new Lexer.Token(Lexer.Token.Type.SYMBOL, "*")
                          });

            Node result = sut.InvokeRecognizeSet();
        }
        #endregion

        #region Token Test tests
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsOR()
        {
            IList<Tuple<Lexer.Token, bool>> data = new List<Tuple<Lexer.Token, bool>>()
                {
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "|"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "||"),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&&"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "\x2208"),    false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),     false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "~"),         false),
                };
            var sut = new TokenTestHarness(data.Select<Tuple<Lexer.Token, bool>, Lexer.Token>(x => x.Item1).ToList<Lexer.Token>());

            foreach ( Tuple<Lexer.Token, bool> test in data)
            {
                Lexer.Token token = test.Item1;
                bool expectedResult = test.Item2;

                Assert.AreEqual(expectedResult, sut.InvokeIsOR(), "Unexpected result for {0}", token.ToString());

                sut.Next();
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsAND()
        {
            IList<Tuple<Lexer.Token, bool>> data = new List<Tuple<Lexer.Token, bool>>()
                {
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "|"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "||"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),      true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&&"),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "\x2208"),    false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),     false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "~"),         false),
                };
            var sut = new TokenTestHarness(data.Select<Tuple<Lexer.Token, bool>, Lexer.Token>(x => x.Item1).ToList<Lexer.Token>());

            foreach (Tuple<Lexer.Token, bool> test in data)
            {
                Lexer.Token token = test.Item1;
                bool expectedResult = test.Item2;

                Assert.AreEqual(expectedResult, sut.InvokeIsAND(), "Unexpected result for {0}", token.ToString());

                sut.Next();
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsNOT()
        {
            IList<Tuple<Lexer.Token, bool>> data = new List<Tuple<Lexer.Token, bool>>()
                {
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "|"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "||"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&&"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),      true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "\x2208"),    false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),     false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "~"),         false),
                };
            var sut = new TokenTestHarness(data.Select<Tuple<Lexer.Token, bool>, Lexer.Token>(x => x.Item1).ToList<Lexer.Token>());

            foreach (Tuple<Lexer.Token, bool> test in data)
            {
                Lexer.Token token = test.Item1;
                bool expectedResult = test.Item2;

                Assert.AreEqual(expectedResult, sut.InvokeIsNOT(), "Unexpected result for {0}", token.ToString());

                sut.Next();
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsKeyword()
        {
            IList<Tuple<Lexer.Token, bool>> data = new List<Tuple<Lexer.Token, bool>>()
                {
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "|"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "||"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),      true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&&"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),      true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),         false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),        false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),       true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "\x2208"),    false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),     true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "~"),         false),
                };
            var sut = new TokenTestHarness(data.Select<Tuple<Lexer.Token, bool>, Lexer.Token>(x => x.Item1).ToList<Lexer.Token>());

            foreach (Tuple<Lexer.Token, bool> test in data)
            {
                Lexer.Token token = test.Item1;
                bool expectedResult = test.Item2;

                Assert.AreEqual(expectedResult, sut.InvokeIsKeyword(), "Unexpected result for {0}", token.ToString());

                sut.Next();
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsSymbol()
        {
            IList<Tuple<Lexer.Token, bool>> data = new List<Tuple<Lexer.Token, bool>>()
                {
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "OR"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "|"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "||"),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "AND"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "&&"),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NOT"),      false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "EQ"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "="),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "=="),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "NE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "!="),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<>"),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GT"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">"),         true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "<="),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "GE"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, ">="),        true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "IN"),       false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "\x2208"),    true),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.KEYWORD, "LIKE"),     false),
                    new Tuple<Lexer.Token, bool>(new Lexer.Token(Lexer.Token.Type.SYMBOL, "~"),         true),
                };
            var sut = new TokenTestHarness(data.Select<Tuple<Lexer.Token, bool>, Lexer.Token>(x => x.Item1).ToList<Lexer.Token>());

            foreach (Tuple<Lexer.Token, bool> test in data)
            {
                Lexer.Token token = test.Item1;
                bool expectedResult = test.Item2;

                Assert.AreEqual(expectedResult, sut.InvokeIsSymbol(), "Unexpected result for {0}", token.ToString());

                sut.Next();
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsIdentifier()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "x")
                });

            Assert.IsTrue(sut.InvokeIsIdentifier());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsIdentifier_False()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.KEYWORD, "x")
                });

            Assert.IsFalse(sut.InvokeIsIdentifier());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsIdentifier_Matched()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "x")
                });

            Assert.IsTrue(sut.InvokeIsIdentifier("x"));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsIdentifier_Unmatched()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "x")
                });

            Assert.IsFalse(sut.InvokeIsIdentifier("y"));
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsNumber()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.NUMBER, "1")
                });

            Assert.IsTrue(sut.InvokeIsNumber());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsNumber_False()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.IDENTIFIER, "x")
                });

            Assert.IsFalse(sut.InvokeIsNumber());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsString()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.STRING, "1")
                });

            Assert.IsTrue(sut.InvokeIsString());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void VerifyIsString_False()
        {
            var sut = new TokenTestHarness(new List<Lexer.Token>()
                {
                    new Lexer.Token(Lexer.Token.Type.NUMBER, "1")
                });

            Assert.IsFalse(sut.InvokeIsString());
        }
        #endregion
        #endregion

        #region Helpers
        private class SimpleTestLexer : L1Lexer<Lexer.Token>
        {
            internal IList<string> Keywords { get; private set; }
            internal IList<char> IdentifierInitialCharset { get; private set; }
            internal IList<char> IdentifierFollowCharset { get; private set; }
            internal IList<string> Symbols { get; private set; }
            internal bool StartInvoked { get; private set; }
            internal bool ResetInvoked { get; private set; }
            internal bool ResetValue { get; set; }

            internal SimpleTestLexer SetReset(bool p)
            {
                ResetValue = p;
                return this;
            }

            #region L1Lexer<Token> Members
            public L1Lexer<Lexer.Token> SetKeywordList(IList<string> keywords)
            {
                Keywords = keywords;
                return this;
            }

            public L1Lexer<Lexer.Token> SetIdentifierCharacters(IList<char> initial, IList<char> subsequent)
            {
                IdentifierInitialCharset = initial;
                IdentifierFollowCharset = subsequent;
                return this;
            }

            public L1Lexer<Lexer.Token> SetSymbolList(IList<string> symbols)
            {
                Symbols = symbols;
                return this;
            }
            #endregion

            #region L1Parser<Token> Members
            public void Start() 
            {
                StartInvoked = true;
            }

            public Lexer.Token Peek()
            {
                return EOF;
            }

            public Lexer.Token Consume()
            {
                return EOF;
            }

            public void Match(Lexer.Token t) { }

            public bool HasNext()
            {
                return false;
            }

            public bool IsEOF()
            {
                return true;
            }

            public bool Reset()
            {
                ResetInvoked = true;
                return ResetValue;
            }

            public bool Close()
            {
                throw new System.NotImplementedException();
            }

            public StreamLocation Location
            {
                get { return new StreamLocation(); }
            }

            public Lexer.Token EOF
            {
                get { return new Lexer.Token(Lexer.Token.Type.EOF); }
            }
            #endregion
        }

        private class ConfiguredTestLexer : L1Lexer<Lexer.Token>
        {
            private IList<Lexer.Token> TokenSequence { get; set; }
            private int TokenIndex { get; set; }

            public ConfiguredTestLexer(IList<Lexer.Token> tokenSequence)
            {
                TokenSequence = tokenSequence;
                TokenIndex = 0;
            }

            #region L1Lexer<Token> Members
            public L1Lexer<Lexer.Token> SetKeywordList(IList<string> keywords) { return this; }
            public L1Lexer<Lexer.Token> SetIdentifierCharacters(IList<char> initial, IList<char> subsequent) { return this; }
            public L1Lexer<Lexer.Token> SetSymbolList(IList<string> symbols) { return this; }
            #endregion

            #region L1Parser<Token> Members
            public void Start() { }

            public Lexer.Token Peek()
            {
                return HasNext() ? TokenSequence[TokenIndex] : EOF;
            }

            public Lexer.Token Consume()
            {
                return HasNext() ? TokenSequence[TokenIndex++] : EOF;
            }

            public void Match(Lexer.Token t) 
            {
                if (Peek().TokenType.Equals(t.TokenType))
                {
                    Consume();
                }
                else
                {
                    throw new ParseException("Match failure", Location);
                }
            }

            public bool HasNext()
            {
                return TokenIndex < TokenSequence.Count;
            }

            public bool IsEOF()
            {
                return !HasNext();
            }

            public bool Reset() { return false; }

            public bool Close() { return false; }

            public StreamLocation Location
            {
                get { return new StreamLocation(TokenIndex, 1, 1); }
            }

            public Lexer.Token EOF
            {
                get { return new Lexer.Token(Lexer.Token.Type.EOF); }
            }
            #endregion
        }

        private class ParseTestHarness : ExpressionParser<string>
        {
            // Constructor
            public ParseTestHarness(L1Lexer<Lexer.Token> lexer) : base(lexer) { }
            // Mock dependent method(s)
            protected override Node RecognizeExpression() { return new TrueNode(InputStream.Location); }
        }

        private class RecognizeExpressionTestHarness : ExpressionParser<string>
        {
            public static readonly Node OrExprResult = new StringNode("RecognizeOrExpr", new StreamLocation());
            // Constructor
            public RecognizeExpressionTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry point for test
            public Node InvokeRecognizeExpression() { return RecognizeExpression(); }
            // Mock dependent method(s)
            protected override Node RecognizeOrExpr() { /* Consider next token the "OR" expr */ InputStream.Consume(); return OrExprResult; }
        }

        private class RecognizeOrExprTestHarness : ExpressionParser<string>
        {
            public static readonly Node AndExprResult = new TrueNode(new StreamLocation());
            // Constructor
            public RecognizeOrExprTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry point for tests
            public Node InvokeRecognizeOrExpr() { return RecognizeOrExpr(); }
            // Mock dependent method(s)
            protected override Node RecognizeAndExpr() { /* Consider next token the "AND" expr */ InputStream.Consume(); return AndExprResult; }
        }

        private class RecognizeAndExprTestHarness : ExpressionParser<string>
        {
            public static readonly Node NotExprResult = new TrueNode(new StreamLocation());
            // Constructor
            public RecognizeAndExprTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeOrExpr() { return RecognizeOrExpr(); }
            public Node InvokeRecognizeAndExpr() { return RecognizeAndExpr(); }
            // Mock dependent method(s)
            protected override Node RecognizeNotExpr() { /* Consider next token the "NOT" expr */ InputStream.Consume(); return NotExprResult; }
        }

        private class RecognizeNotExprTestHarness : ExpressionParser<string>
        {
            public static readonly Node CondExprResult = new TrueNode(new StreamLocation());
            // Constructor
            public RecognizeNotExprTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeAndExpr() { return RecognizeAndExpr(); }
            public Node InvokeRecognizeNotExpr() { return RecognizeNotExpr(); }
            // Mock dependent method(s)
            protected override Node RecognizeCondExpr() { /* Consider next token the "Cond" expr */ InputStream.Consume(); return CondExprResult; }
        }

        private class RecognizeCondExprTestHarness : ExpressionParser<string>
        {
            public static readonly Node OrExprResult = new TrueNode(new StreamLocation());
            public static readonly Node RelationResult = new TrueNode(new StreamLocation());
            // Constructor
            public RecognizeCondExprTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeNotExpr() { return RecognizeNotExpr(); }
            public Node InvokeRecognizeCondExpr() { return RecognizeCondExpr(); }
            // Mock dependent method(s)
            protected override Node RecognizeOrExpr() { /* Consider next token the "OR" expr */ InputStream.Consume(); return OrExprResult; }
            protected override Node RecognizeRelation() { /* Consider next token the "Relation" */ InputStream.Consume(); return RelationResult; }
        }

        private class RecognizeRelationTestHarness : ExpressionParser<string>
        {
            public static readonly ValueNode[] OperandResult = { new NumberNode("1", new StreamLocation()), new NumberNode("2", new StreamLocation()) };
            public static readonly RelationNode.Operator OperationResult = RelationNode.Operator.EQ;
            private int operand = 0;
            // Constructor
            public RecognizeRelationTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeRelation() { return RecognizeRelation(); }
            // Mock dependent method(s)
            protected override ValueNode RecognizeOperand() { /* Consider next token the "Operand" */ InputStream.Consume(); return OperandResult[operand++]; }
            protected override RelationNode.Operator RecognizeOperator() { /* Consider next token the "Operator" */ InputStream.Consume(); return OperationResult; }
        }

        private class RecognizeOperandTestHarness : ExpressionParser<string>
        {
            public static readonly ValueNode IdentifierResult = new IdentifierNode<string>("Identifier", new IntegerField<string>(s => 1), Expression.Parameter(typeof(string)), new StreamLocation());
            public static readonly ValueNode ValueResult = new NumberNode("1", new StreamLocation());
            // Constructor
            public RecognizeOperandTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeOperand() { return RecognizeOperand(); }
            // Mock dependent method(s)
            protected override ValueNode RecognizeIdentifier() { /* Consider next token the "Value" */ InputStream.Consume(); return IdentifierResult; }
            protected override ValueNode RecognizeValue() { /* Consider next token the "Value" */ InputStream.Consume(); return ValueResult; }
        }

        private class RecognizeOperatorTestHarness : ExpressionParser<string>
        {
            // Constructor
            public RecognizeOperatorTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public RelationNode.Operator InvokeRecognizeOperator() { return RecognizeOperator(); }
            // Mock dependent method(s)
        }

        private class RecognizeIdentifierTestHarness : ExpressionParser<string>
        {
            public readonly Node IdentifierResult;
            // Constructor
            public RecognizeIdentifierTestHarness(IList<Lexer.Token> testTokenSequence) : base(new ConfiguredTestLexer(testTokenSequence)) 
            {
                IdentifierResult = new IdentifierNode<string>("identifier", new IntegerField<string>(s => 1), base.ExpressionParameter, new StreamLocation());
            }
            // Create visible entry points for tests
            public Node InvokeRecognizeIdentifier() { return RecognizeIdentifier(); }
            // Mock dependent method(s)
        }

        private class RecognizeValueTestHarness : ExpressionParser<string>
        {
            public static ValueNode RangeResult = new RangeNode(new NumberNode("1", new StreamLocation()), true, new NumberNode("2", new StreamLocation()), true, new StreamLocation());
            public static ValueNode SetResult = new SetNode(new HashSet<SimpleValueNode>() { new NumberNode("1", new StreamLocation()) }, new StreamLocation());
            public static SimpleValueNode SimpleResult = new NumberNode("1", new StreamLocation());
            // Constructor
            public RecognizeValueTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeValue() { return RecognizeValue(); }
            // Mock dependent method(s)
            protected override ValueNode RecognizeRange() { return RangeResult; }
            protected override ValueNode RecognizeSet() { return SetResult; }
            protected override SimpleValueNode RecognizeSimpleValue() { return SimpleResult; }
        }

        private class RecognizeSimpleValueTestHarness : ExpressionParser<string>
        {
            public static SimpleValueNode NumberResult = new NumberNode("1", new StreamLocation());
            // Constructor
            public RecognizeSimpleValueTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeSimpleValue() { return RecognizeSimpleValue(); }
            // Mock dependent method(s)
            protected override SimpleValueNode RecognizeNumberOrDate(StreamLocation loc) { return NumberResult; }
        }

        private class RecognizeNumberOrDateTestHarness : ExpressionParser<string>
        {
            private StreamLocation loc;
            // Constructor
            public RecognizeNumberOrDateTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public ValueNode InvokeRecognizeNumberOrDate(StreamLocation loc) { this.loc = loc;  return RecognizeNumberOrDate(loc); }
            // Mock dependent method(s)
            protected override SimpleValueNode RecognizeDateTail(string year, StreamLocation loc) 
            {
                Assert.AreEqual(this.loc, loc);
                return new DateNode(year, "1", "1", loc);
            }
        }

        private class RecognizeDateTailTestHarness : ExpressionParser<string>
        {
            // Constructor
            public RecognizeDateTailTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public ValueNode InvokeRecognizeDateTail(string year, StreamLocation loc) { return RecognizeDateTail(year, loc); }
            // Mock dependent method(s)
        }

        private class RecognizeRangeTestHarness : ExpressionParser<string>
        {
            public static SimpleValueNode[] valueResult = { new NumberNode("1", new StreamLocation()), new NumberNode("2", new StreamLocation()) };
            private int index = 0;
            // Constructor
            public RecognizeRangeTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeRange() { return RecognizeRange(); }
            // Mock dependent method(s)
            protected override SimpleValueNode RecognizeSimpleValue() { InputStream.Consume();  return valueResult[index++]; }
        }

        private class RecognizeSetTestHarness : ExpressionParser<string>
        {
            public static SimpleValueNode[] valueResult = { new StringNode("A", new StreamLocation()), new StringNode("B", new StreamLocation()) };
            private int index = 0;
            // Constructor
            public RecognizeSetTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public Node InvokeRecognizeSet() { return RecognizeSet(); }
            // Mock dependent method(s)
            protected override SimpleValueNode RecognizeSimpleValue() { InputStream.Consume(); return valueResult[index++]; }
        }

        private class TokenTestHarness : ExpressionParser<string>
        {
            // Constructor
            public TokenTestHarness(IList<Lexer.Token> testTokenSequence)
                : base(new ConfiguredTestLexer(testTokenSequence)) { }
            // Create visible entry points for tests
            public bool InvokeIsOR() { return IsOR(); }
            public bool InvokeIsAND() { return IsAND(); }
            public bool InvokeIsNOT() { return IsNOT(); }
            public bool InvokeIsKeyword() { return IsKeyword(); }
            public bool InvokeIsKeyword(string kw) { return IsKeyword(kw); }
            public bool InvokeIsIdentifier() { return IsIdentifier(); }
            public bool InvokeIsIdentifier(string x) { return IsIdentifier(x); }
            public bool InvokeIsSymbol() { return IsSymbol(); }
            public bool InvokeIsSymbol(string x) { return IsSymbol(x); }
            public bool InvokeIsNumber() { return IsNumber(); }
            public bool InvokeIsString() { return IsString(); }
            // Control
            public TokenTestHarness Next() { InputStream.Consume(); return this; }
        }

        private bool IsExpectedNode(Node expectedResult, Node result)
        {
            return (expectedResult.ToString().Equals(result.ToString()));
        }
        #endregion
    }
}
