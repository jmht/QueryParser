using System.Linq.Expressions;
using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace HigginsThomas.QueryParser.UnitTests
{
    [TestClass]
    public class ExpressionParseTreeTest
    {
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ConjunctionNodeTest_OR()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression expectedLeft = Expression.Constant(false);
            Expression expectedRight = Expression.Constant(true);

            ConjunctionNode sut = new ConjunctionNode(ConjunctionNode.Conjunction.OR, new TestNode(expectedLeft), new TestNode(expectedRight), expectedLocation);

            Assert.AreEqual(TNode.CONJUNCTION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("OR"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Or, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(expectedLeft, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(expectedRight, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void ConjunctionNodeTest_AND()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression expectedLeft = Expression.Constant(false);
            Expression expectedRight = Expression.Constant(true);

            ConjunctionNode sut = new ConjunctionNode(ConjunctionNode.Conjunction.AND, new TestNode(expectedLeft), new TestNode(expectedRight), expectedLocation);

            Assert.AreEqual(TNode.CONJUNCTION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("AND"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.And, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(expectedLeft, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(expectedRight, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void IdentityNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression expectedExpr = Expression.Constant(43L, typeof(long));

            IdentityNode sut = new IdentityNode(new TestNode(expectedExpr), expectedLocation);

            Assert.AreEqual(TNode.IDENTITY, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("IDENTITY"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(long), sut.SubExpression.Type);
            Assert.AreEqual(expectedExpr, sut.SubExpression);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NegationNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression expectedExpr = Expression.Constant(true);

            NegationNode sut = new NegationNode(new TestNode(expectedExpr), expectedLocation);

            Assert.AreEqual(TNode.NEGATIVE, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("NEGATIVE"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(UnaryExpression));
            Assert.AreEqual(ExpressionType.Not, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(expectedExpr, ((UnaryExpression)sut.SubExpression).Operand);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_EQ()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.EQ, 
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER), 
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER), 
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Equal, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_NE()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.NE,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER), 
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER), 
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.NotEqual, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_LT()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.LT,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.LessThan, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_GT()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.GT,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.GreaterThan, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_LE()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.LE,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.LessThanOrEqual, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_GE()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(34L);

            RelationNode sut = new RelationNode(RelationNode.Operator.GE,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.INTEGER),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.GreaterThanOrEqual, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(operandA, ((BinaryExpression)sut.SubExpression).Left);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_IN_Range_Inclusive()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L, typeof(long?));
            SimpleValueNode rangeLow = new NumberNode("34", new StreamLocation());
            SimpleValueNode rangeHigh = new NumberNode("68", new StreamLocation());

            RelationNode sut = new RelationNode(RelationNode.Operator.IN, 
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER), 
                                                new RangeNode(rangeLow, true, rangeHigh, true, new StreamLocation()), 
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.And, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            {
                Expression left = ((BinaryExpression)sut.SubExpression).Left;
                Assert.IsInstanceOfType(left, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.GreaterThanOrEqual, left.NodeType);
                Assert.AreEqual(operandA, ((BinaryExpression)left).Left);
                Assert.AreEqual(rangeLow.SubExpression, ((BinaryExpression)left).Right);
            }
            {
                Expression right = ((BinaryExpression)sut.SubExpression).Right;
                Assert.IsInstanceOfType(right, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.LessThanOrEqual, right.NodeType);
                Assert.AreEqual(operandA, ((BinaryExpression)right).Left);
                Assert.AreEqual(rangeHigh.SubExpression, ((BinaryExpression)right).Right);
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_IN_Range_Exclusive()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L, typeof(long?));
            SimpleValueNode rangeLow = new NumberNode("34", new StreamLocation());
            SimpleValueNode rangeHigh = new NumberNode("68", new StreamLocation());

            RelationNode sut = new RelationNode(RelationNode.Operator.IN,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new RangeNode(rangeLow, false, rangeHigh, false, new StreamLocation()),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.And, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            {
                Expression left = ((BinaryExpression)sut.SubExpression).Left;
                Assert.IsInstanceOfType(left, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.GreaterThan, left.NodeType);
                Assert.AreEqual(operandA, ((BinaryExpression)left).Left);
                Assert.AreEqual(rangeLow.SubExpression, ((BinaryExpression)left).Right);
            }
            {
                Expression right = ((BinaryExpression)sut.SubExpression).Right;
                Assert.IsInstanceOfType(right, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.LessThan, right.NodeType);
                Assert.AreEqual(operandA, ((BinaryExpression)right).Left);
                Assert.AreEqual(rangeHigh.SubExpression, ((BinaryExpression)right).Right);
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_IN_Set()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L, typeof(long?));
            List<ConstantExpression> testExprs = new List<ConstantExpression>() { Expression.Constant(34L, typeof(long?)),
                                                                                  Expression.Constant(64L, typeof(long?)) };
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>();
            foreach (ConstantExpression expr in testExprs)
            {
                testSet.Add(new SimpleValueTestNode(expr, FieldValueType.INTEGER));
            }

            RelationNode sut = new RelationNode(RelationNode.Operator.IN,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SetNode(testSet, new StreamLocation()), 
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Or, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            List<ConstantExpression> resultExprs = new List<ConstantExpression>();
            {
                Expression left = ((BinaryExpression)sut.SubExpression).Left;
                Assert.IsInstanceOfType(left, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.Or, sut.SubExpression.NodeType);
                Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
                {
                    Expression leftLeft = ((BinaryExpression)left).Left;
                    Assert.IsInstanceOfType(leftLeft, typeof(ConstantExpression));
                    Assert.AreEqual(ExpressionType.Constant, leftLeft.NodeType);
                    Assert.AreEqual(typeof(bool), leftLeft.Type);
                    Assert.AreEqual(false, ((ConstantExpression)leftLeft).Value);
                }
                {
                    Expression leftRight = ((BinaryExpression)left).Right;
                    Assert.IsInstanceOfType(leftRight, typeof(BinaryExpression));
                    Assert.AreEqual(ExpressionType.Equal, leftRight.NodeType);
                    Assert.AreEqual(operandA, ((BinaryExpression)leftRight).Left);
                    resultExprs.Add((ConstantExpression)((BinaryExpression)leftRight).Right);
                }
            }
            {
                Expression right = ((BinaryExpression)sut.SubExpression).Right;
                Assert.IsInstanceOfType(right, typeof(BinaryExpression));
                Assert.AreEqual(ExpressionType.Equal, right.NodeType);
                Assert.AreEqual(operandA, ((BinaryExpression)right).Left);
                resultExprs.Add((ConstantExpression)((BinaryExpression)right).Right);
            }
            CollectionAssert.AreEquivalent(testExprs, resultExprs);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_IN_EmptySet()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L, typeof(long?));
            List<ConstantExpression> testExprs = new List<ConstantExpression>();
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>();
            foreach (ConstantExpression expr in testExprs)
            {
                testSet.Add(new SimpleValueTestNode(expr, FieldValueType.INTEGER));
            }

            RelationNode sut = new RelationNode(RelationNode.Operator.IN,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SetNode(testSet, new StreamLocation()),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.AreEqual(false, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RelationNodeTest_TypeCoercion()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant(3.4D);

            RelationNode sut = new RelationNode(RelationNode.Operator.EQ,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.FLOAT),
                                                expectedLocation);

            Assert.AreEqual(TNode.RELATION, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RELATION"));
            Assert.IsInstanceOfType(sut.SubExpression, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Equal, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(bool), sut.SubExpression.Type);
            Assert.IsInstanceOfType(((BinaryExpression)sut.SubExpression).Left, typeof(UnaryExpression));
            UnaryExpression cast = (UnaryExpression)((BinaryExpression)sut.SubExpression).Left;
            Assert.AreEqual(ExpressionType.Convert, cast.NodeType);
            Assert.AreEqual(operandA, cast.Operand);
            Assert.AreEqual(operandB, ((BinaryExpression)sut.SubExpression).Right);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RelationNodeTest_TypeMismatch()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Expression operandA = Expression.Constant(43L);
            Expression operandB = Expression.Constant("34");

            RelationNode sut = new RelationNode(RelationNode.Operator.EQ,
                                                new SimpleValueTestNode(operandA, FieldValueType.INTEGER),
                                                new SimpleValueTestNode(operandB, FieldValueType.TEXT),
                                                expectedLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void IdentifierNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            Field<string> retriever = new IntegerField<string>(s => int.Parse(s));
            ParameterExpression arg = Expression.Parameter(typeof(string), "arg");

            ValueNode sut = new IdentifierNode<string>("identifier", retriever, arg, expectedLocation);

            Assert.AreEqual(TNode.IDENTIFIER, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("IDENTIFIER"));
            Assert.AreEqual(FieldValueType.INTEGER, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(UnaryExpression));
            Assert.AreEqual(ExpressionType.Convert, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(long?), sut.SubExpression.Type);
            {
                Expression invokeExpr = ((UnaryExpression)sut.SubExpression).Operand;
                Assert.IsInstanceOfType(invokeExpr, typeof(InvocationExpression));
                Assert.AreEqual(ExpressionType.Invoke, invokeExpr.NodeType);
                Assert.AreEqual(arg, ((InvocationExpression)invokeExpr).Arguments[0]);
            }
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NumberNodeTest_Integer()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testNumber = "34";
            long? expectedValue = (long?)long.Parse(testNumber);

            ValueNode sut = new NumberNode(testNumber, expectedLocation);

            Assert.AreEqual(TNode.NUMBER, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("NUMBER"));
            Assert.AreEqual(FieldValueType.INTEGER, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(long?), sut.SubExpression.Type);
            Assert.AreEqual(expectedValue, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NumberNodeTest_Float_Decimal()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testNumber = "34.";
            double? expectedValue = (double?)double.Parse(testNumber);

            ValueNode sut = new NumberNode(testNumber, expectedLocation);

            Assert.AreEqual(TNode.NUMBER, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("NUMBER"));
            Assert.AreEqual(FieldValueType.FLOAT, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(double?), sut.SubExpression.Type);
            Assert.AreEqual(expectedValue, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NumberNodeTest_Float_ExponentLC()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testNumber = "34e-1";
            double? expectedValue = (double?)double.Parse(testNumber);

            ValueNode sut = new NumberNode(testNumber, expectedLocation);

            Assert.AreEqual(TNode.NUMBER, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("NUMBER"));
            Assert.AreEqual(FieldValueType.FLOAT, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(double?), sut.SubExpression.Type);
            Assert.AreEqual(expectedValue, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NumberNodeTest_Float_ExponentUC()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testNumber = "34E+3";
            double? expectedValue = (double?)double.Parse(testNumber);

            ValueNode sut = new NumberNode(testNumber, expectedLocation);

            Assert.AreEqual(TNode.NUMBER, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("NUMBER"));
            Assert.AreEqual(FieldValueType.FLOAT, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(double?), sut.SubExpression.Type);
            Assert.AreEqual(expectedValue, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void NumberNodeTest_FormatError()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testNumber = "34..0";

            ValueNode sut = new NumberNode(testNumber, expectedLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void StringNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            string testString = "hello world";

            ValueNode sut = new StringNode(testString, expectedLocation);

            Assert.AreEqual(TNode.STRING, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("STRING"));
            Assert.AreEqual(FieldValueType.TEXT, sut.ValueType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.AreEqual(typeof(string), sut.SubExpression.Type);
            Assert.AreEqual(testString, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void SetNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>()
                { 
                    new StringNode("a", new StreamLocation()),
                    new StringNode("b", new StreamLocation()),
                    new StringNode("c", new StreamLocation()),
                };

            ValueNode sut = new SetNode(testSet, expectedLocation);

            Assert.AreEqual(TNode.SET, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("SET"));
            Assert.AreEqual(FieldValueType.TEXT, sut.ValueType);
            Assert.IsNull(sut.SubExpression);
            Assert.AreEqual(testSet, ((SetNode)sut).GetSet());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void SetNodeTest_Empty()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>();

            ValueNode sut = new SetNode(testSet, expectedLocation);

            Assert.AreEqual(TNode.SET, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("SET"));
            Assert.AreEqual(FieldValueType.NONE, sut.ValueType);
            Assert.IsNull(sut.SubExpression);
            Assert.AreEqual(testSet, ((SetNode)sut).GetSet());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void SetNodeTest_MixedCompatibleTypes()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>()
                { 
                    new NumberNode("34", new StreamLocation()),
                    new NumberNode("3.4", new StreamLocation()),
                    new NumberNode("34e-2", new StreamLocation()),
                };

            ValueNode sut = new SetNode(testSet, expectedLocation);

            Assert.AreEqual(TNode.SET, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("SET"));
            Assert.AreEqual(FieldValueType.FLOAT, sut.ValueType);
            Assert.IsNull(sut.SubExpression);
            Assert.AreEqual(testSet, ((SetNode)sut).GetSet());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void SetNodeTest_MixedIncompatibleTypes()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            ISet<SimpleValueNode> testSet = new HashSet<SimpleValueNode>()
                { 
                    new NumberNode("34", new StreamLocation()),
                    new StringNode("3.4", new StreamLocation()),
                    new NumberNode("34e-2", new StreamLocation()),
                };

            ValueNode sut = new SetNode(testSet, expectedLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RangeNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            SimpleValueNode testLowBound = new NumberNode("1", new StreamLocation());
            SimpleValueNode testUpperBound = new NumberNode("10", new StreamLocation());
            bool testInclusiveLeft = true;
            bool testInclusiveRight = false;

            RangeNode sut = new RangeNode(testLowBound, testInclusiveLeft, testUpperBound, testInclusiveRight, expectedLocation);

            Assert.AreEqual(TNode.RANGE, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RANGE"));
            Assert.AreEqual(FieldValueType.INTEGER, sut.ValueType);
            Assert.IsNull(sut.SubExpression);
            Assert.AreEqual(testLowBound, sut.GetLeft());
            Assert.AreEqual(testInclusiveLeft, sut.IsInclusiveLeft());
            Assert.AreEqual(testUpperBound, sut.GetRight());
            Assert.AreEqual(testInclusiveRight, sut.IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void RangeNodeTest_CoercedTypes()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            SimpleValueNode testLowBound = new NumberNode("1", new StreamLocation());
            SimpleValueNode testUpperBound = new NumberNode("10.", new StreamLocation());
            bool testInclusiveLeft = false;
            bool testInclusiveRight = true;

            RangeNode sut = new RangeNode(testLowBound, testInclusiveLeft, testUpperBound, testInclusiveRight, expectedLocation);

            Assert.AreEqual(TNode.RANGE, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("RANGE"));
            Assert.AreEqual(FieldValueType.FLOAT, sut.ValueType);
            Assert.IsNull(sut.SubExpression);
            Assert.AreEqual(testLowBound, sut.GetLeft());
            Assert.AreEqual(testInclusiveLeft, sut.IsInclusiveLeft());
            Assert.AreEqual(testUpperBound, sut.GetRight());
            Assert.AreEqual(testInclusiveRight, sut.IsInclusiveRight());
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        [ExpectedException(typeof(ParseException))]
        public void RangeNodeTest_IncompatibleTypes()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            SimpleValueNode testLowBound = new NumberNode("1", new StreamLocation());
            SimpleValueNode testUpperBound = new StringNode("10", new StreamLocation());
            bool testInclusiveLeft = true;
            bool testInclusiveRight = true;

            RangeNode sut = new RangeNode(testLowBound, testInclusiveLeft, testUpperBound, testInclusiveRight, expectedLocation);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void DateNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);
            DateTime testDate = new DateTime(2013, 10, 3);

            DateNode sut = new DateNode(testDate.Year.ToString(), testDate.Month.ToString(), testDate.Day.ToString(), expectedLocation);

            Assert.AreEqual(TNode.DATE, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("DATE"));
            Assert.AreEqual(FieldValueType.DATE, sut.ValueType);

            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(typeof(DateTime?), ((ConstantExpression)sut.SubExpression).Type);
            Assert.AreEqual(testDate, ((ConstantExpression)sut.SubExpression).Value);
        }

        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void TrueNodeTest()
        {
            StreamLocation expectedLocation = new StreamLocation(3, 2, 1);

            TrueNode sut = new TrueNode(expectedLocation);

            Assert.AreEqual(TNode.TRUE, sut.NodeType);
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.IsTrue(sut.Description.Contains("TRUE"));

            Assert.AreEqual(ExpressionType.Constant, sut.SubExpression.NodeType);
            Assert.IsInstanceOfType(sut.SubExpression, typeof(ConstantExpression));
            Assert.AreEqual(typeof(bool), ((ConstantExpression)sut.SubExpression).Type);
            Assert.AreEqual(true, ((ConstantExpression)sut.SubExpression).Value);
        }


        #region Helpers
        private class TestNode : Node
        {
            public TNode NodeType { get { return TNode.TRUE; } }
            public StreamLocation Location { get { return new StreamLocation(); } }
            public Expression SubExpression { get; private set; }
            public string Description { get { return "Test Node"; } }

            public TestNode(Expression expr)
            {
                SubExpression = expr;
            }
        }

        private class SimpleValueTestNode : TestNode, SimpleValueNode
        {
            public FieldValueType ValueType { get; private set; }
            public SimpleValueTestNode(Expression expr, FieldValueType type)
                : base(expr)
            {
                ValueType = type;
            }
        }
        #endregion
    }
}
