﻿using System;
using MessagePack;
using PT.PM.Common.Files;
using PT.PM.Common.MessagePack;

namespace PT.PM.Common
{
    [MessagePackObject]
    [MessagePackFormatter(typeof(TextSpanFormatter))]
    public struct TextSpan: IEquatable<TextSpan>, IComparable<TextSpan>, IComparable
    {
        public static readonly TextSpan Zero = default;

        public TextSpan(int start, int length, CodeFile codeFile = null)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (start + length < start)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            Start = start;
            Length = length;
            CodeFile = codeFile;
        }

        public TextSpan(TextSpan textSpan)
        {
            Start = textSpan.Start;
            Length = textSpan.Length;
            CodeFile = textSpan.CodeFile;
        }

        [Key(0)]
        public int Start { get; }

        [Key(1)]
        public int Length { get; }

        [Key(2)]
        public CodeFile CodeFile { get; set; }

        [IgnoreMember]
        public int End => Start + Length;

        [IgnoreMember]
        public bool IsZero => Start == 0 && Length == 0 && CodeFile == null;

        public TextSpan Union(TextSpan span)
        {
            if (CodeFile != span.CodeFile)
            {
                if (IsZero || (CodeFile != null && span.CodeFile == null))
                {
                    return span;
                }

                return this;
            }

            if (IsZero)
            {
                return span;
            }

            if (span.IsZero)
            {
                return this;
            }

            int unionStart = Math.Min(Start, span.Start);
            int unionEnd = Math.Max(End, span.End);

            return FromBounds(unionStart, unionEnd, CodeFile);
        }

        public bool Includes(TextSpan span)
        {
            return CodeFile == span.CodeFile && span.Start >= Start && span.End <= End;
        }

        public TextSpan AddOffset(int offset)
        {
            return new TextSpan(Start + offset, Length, CodeFile);
        }

        public static TextSpan FromBounds(int start, int end, CodeFile codeFile = null)
        {
            return new TextSpan(start, end - start, codeFile);
        }

        public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

        public static bool operator !=(TextSpan left, TextSpan right) => !left.Equals(right);

        public override bool Equals(object obj)
        {
            return obj is TextSpan && Equals((TextSpan)obj);
        }

        public bool EqualsNoFile(TextSpan other)
        {
            return Start == other.Start && Length == other.Length;
        }

        public bool Equals(TextSpan other)
        {
            if (CodeFile != other.CodeFile)
            {
                return false;
            }

            return Start == other.Start && Length == other.Length;
        }

        public override int GetHashCode()
        {
            int result = (Start << 16) + Length;

            if (!(CodeFile is null))
            {
                result = Hash.Combine(CodeFile.GetHashCode(), result);
            }

            return result;
        }

        public override string ToString() => ToString(true);

        public string ToString(bool includeFileName)
        {
            string result = Start == End
                ? $"[{Start})"
                : $"[{Start}..{End})";

            if (includeFileName && !(CodeFile is null))
            {
                result = $"{result}; {CodeFile}";
            }

            return result;
        }

        public static bool operator <(TextSpan textSpan1, TextSpan textSpan2) => textSpan1.CompareTo(textSpan2) < 0;
        
        public static bool operator <=(TextSpan textSpan1, TextSpan textSpan2) => textSpan1.CompareTo(textSpan2) <= 0;
        
        public static bool operator >(TextSpan textSpan1, TextSpan textSpan2) => textSpan1.CompareTo(textSpan2) > 0;
        
        public static bool operator >=(TextSpan textSpan1, TextSpan textSpan2) => textSpan1.CompareTo(textSpan2) >= 0;

        public int CompareTo(object obj)
        {
            if (obj is TextSpan otherTextSpan)
            {
                return CompareTo(otherTextSpan);
            }

            return 1;
        }

        public int CompareTo(TextSpan other)
        {
            if (CodeFile != other.CodeFile)
            {
                return CodeFile != null
                    ? CodeFile.CompareTo(other.CodeFile)
                    : 1;
            }

            int diff = Start - other.Start;
            if (diff != 0)
            {
                return diff;
            }

            return Length - other.Length;
        }
    }
}
