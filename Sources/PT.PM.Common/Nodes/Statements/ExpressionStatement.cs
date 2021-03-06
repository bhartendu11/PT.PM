﻿using MessagePack;
using PT.PM.Common.Nodes.Expressions;

namespace PT.PM.Common.Nodes.Statements
{
    [MessagePackObject]
    public class ExpressionStatement : Statement
    {
        [Key(0)] public override UstType UstType => UstType.ExpressionStatement;

        [Key(UstFieldOffset)]
        public Expression Expression { get; set; }

        public ExpressionStatement()
        {
        }

        public ExpressionStatement(Expression expression, TextSpan textSpan = default)
            : base(textSpan.IsZero ? expression.TextSpan : textSpan)
        {
            Expression = expression;
        }

        public override Ust[] GetChildren() => new Ust[] { Expression };

        public override string ToString()
        {
            return $"{Expression};";
        }
    }
}
