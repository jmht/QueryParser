using System;
using HigginsThomas.QueryParser.Core;
using System.Linq.Expressions;


namespace HigginsThomas.QueryParser.Parser
{
    public enum FieldValueType
    {
        NONE,
        TEXT,
        INTEGER,
        FLOAT,
        DATE
    };

    public static class FieldValueHelper
    {
        public static FieldValueType VerifyTypeCompatible(FieldValueType a, FieldValueType b, StreamLocation location)
        {
            if (a == FieldValueType.NONE) return b;
            if (a == b || b == FieldValueType.NONE) return a;
            if (a == FieldValueType.INTEGER && b == FieldValueType.FLOAT) return FieldValueType.FLOAT;
            if (a == FieldValueType.FLOAT && b == FieldValueType.INTEGER) return FieldValueType.FLOAT;
            throw new ParseException(String.Format("Type Mismatch: {0} is not compatible with {1}", a, b), location);
        }
    }

    public abstract class Field<T>
    {
        public FieldValueType Type { get; protected set; }
        public Expression Getter { get; protected set; }
    }

    public class IntegerField<T> : Field<T>
    {
        public IntegerField(Expression<Func<T, long?>> getter) 
        {
            Type = FieldValueType.INTEGER;
            Getter = getter;  
        }
    }

    public class FloatField<T> : Field<T>
    {
        public FloatField(Expression<Func<T, double?>> getter) 
        {
            Type = FieldValueType.FLOAT;
            Getter = getter;  
        }
    }

    public class TextField<T> : Field<T>
    {
        public TextField(Expression<Func<T, string>> getter) 
        {
            Type = FieldValueType.TEXT;
            Getter = getter;  
        }
    }

    public class DateField<T> : Field<T>
    {
        public DateField(Expression<Func<T, DateTime?>> getter) 
        {
            Type = FieldValueType.DATE;
            Getter = getter;  
        }
    }
}
