﻿using System;

namespace PT.PM.Common.Nodes.Tokens.Literals
{
    public class StringLiteral : Literal
    {
        public override string TextValue => Text;

        public virtual string Text { get; set; }

        public int EscapeCharsLength { get; set; } = 1;

        public StringLiteral(string text, TextSpan textSpan)
            : base(textSpan)
        {
            Text = text;
        }

        public StringLiteral(string text)
        {
            Text = text;
        }

        public StringLiteral()
        {
        }

        public override int CompareTo(Ust other)
        {
            var baseCompareResult = base.CompareTo(other);
            if (baseCompareResult != 0)
            {
                return baseCompareResult;
            }

            return String.Compare(Text, ((StringLiteral)other).Text, StringComparison.Ordinal); // TODO: Regular expressions ???
        }

        public override string ToString() => $"\"{Text}\"";
    }
}