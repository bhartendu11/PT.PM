﻿using PT.PM.Common;
using PT.PM.Matching.Patterns;
using System.Collections.Generic;
using System.Linq;
using static PT.PM.Common.Language;

namespace PT.PM.Patterns.PatternsRepository
{
    public partial class DefaultPatternRepository
    {
        public IEnumerable<PatternRootUst> CreateJavaPatterns()
        {
            var patterns = new List<PatternRootUst>();

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "InadequateRsaPadding. Weak Encryption: Inadequate RSA Padding. ",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("getInstance"),
                        Target = new PatternMemberReferenceExpression
                        {
                            Name = new PatternIdToken("Cipher"),
                            Target = new PatternMemberReferenceExpression
                            {
                                Name = new PatternIdToken("crypto"),
                                Target = new PatternIdToken("javax")
                            }
                        }
                    },
                    Arguments = new PatternArgs(new PatternStringRegexLiteral("^RSA/NONE/NoPadding$"))
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "WeakCryptographicAlgorithm. Weak Encryption: Broken or Risky Cryptographic Algorithm" +
                    "https://cwe.mitre.org/data/definitions/327.html",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("getInstance"),
                        Target = new PatternMemberReferenceExpression
                        {
                            Name = new PatternIdToken("Cipher"),
                            Target = new PatternMemberReferenceExpression
                            {
                                Name = new PatternIdToken("crypto"),
                                Target = new PatternIdToken("javax")
                            }
                        }
                    },
                    Arguments = new PatternArgs(new PatternStringRegexLiteral(@"DES"))
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "OverlyBroadPath. Cookie Security: Overly Broad Path.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("setPath"),
                        Target = new PatternIdRegexToken(@"[cC]ookie")
                    },
                    Arguments = new PatternArgs(new PatternStringRegexLiteral { Regex = "^/?$" })
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "OverlyBroadDomain Cookie Security: Overly Broad Domain.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("setDomain"),
                        Target = new PatternIdRegexToken("[cC]ookie")
                    },
                    Arguments = new PatternArgs(new PatternStringRegexLiteral(@"^.?[a-zA-Z0-9\-]+\.[a-zA-Z0-9\-]+$"))
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "PoorSeeding.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("setSeed"),
                        Target = new PatternAnyExpression()
                    },
                    Arguments = new PatternArgs(new PatternIntRangeLiteral())
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "WeakCryptographicHash.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("getInstance"),
                        Target = new PatternIdToken("MessageDigest")
                    },
                    Arguments = new PatternArgs(new PatternStringRegexLiteral("MD5|SHA-1"))
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "AndroidPermissionCheck. Often Misused: Android Permission Check.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdRegexToken("^(checkCallingOrSelfPermission|checkCallingOrSelfUriPermission)$"),
                        Target = new PatternAnyExpression()
                    },
                    Arguments = new PatternArgs(new PatternMultipleExpressions())
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "AndroidHostnameVerificationDisabled. Insecure SSL: Android Hostname Verification Disabled.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternOr
                (
                    new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("ALLOW_ALL_HOSTNAME_VERIFIER"),
                        Target = new PatternIdToken("SSLSocketFactory")
                    },
                    new PatternObjectCreateExpression
                    {
                        Type = new PatternIdToken("AllowAllHostnameVerifier"),
                        Arguments = new PatternArgs(new PatternMultipleExpressions())
                    }
                )
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "SAXReaderExternalEntity",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs(new PatternNot(new PatternStringRegexLiteral())),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("read"),
                        Target = new PatternObjectCreateExpression
                        {
                            Type = new PatternIdToken("SAXReader"),
                            Arguments = new PatternArgs()
                        }
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "XmlExternalEntity",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs(new PatternNot(new PatternStringRegexLiteral())),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("parse"),
                        Target = new PatternObjectCreateExpression
                        {
                            Type = new PatternIdToken("XMLUtil"),
                            Arguments = new PatternArgs()
                        }
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "StickyBroadcast. Android Bad Practices: Sticky Broadcast.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs(new PatternAnyExpression()),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("sendStickyBroadcast"),
                        Target = new PatternAnyExpression()
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "SendStickyBroadcastAsUser. Android Bad Practices: Sticky Broadcast.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs
                    (
                        new PatternAnyExpression(),
                        new PatternAnyExpression()
                    ),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken("sendStickyBroadcastAsUser"),
                        Target = new PatternAnyExpression()
                    }
                }
            });

            // TODO: implement "createSocket"
            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "InsecureSSL. Insecure SSL: Android Socket.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs
                    (
                        new PatternAnyExpression(),
                        new PatternAnyExpression()
                    ),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken { Id = "getInsecure" },
                        Target = new PatternAnyExpression()
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "HardcodedSalt. Weak Cryptographic Hash: Hardcoded Salt.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs
                    (
                        new PatternAnyExpression(),
                        new PatternStringRegexLiteral()
                    ),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken { Id = "hash" },
                        Target = new PatternAnyExpression()
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "MissingReceiverPermission. The program sends a broadcast without specifying the receiver permission. " +
                            "Broadcasts sent without the receiver permission are accessible to any receiver. If these broadcasts contain sensitive data or reach a malicious receiver, the application may be compromised.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs(new PatternAnyExpression()),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken { Id = "sendBroadcast" },
                        Target = new PatternAnyExpression()
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "MissingBroadcasterPermission. The program registers a receiver without specifying the broadcaster permission. " +
                    "Receiver registered without the broadcaster permission will receive messages from any broadcaster. " +
                    "If these messages contain malicious data or come from a malicious broadcaster, the application may be compromised. " +
                    "Use this form: public abstract Intent registerReceiver (BroadcastReceiver receiver, IntentFilter filter, String broadcastPermission, Handler scheduler)",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternInvocationExpression
                {
                    Arguments = new PatternArgs
                    (
                        new PatternAnyExpression(),
                        new PatternAnyExpression()
                    ),
                    Target = new PatternMemberReferenceExpression
                    {
                        Name = new PatternIdToken { Id = "registerReceiver" },
                        Target = new PatternAnyExpression()
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "CookieNotSentOverSSL. Cookie Security: Cookie not Sent Over SSL.",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternStatements
                (
                    new PatternVarOrFieldDeclaration
                    {
                        Type = new PatternIdToken("Cookie"),
                        Right = new PatternAssignmentExpression
                        {
                            Left = new PatternVar("cookie"),
                            Right = new PatternObjectCreateExpression
                            {
                                Type = new PatternIdToken("Cookie"),
                                Arguments = new PatternArgs(new PatternMultipleExpressions())
                            },
                        }
                    },

                    new PatternNot
                    (
                        new PatternInvocationExpression
                        {
                            Arguments = new PatternArgs(new PatternBooleanLiteral(true)),
                            Target = new PatternMemberReferenceExpression
                            {
                                Name = new PatternIdToken("setSecure"),
                                Target = new PatternVar("cookie")
                            }
                        }
                    ),

                    new PatternInvocationExpression
                    {
                        Arguments = new PatternArgs(new PatternVar("cookie")),
                        Target = new PatternMemberReferenceExpression
                        {
                            Name = new PatternIdToken("addCookie"),
                            Target = new PatternAnyExpression()
                        }
                    }
                )
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "Use of NullPointerException Catch to Detect NULL Pointer Dereference",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternTryCatchStatement
                {
                    ExceptionTypes = new List<PatternBase> { new PatternIdToken("NullPointerException") },
                    IsCatchBodyEmpty = false
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "UsingCloneWithoutCloneable. Using clone method without implementing Clonable",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternAnd
                (
                    new PatternClassDeclaration
                    {
                        Body = new PatternExpressionInsideNode
                        {
                            Expression = new PatternMethodDeclaration
                            {
                                Name = new PatternIdToken("clone"),
                                AnyBody = true
                            }
                        }
                    },

                    new PatternNot
                    (
                        new PatternClassDeclaration
                        {
                            BaseTypes = new List<PatternBase>{ new PatternIdToken("Cloneable") }
                        }
                    )
                )
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "ExtendingSecurityManagerWithoutFinal. Class extending SecurityManager is not final",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternAnd
                (
                    new PatternClassDeclaration
                    {
                        BaseTypes = new List<PatternBase>
                        {
                            new PatternIdRegexToken("SecurityManager")
                        }
                    },

                    new PatternNot
                    (
                        new PatternClassDeclaration
                        {
                            Modifiers = new List<PatternBase>
                            {
                                new PatternIdRegexToken("final")
                            }
                        }
                    )
                )
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "ImproperValidationEmptyMethod. Improper Certificate Validation (Empty method)",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternClassDeclaration
                {
                    BaseTypes = new List<PatternBase>
                    {
                        new PatternIdRegexToken("X509TrustManager|SSLSocketFactory")
                    },
                    Body = new PatternExpressionInsideNode
                    {
                        Expression = new PatternMethodDeclaration(
                            Enumerable.Empty<PatternBase>().ToList(), new PatternIdRegexToken(".+"), false)
                    }
                }
            });

            patterns.Add(new PatternRootUst
            {
                Key = patternIdGenerator.NextId(),
                DebugInfo = "PoorLoggingPractice. Declare logger not static or final",
                Languages = new HashSet<Language>() { Java },
                Node = new PatternAnd
                (
                    new PatternVarOrFieldDeclaration
                    {
                        LocalVariable = false,
                        Modifiers = new List<PatternBase>(),
                        Type = new PatternIdRegexToken("[Ll]og"),
                        Name = new PatternIdRegexToken(".+")
                    },
                    new PatternNot
                    (
                        new PatternVarOrFieldDeclaration
                        {
                            LocalVariable = false,
                            Modifiers = new List<PatternBase>
                            {
                                new PatternIdRegexToken("static"),
                                new PatternIdRegexToken("final")
                            },
                            Type = new PatternIdRegexToken("[Ll]og"),
                            Name = new PatternIdRegexToken(".+")
                        }
                    )
                )
            });

            return patterns;
        }
    }
}
