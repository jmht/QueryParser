using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HigginsThomas.QueryParser.Core;

namespace HigginsThomas.QueryParser.Parser
{
    public enum TNode
    {
        CONJUNCTION,
        NEGATIVE,
        IDENTITY,
        RELATION,
        IDENTIFIER,
        NUMBER,
        STRING,
        SET,
        RANGE,
        DATE,
        TRUE
    };

    public interface Node
    {
        TNode NodeType { get; }
        StreamLocation Location { get; }
        Expression SubExpression { get; }
        string Description { get; }
        string ToString();
    }

    public interface ValueNode : Node
    {
        FieldValueType ValueType { get; }
    }

    public interface SimpleValueNode : ValueNode { }    // This is a marker interface to distinguish the simple values from the structured ones

    public class ConjunctionNode : Node {
        public enum Conjunction { AND, OR };

        public TNode NodeType { get { return TNode.CONJUNCTION; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }

        public ConjunctionNode(Conjunction type, Node left, Node right, StreamLocation location)
        {
            Location = location;
            SubExpression = CreateSubExpression(type, left, right, location);
            Description = String.Format("{0}[{1}]({2}, {3})", NodeType, type, left, right);
        }

        private static Expression CreateSubExpression(Conjunction type, Node left, Node right, StreamLocation location)
        {
            switch (type)
            {
                case Conjunction.AND:
                    return Expression.And(left.SubExpression, right.SubExpression);
                case Conjunction.OR:
                    return Expression.Or(left.SubExpression, right.SubExpression);
                default:
                    throw new ParseException(String.Format("Internal error: Invalid conjunction: {0}", type), location);
            }
        }
    }

    public class IdentityNode : Node {
        public TNode NodeType { get { return TNode.IDENTITY; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }

        public IdentityNode(Node child, StreamLocation location) {
            Location = location;
            SubExpression = CreateSubExpression(child);
            Description = String.Format("{0}({1})", NodeType, child);
        }

        private static Expression CreateSubExpression(Node child)
        {
            return child.SubExpression;
        }
    }

    public class NegationNode : Node {
        public TNode NodeType { get { return TNode.NEGATIVE; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }

        public NegationNode(Node child, StreamLocation location) {
            Location = location;
            SubExpression = CreateSubExpression(child);
            Description = String.Format("{0}({1})", NodeType, child);
        }

        private static Expression CreateSubExpression(Node child)
        {
            return Expression.Not(child.SubExpression);
        }
    }

    public class RelationNode : Node {
        public enum Operator { EQ, NE, LT, GT, LE, GE, IN, LIKE};

        public TNode NodeType { get { return TNode.RELATION; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }

        public RelationNode(Operator op, ValueNode left, ValueNode right, StreamLocation location) {
            Location = location;
            SubExpression = CreateSubExpression(op, left, right, location);
            Description = String.Format("{0}[{1}]({2}, {3})", NodeType, op, left, right);
        }

        private static Expression CreateSubExpression(Operator op, ValueNode left, ValueNode right, StreamLocation location)
        {
            FieldValueType resultType = FieldValueHelper.VerifyTypeCompatible(left.ValueType, right.ValueType, location);

            if (op == Operator.IN) return CreateInExpression(resultType, left, right, location);
            if (op == Operator.LIKE) return CreateLikeExpression(resultType, left, right, location);

            Expression leftCast = CastTo(resultType, left);
            Expression rightCast = CastTo(resultType, right);
            switch (op)
            {
                case Operator.EQ:
                    return Expression.Equal(leftCast, rightCast);
                case Operator.NE:
                    return Expression.NotEqual(leftCast, rightCast);
                case Operator.LT:
                    return Expression.LessThan(leftCast, rightCast);
                case Operator.GT:
                    return Expression.GreaterThan(leftCast, rightCast);
                case Operator.LE:
                    return Expression.LessThanOrEqual(leftCast, rightCast);
                case Operator.GE:
                    return Expression.GreaterThanOrEqual(leftCast, rightCast);
                default:
                    throw new ParseException(String.Format("Internal error: Invalid operator: {0}", op), location);
            }
        }

        private static Expression CreateInExpression(FieldValueType resultType, ValueNode left, ValueNode right, StreamLocation location)
        {
            if (!(left is SimpleValueNode)) throw new ParseException(String.Format("Left operand of 'in' must be an identifier or simple value: {0}", left), location);
            SimpleValueNode value = (SimpleValueNode)left;
            if (right is SetNode)
            {
                return CreateInSetExpression(resultType, value, (SetNode)right);
            }
            else if (right is RangeNode)
            {
                return CreateInRangeExpression(resultType, value, (RangeNode)right);
            }
            else
            {
                throw new ParseException(String.Format("Right operand of 'in' must be a set or range: {0}", right), location);
            }
        }

        private static Expression CreateInSetExpression(FieldValueType resultType, SimpleValueNode value, SetNode setNode)
        {
            ISet<SimpleValueNode> set = setNode.GetSet();
            Expression expr = Expression.Constant(false); // default (empty set) is false
            foreach (SimpleValueNode element in set)
            {
                Expression e = Expression.Equal(CastTo(resultType, value), CastTo(resultType, element));
                expr = Expression.Or(expr, e);
            }
            return expr;
        }

        private static Expression CreateInRangeExpression(FieldValueType resultType, SimpleValueNode value, RangeNode range)
        {
            Expression castValue = CastTo(resultType, value);
            Expression castLeft = CastTo(resultType, range.GetLeft());
            Expression castRight = CastTo(resultType, range.GetRight());
            Expression lower, upper;
            if (range.IsInclusiveLeft())
            {
                lower = Expression.GreaterThanOrEqual(castValue, castLeft);
            }
            else
            {
                lower = Expression.GreaterThan(castValue, castLeft);
            }
            if (range.IsInclusiveRight())
            {
                upper = Expression.LessThanOrEqual(castValue, castRight);
            }
            else
            {
                upper = Expression.LessThan(castValue, castRight);
            }
            return Expression.And(lower, upper);
        }

        private static Expression CreateLikeExpression(FieldValueType resultType, ValueNode left, ValueNode right, StreamLocation location)
        {
            throw new NotImplementedException();
        }

        private static Expression CastTo(FieldValueType targetType, ValueNode source)
        {
            if (targetType == source.ValueType) return source.SubExpression;
            if (targetType == FieldValueType.FLOAT) return Expression.Convert(source.SubExpression, typeof(double));
            throw new ParseException(String.Format("Internal Error: unsupported type conversion requested.  target: {0} source: {1}", targetType, source.ValueType), source.Location);
        }
    }

    public class IdentifierNode<T> : SimpleValueNode {
        public TNode NodeType { get { return TNode.IDENTIFIER; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public IdentifierNode(string identifier, Field<T> retriever, ParameterExpression arg, StreamLocation location) {
            Location = location;
            ValueType = retriever.Type;
            SubExpression = CreateSubExpression(retriever, arg);
            Description = String.Format("{0}[{1}]", NodeType, identifier);
        }

        private static Expression CreateSubExpression(Field<T> retriever, ParameterExpression arg)
        {
            Expression expr = Expression.Invoke(retriever.Getter, arg);
            switch (retriever.Type)
            {
                case FieldValueType.DATE:
                    expr = Expression.Convert(expr, typeof(DateTime?));
                    break;
                case FieldValueType.FLOAT:
                    expr = Expression.Convert(expr, typeof(double?));
                    break;
                case FieldValueType.INTEGER:
                    expr = Expression.Convert(expr, typeof(long?));
                    break;
                case FieldValueType.TEXT:
                    expr = Expression.Convert(expr, typeof(string));
                    break;
            }
            return expr;
        }
    }

    public class NumberNode : SimpleValueNode {
        public TNode NodeType { get { return TNode.NUMBER; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public NumberNode(string number, StreamLocation location) {
            Location = location;
            FieldValueType valueType;
            SubExpression = CreateSubExpression(number, location, out valueType);
            ValueType = valueType;
            Description = String.Format("{0}[{1}]", NodeType, number);
        }

        private static Expression CreateSubExpression(string number, StreamLocation location, out FieldValueType valueType)
        {
            try
            {
                if (number.Contains(".") || number.Contains("E") || number.Contains("e"))
                {
                    valueType = FieldValueType.FLOAT;
                    return Expression.Constant(double.Parse(number), typeof(double?));
                }
                else
                {
                    valueType = FieldValueType.INTEGER;
                    return Expression.Constant(long.Parse(number), typeof(long?));
                }
            }
            catch (Exception e)
            {
                throw new ParseException(String.Format("Invalid numeric constant: {0}", number), location, e);
            }
        }
    }

    public class StringNode : SimpleValueNode {
        public TNode NodeType { get { return TNode.STRING; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public StringNode(string s, StreamLocation location) {
            Location = location;
            SubExpression = CreateSubExpression(s);
            Description = String.Format("{0}[{1}]", NodeType, s);
            ValueType = FieldValueType.TEXT;
        }

        private static Expression CreateSubExpression(string s)
        {
            return Expression.Constant(s);
        }
    }

    public class SetNode : ValueNode {
        private ISet<SimpleValueNode> s;

        public TNode NodeType { get { return TNode.SET; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public SetNode(ISet<SimpleValueNode> s, StreamLocation location) {
            this.s = s;
            Location = location;
            SubExpression = null;
            ValueType = verifyTypes(s, location);
            Description = String.Format("{0}[{1}]", NodeType, s);
        }
        public ISet<SimpleValueNode> GetSet() { return s; }

        private static FieldValueType verifyTypes(ISet<SimpleValueNode> s, StreamLocation location)
        {
            FieldValueType valueType = FieldValueType.NONE;
            foreach ( SimpleValueNode node in s)
            {
                valueType = FieldValueHelper.VerifyTypeCompatible(valueType, node.ValueType, location);
            }
            return valueType;
        }
    }

    public class RangeNode : ValueNode {
        private bool inclusiveLeft;
        private bool inclusiveRight;
        private SimpleValueNode left;
        private SimpleValueNode right;

        public TNode NodeType { get { return TNode.RANGE; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public RangeNode(SimpleValueNode left, bool inclusiveLeft, SimpleValueNode right, bool inclusiveRight, StreamLocation location) {
            this.left = left;
            this.right = right;
            this.inclusiveLeft = inclusiveLeft;
            this.inclusiveRight = inclusiveRight;
            Location = location;
            SubExpression = null;
            ValueType = FieldValueHelper.VerifyTypeCompatible(left.ValueType, right.ValueType, location);
            Description = String.Format("{0}[{3}{1}:{2}{4}]", NodeType, left, right, inclusiveLeft ? '[' : '>', inclusiveRight ? ']' : '<');
        }
        public SimpleValueNode GetLeft() { return left; }
        public SimpleValueNode GetRight() { return right; }
        public bool IsInclusiveLeft() { return inclusiveLeft; }
        public bool IsInclusiveRight() { return inclusiveRight; }
    }

    public class DateNode : SimpleValueNode {
        public TNode NodeType { get { return TNode.DATE; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }
        public FieldValueType ValueType { get; private set; }

        public DateNode(string year, string month, string day, StreamLocation location) {
            Location = location;
            SubExpression = CreateSubExpression(year, month, day, location);
            Description = String.Format("{0}[{1}-{2}-{3}]", NodeType, year, month, day);
            ValueType = FieldValueType.DATE;
        }

        private static Expression CreateSubExpression(string year, string month, string day, StreamLocation location)
        {
            try
            {
                DateTime date = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                return Expression.Constant(date, typeof(DateTime?));
            }
            catch (Exception e)
            {
                throw new ParseException(String.Format("Invalid date constant: {0}-{1}-{2}", year, month, day), location, e);
            }
        }
    }

    public class TrueNode : Node
    {
        public TNode NodeType { get { return TNode.TRUE; } }
        public StreamLocation Location { get; private set; }
        public Expression SubExpression { get; private set; }
        public string Description { get; private set; }

        public TrueNode(StreamLocation location)
        {
            Location = location;
            SubExpression = CreateSubExpression();
            Description = String.Format("{0}", NodeType);
        }

        private static Expression CreateSubExpression()
        {
            return Expression.Constant(true);
        }
    }
}
