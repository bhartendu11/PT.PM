﻿using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PT.PM.Common;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Tokens.Literals;

namespace PT.PM.Matching.Patterns
{
    public class PatternAny : PatternUst, IRegexPattern, ITerminalPattern
    {
        [JsonIgnore]
        public string Default => ".*";

        public string RegexString
        {
            get => Regex?.ToString() ?? "";
            set => Regex = value == null
                ? null
                : new Regex(string.IsNullOrEmpty(value) ? Default : value, RegexOptions.Compiled);
        }

        public Regex Regex { get; private set; }

        [JsonIgnore]
        public override bool Any => Regex == null || RegexString.Equals(Default);

        public bool UseUstString { get; set; }

        public PatternAny()
        {
        }

        public PatternAny(string regex, TextSpan textSpan = default)
            : base(textSpan)
        {
            RegexString = regex;
        }

        protected override MatchContext Match(Ust ust, MatchContext context)
        {
            if (Any)
            {
                return ust == null ? context.MakeSuccess() : context.AddMatch(ust);
            }

            int escapeCharsLength = (ust as StringLiteral)?.EscapeCharsLength ?? 0;
            var matches = UseUstString || ust.TextSpan.Start > ust.CurrentSourceFile.Data.Length
                ? Regex.MatchRegex(ust.ToString(), escapeCharsLength, ust.TextSpan.Start)
                : Regex.MatchRegex(ust.CurrentSourceFile, ust.TextSpan, escapeCharsLength);

            return matches.Count > 0
                ? context.AddMatches(matches)
                : context.Fail();
        }

        public override string ToString()
        {
            return Any ? "<#>" : $@"<#{RegexString}#>";
        }
    }
}
