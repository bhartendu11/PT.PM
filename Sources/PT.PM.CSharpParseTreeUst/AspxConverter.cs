﻿using AspxParser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PT.PM.Common;
using PT.PM.Common.Exceptions;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Collections;
using PT.PM.CSharpParseTreeUst.RoslynUstVisitor;
using System;
using System.Collections.Generic;
using System.Threading;
using PT.PM.Common.Files;

namespace PT.PM.CSharpParseTreeUst
{
    public class AspxConverter : DepthFirstAspxWithoutCloseTagVisitor<Ust>, IParseTreeToUstConverter
    {
        private Stack<bool> runAtServer = new Stack<bool>();
        private TextFile sourceFile;

        public Language Language => Language.Aspx;

        public HashSet<Language> AnalyzedLanguages { get; set; }

        public ILogger Logger { get; set; } = DummyLogger.Instance;

        public RootUst ParentRoot { get; set; }

        public static AspxConverter Create() => new AspxConverter();

        public AspxConverter()
        {
            AnalyzedLanguages = Language.GetSelfAndSublanguages();
            runAtServer.Push(false);
        }

        public RootUst Convert(ParseTree langParseTree)
        {
            var aspxParseTree = (AspxParseTree)langParseTree;
            try
            {
                RootUst result;
                sourceFile = langParseTree.SourceFile;
                Ust visited = aspxParseTree.Root.Accept(this);
                if (visited is RootUst rootUst)
                {
                    result = rootUst;
                }
                else
                {
                    result = new RootUst(sourceFile, Language);
                    result.Node = visited;
                }
                result.FillParentAndRootForDescendantsAndSelf();

                return result;
            }
            catch (Exception ex) when (!(ex is ThreadAbortException))
            {
                Logger.LogError(new ConversionException(aspxParseTree.SourceFile, ex));
                return null;
            }
        }

        public override Ust Visit(AspxNode.Root node)
        {
            var members = new List<Ust>();
            foreach (var child in node.Children)
            {
                var accepted = child.Accept(this);
                if (accepted != null)
                {
                    members.Add(accepted);
                }
            }

            if (members.Count == 1)
            {
                return members[0];
            }

            return new Collection(members);
        }

        public override Ust Visit(AspxNode.HtmlTag node)
        {
            runAtServer.Push(node.Attributes.IsRunAtServer);

            var members = new List<Ust>();
            foreach (var child in node.Children)
            {
                Ust accepted = child.Accept(this);
                if (accepted != null)
                {
                    members.Add(accepted);
                }
            }

            runAtServer.Pop();

            if (members.Count == 1)
            {
                return members[0];
            }

            return new Collection(members);
        }

        public override Ust Visit(AspxNode.AspxTag node)
        {
            runAtServer.Push(node.Attributes.IsRunAtServer);
            var result = VisitChildren(node);
            runAtServer.Pop();
            return result;
        }

        public override Ust Visit(AspxNode.AspxDirective node)
        {
            // TODO: implement AspxDirective processing.
            runAtServer.Push(node.Attributes.IsRunAtServer);
            var result = VisitChildren(node);
            runAtServer.Pop();
            return result;
        }

        public override Ust Visit(AspxNode.DataBinding node)
        {
            return VisitChildren(node);
        }

        public override Ust Visit(AspxNode.CodeRender node)
        {
            return ParseAndConvert(node.Expression, node.Location);
        }

        public override Ust Visit(AspxNode.CodeRenderExpression node)
        {
            string wrappedExpression = ResponseWriteWrappedString(node.Expression);
            return ParseExpressionAndConvert(wrappedExpression, node.Location.Start);
        }

        public override Ust Visit(AspxNode.CodeRenderEncode node)
        {
            string wrappedExpression = HtmlEncodeWrappedString(node.Expression);
            return ParseExpressionAndConvert(wrappedExpression, node.Location.Start);
        }

        public override Ust Visit(AspxNode.Literal node)
        {
            Ust result = null;
            if (!string.IsNullOrWhiteSpace(node.Text) && runAtServer.Peek())
            {
                result = ParseAndConvert(node.Text, node.Location);
            }
            return result;
        }

        private Ust ParseAndConvert(string code, global::AspxParser.Location location)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(kind: SourceCodeKind.Script));
            var converter = new CSharpRoslynParseTreeConverter();
            RootUst result = converter.Convert(new CSharpRoslynParseTree(tree) { SourceFile = sourceFile });
            if (result != null)
            {
                result.ApplyActionToDescendantsAndSelf(ust => ust.TextSpan = ust.TextSpan.AddOffset(location.Start));
            }
            return result;
        }

        private Ust ParseExpressionAndConvert(string expression, int offset)
        {
            ExpressionSyntax node = SyntaxFactory.ParseExpression(expression);
            var converter = new CSharpRoslynParseTreeConverter();
            Ust result = converter.Visit(node);
            RootUst resultRoot =
                result as RootUst ?? new RootUst(sourceFile, Language.CSharp, result.TextSpan) { Node = result };
            resultRoot.SourceFile = sourceFile;
            result.ApplyActionToDescendantsAndSelf(ust => ust.TextSpan = ust.TextSpan.AddOffset(offset));
            return result;
        }

        private static string ResponseWriteWrappedString(string expression)
        {
            return $"Response.Write({expression})";
        }

        private static string HtmlEncodeWrappedString(string expression)
        {
            return $"System.Net.WebUtility.HtmlEncode({expression})";
        }
    }
}
