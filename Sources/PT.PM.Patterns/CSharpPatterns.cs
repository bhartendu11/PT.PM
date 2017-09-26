﻿using PT.PM.Common;
using PT.PM.Matching.Patterns;
using System.Collections.Generic;
using static PT.PM.Common.Language;

namespace PT.PM.Patterns.PatternsRepository
{
    public partial class DefaultPatternRepository
    {
        public IEnumerable<PatternRootUst> CreateCSharpPatterns()
        {
            var patterns = new List<PatternRootUst>();

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "WeakCryptographicHash",
                Languages = new HashSet<Language>() { CSharp },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("Create"),
                        Target = new PatternMemberReferenceExpression
                        {
                            Name = new PatternIdToken("MD5"),
                            Target = new PatternMemberReferenceExpression
                            {
                                Name = new PatternIdToken("Cryptography"),
                                Target = new PatternMemberReferenceExpression
                                {
                                    Name = new PatternIdToken("Security"),
                                    Target = new PatternIdToken("System")
                                }
                            }
                        }
                    },
                    Arguments = new PatternArgs(new PatternMultipleExpressions())
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "Use of NullReferenceException Catch to Detect NULL Pointer Dereference",
                Languages = new HashSet<Language>() { CSharp },
                Node = new PatternTryCatchStatement
                {
                    ExceptionTypes = new List<PatternBase>
                    {
                        new PatternIdToken("NullReferenceException"),
                        new PatternIdToken("System.NullReferenceException")
                    },
                    IsCatchBodyEmpty = false
                }
            });

            return patterns;
        }
    }
}