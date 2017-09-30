﻿using PT.PM.Common;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Expressions;

namespace PT.PM.Matching.Patterns
{
    public class PatternBinaryOperatorExpression : PatternBase
    {
        public PatternBase Left { get; set; }

        public PatternBinaryOperatorLiteral Operator { get; set; }

        public PatternBase Right { get; set; }

        public PatternBinaryOperatorExpression()
        {
        }

        public PatternBinaryOperatorExpression(PatternBase left, PatternBinaryOperatorLiteral op, PatternBase right,
            TextSpan textSpan = default(TextSpan))
            : base(textSpan)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override Ust[] GetChildren() => new Ust[] { Left, Operator, Right };

        public override string ToString() => $"{Left} {Operator} {Right}";

        public override MatchingContext Match(Ust ust, MatchingContext context)
        {
            MatchingContext newContext;

            if (ust is BinaryOperatorExpression binaryOperatorExpression)
            {
                newContext = Left.Match(binaryOperatorExpression.Left, context);
                if (!newContext.Success)
                {
                    return newContext;
                }

                newContext = Operator.Match(binaryOperatorExpression.Operator, newContext);
                if (!newContext.Success)
                {
                    return newContext;
                }

                newContext = Right.Match(binaryOperatorExpression.Right, newContext);
            }
            else
            {
                newContext = context.Fail();
            }

            return newContext.AddMatchIfSuccess(ust);
        }
    }
}
