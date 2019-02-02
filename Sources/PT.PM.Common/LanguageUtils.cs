﻿using System;
using System.Collections.Generic;
using System.Linq;
using static PT.PM.Common.Language;

namespace PT.PM.Common
{
    public static class LanguageUtils
    {
        private static readonly string[] LanguageSeparators = { " ", ",", ";", "|" };

        private static readonly Dictionary<Language, Func<ILanguageParser>> parserConstructors = new Dictionary<Language, Func<ILanguageParser>>();
        private static readonly Dictionary<Language, Func<IParseTreeToUstConverter>> converterConstructors = new Dictionary<Language, Func<IParseTreeToUstConverter>>();

        public static readonly Dictionary<Language, LanguageInfo> LanguageInfos = new Dictionary<Language, LanguageInfo>
        {
            [CSharp] = new LanguageInfo(CSharp, ".cs", false, "C#", hasAntlrParser: false),
            [Java] = new LanguageInfo(Java, ".java", false, "Java"),
            [Php] = new LanguageInfo(Php, new[] { ".php" }, true, "PHP", new [] { JavaScript, Html }),
            [PlSql] = new LanguageInfo(PlSql, new[] { ".sql", ".pks", ".pkb", ".tps", ".vw" }, true, "PL/SQL"),
            [TSql] = new LanguageInfo(TSql, ".sql", true, "T-SQL"),
            [MySql] = new LanguageInfo(MySql, ".sql", true, "MySql"),
            [JavaScript] = new LanguageInfo(JavaScript, ".js", false, "JavaScript", hasAntlrParser: false),
            [Aspx] = new LanguageInfo(Aspx, new[] { ".asax", ".aspx", ".ascx", ".master" }, false, "Aspx", new[] { CSharp }, false, false),
            [Html] = new LanguageInfo(Html, ".html", true, "HTML", new[] { JavaScript }),
            [C] = new LanguageInfo(C, new[] { ".c", ".h" }, false, "C", hasAntlrParser: false),
            [CPlusPlus] = new LanguageInfo(CPlusPlus, new[] { ".cpp", ".hpp", ".cc", ".cxx" }, false, "C++", new[] { C }, hasAntlrParser: false),
            [ObjectiveC] = new LanguageInfo(ObjectiveC, new[] { ".m", ".mm" }, false, "Objective-C", new[] { C }, hasAntlrParser: false),
            [Swift] = new LanguageInfo(Swift, new[] { ".swift" }, false, "Swift", hasAntlrParser: false),
            [Uncertain] = new LanguageInfo(Uncertain, ".*", false, "Uncertain", hasAntlrParser: false),
            [Language.Json] = new LanguageInfo(Language.Json, ".json", false, "Json UST | CPG", hasAntlrParser: false),
            [Language.MessagePack] = new LanguageInfo(Language.MessagePack, ".msgpack", false, "MessagePack UST | CPG", hasAntlrParser: false),
        };

        public static readonly HashSet<Language> Languages = new HashSet<Language>
        {
            CSharp,
            Java,
            Php,
            PlSql,
            TSql,
            MySql,
            JavaScript,
            Aspx,
            Html,
            C,
            CPlusPlus,
            ObjectiveC,
            Swift
        };

        public static readonly HashSet<Language> SqlLanguages = new HashSet<Language>
        {
            PlSql,
            TSql,
            MySql
        };

        public static readonly HashSet<Language> CLangsLanguages = new HashSet<Language>
        {
            C,
            ObjectiveC,
            CPlusPlus,
            Swift
        };

        public static readonly HashSet<Language> SerializationLanguages = new HashSet<Language>
        {
            Language.Json,
            Language.MessagePack
        };

        public static readonly HashSet<Language> PatternLanguages = new HashSet<Language>();
        public static readonly Dictionary<Language, HashSet<Language>> SuperLanguages = new Dictionary<Language, HashSet<Language>>();
        public static readonly HashSet<Language> LanguagesWithParser = new HashSet<Language>();

        static LanguageUtils()
        {
            foreach (var pair in LanguageInfos)
            {
                Language language = pair.Key;
                LanguageInfo languageInfo = pair.Value;

                if (languageInfo.IsPattern)
                {
                    PatternLanguages.Add(language);
                }

                foreach (Language sublanguage in languageInfo.Sublanguages)
                {
                    if (!SuperLanguages.TryGetValue(sublanguage, out HashSet<Language> superLanguages))
                    {
                        superLanguages = new HashSet<Language>();
                        SuperLanguages.Add(sublanguage, superLanguages);
                    }

                    superLanguages.Add(language);
                }
            }
        }

        public static bool IsSql(this Language language) => SqlLanguages.Contains(language);

        public static bool IsSerialization(this Language language) => SerializationLanguages.Contains(language);

        public static bool IsCLangs(this Language language) => CLangsLanguages.Contains(language);

        public static bool IsCaseInsensitive(this Language language) => LanguageInfos[language].IsCaseInsensitive;

        public static string[] GetExtensions(this Language language) => LanguageInfos[language].Extensions;

        public static Language[] GetSublanguages(this Language language) => LanguageInfos[language].Sublanguages;

        public static Language[] GetLanguagesByExtension(string extension)
        {
            var result = new List<Language>();

            foreach (var languageInfo in LanguageInfos)
            {
                if (languageInfo.Value.Extensions.Contains(extension))
                {
                    result.Add(languageInfo.Key);
                }
            }

            return result.ToArray();
        }

        public static bool HasAntlrParser(this Language language) => LanguageInfos[language].HasAntlrParser;

        public static bool IsParserExists(this Language language) => LanguagesWithParser.Contains(language) || language.IsSerialization();

        public static void RegisterParserConverter(Language language, Func<ILanguageParser> parserConstructor, Func<IParseTreeToUstConverter> converterConstructor)
        {
            RegisterParser(language, parserConstructor);
            RegisterConverter(language, converterConstructor);
        }

        public static void RegisterParser(Language language, Func<ILanguageParser> parserConstructor)
        {
            parserConstructors[language] = parserConstructor;
            LanguagesWithParser.Add(language);
        }

        public static void RegisterConverter(Language language, Func<IParseTreeToUstConverter> converterConstructor)
        {
            converterConstructors[language] = converterConstructor;
        }

        public static ILanguageParser CreateParser(this Language language)
        {
            if (parserConstructors.TryGetValue(language, out Func<ILanguageParser> parserConstructor))
            {
                return parserConstructor();
            }

            throw new NotImplementedException($"Language {language} parser is not supported");
        }

        public static IParseTreeToUstConverter CreateConverter(this Language language)
        {
            if (converterConstructors.TryGetValue(language, out Func<IParseTreeToUstConverter> converterConstructor))
            {
                return converterConstructor();
            }

            throw new NotImplementedException($"Language {language} converter is not supported");
        }

        public static HashSet<Language> ParseLanguages(this string languages, bool allByDefault = true,
            bool patternLanguages = false)
        {
            if (languages == null)
            {
                return new HashSet<Language>(!patternLanguages ? (IEnumerable<Language>)LanguageInfos.Keys : PatternLanguages);
            }

            return languages.Split(LanguageSeparators, StringSplitOptions.RemoveEmptyEntries).ParseLanguages(allByDefault, patternLanguages);
        }

        public static HashSet<Language> ParseLanguages(this IEnumerable<string> languageStrings, bool allByDefault = true,
            bool patternLanguages = false)
        {
            string[] languageStringsArray = languageStrings?.ToArray() ?? ArrayUtils<string>.EmptyArray;
            var languages = !patternLanguages ? Languages : PatternLanguages;
            HashSet<Language> negationLangs = new HashSet<Language>(languages);

            if (allByDefault && languageStringsArray.Length == 0)
            {
                return negationLangs;
            }

            var langs = new HashSet<Language>();
            bool containsNegation = false;
            foreach (string languageString in languageStringsArray)
            {
                bool negation = false;
                string langStr = languageString;
                if (langStr.StartsWith("~") || langStr.StartsWith("!"))
                {
                    negation = true;
                    langStr = langStr.Substring(1);
                }
                bool isSql = langStr.EqualsIgnoreCase("sql");

                foreach (Language language in languages)
                {
                    LanguageInfo languageInfo = LanguageInfos[language];
                    bool result = isSql
                        ? language.IsSql()
                        : (language.ToString().EqualsIgnoreCase(langStr) || languageInfo.Title.EqualsIgnoreCase(langStr) ||
                           languageInfo.Extensions.Any(ext => (ext.StartsWith(".") ? ext.Substring(1) : ext).EqualsIgnoreCase(langStr)));
                    if (negation)
                    {
                        containsNegation = true;
                        if (result)
                        {
                            negationLangs.Remove(language);
                        }
                    }
                    else
                    {
                        if (result)
                        {
                            langs.Add(language);
                        }
                    }
                };
            }

            if (containsNegation)
            {
                foreach (Language lang in negationLangs)
                {
                    langs.Add(lang);
                }
            }

            return langs;
        }

        public static HashSet<Language> GetSelfAndSublanguages(this Language language)
        {
            var result = new HashSet<Language> { language };
            foreach (Language lang in LanguageInfos[language].Sublanguages)
                result.Add(lang);
            return result;
        }
    }
}
