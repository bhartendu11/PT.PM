﻿using Newtonsoft.Json;
using PT.PM.Common;
using PT.PM.Common.Nodes;
using PT.PM.Matching.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PT.PM.Common.Files;

namespace PT.PM.Matching
{
    public class PatternRoot : PatternUst, ILoggable
    {
        private HashSet<Language> languages = new HashSet<Language>();
        private Regex pathWildcardRegex;

        public static readonly string[] KeySeparators = { ",", ";" };

        public static string[] ExtractKeys(string keys)
        {
            return keys.Split(KeySeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        public ILogger Logger { get; set; } = DummyLogger.Instance;

        public string Key { get; set; } = "";

        public string FilenameWildcard { get; set; } = "";

        public Regex FilenameWildcardRegex
        {
            get
            {
                if (!string.IsNullOrEmpty(FilenameWildcard) && pathWildcardRegex == null)
                {
                    pathWildcardRegex = new WildcardConverter().Convert(FilenameWildcard);
                }
                return pathWildcardRegex;
            }
        }

        public HashSet<Language> Languages
        {
            get => languages;
            set
            {
                Language notPatternLang = value.FirstOrDefault(lang => !LanguageUtils.LanguageInfos[lang].IsPattern);
                if (notPatternLang != Language.Uncertain)
                {
                    throw new ArgumentException($"Unable to create pattern for {LanguageUtils.LanguageInfos[notPatternLang].Title}");
                }
                languages = value;
            }
        }

        public string DataFormat { get; set; } = "";

        [JsonIgnore]
        public string DebugInfo { get; set; } = "";

        public PatternUst Node { get; set; } = new PatternAny();

        public TextFile File { get; set; } = TextFile.Empty;

        public PatternRoot()
        {
        }

        public PatternRoot(PatternUst node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public override string ToString()
        {
            return (!string.IsNullOrEmpty(DebugInfo) ? DebugInfo : Key) ?? "";
        }

        public void Match(Ust ust, UstConstantFolder ustConstantFolder)
        {
            var context = new MatchContext(this, ustConstantFolder) { Logger = Logger };

            if (ust is RootUst rootUst)
            {
                if (Node is PatternCommentRegex ||
                    Node is PatternOr patternOr && patternOr.Patterns.Any(v => v is PatternCommentRegex))
                {
                    foreach (Comment commentLiteral in rootUst.Comments)
                    {
                        MatchAndAddResult(Node, commentLiteral, context);
                    }
                }
                else
                {
                    TraverseChildren(Node, rootUst, context);
                }
            }
        }

        protected override MatchContext Match(Ust ust, MatchContext context)
        {
            MatchAndAddResult(this, ust, context);
            return context;
        }

        private static void TraverseChildren(PatternUst patternUst, Ust ust, MatchContext context)
        {
            MatchAndAddResult(patternUst, ust, context);

            if (ust != null && !(patternUst is PatternAny) && !context.MatchedWithFolded)
            {
                foreach (Ust child in ust.Children)
                {
                    TraverseChildren(patternUst, child, context);
                }
            }

            context.MatchedWithFolded = false;
        }

        private static void MatchAndAddResult(PatternUst patternUst, Ust ust, MatchContext context)
        {
            if (patternUst.MatchUst(ust, context).Success)
            {
                if (context.Locations.Count == 0)
                {
                    context.Locations.Add(ust.TextSpan);
                }

                //TODO: handle Few matches not only in Comments, also in strings
                if(context.Locations.Count > 1
                    && patternUst is PatternCommentRegex)
                {
                    foreach(var location in context.Locations)
                    {
                        var match = new MatchResult(ust,
                            context.PatternUst,
                            new List<TextSpan>(1) { location });
                        context.Logger.LogInfo(match);
                    }
                }
                else
                {
                    var match = new MatchResult(ust, context.PatternUst, context.Locations);
                    context.Logger.LogInfo(match);
                }
            }

            context.Locations.Clear();
        }
    }
}
