﻿namespace PT.PM.Cli.Common
{
    public class CliParameters
    {
        [Option('c', "config", HelpText = "Json file with settings")]
        public string ConfigFile { get; set; }

        [Option('f', "files", HelpText = "Input file or directory to be processed")]
        public string InputFileNameOrDirectory { get; set; }

        [Option('l', "languages", HelpText = "Languages to be processed", Separator = ',')]
        public string[] Languages { get; set; }

        [Option("patterns", HelpText = "Patterns to be processed")]
        public string Patterns { get; set; }

        [Option("pattern-ids", HelpText = "Pattern identifiers to be processed", Separator = ',')]
        public string[] PatternIds { get; set; }

        [Option('t', "threads", HelpText = "Number of processing threads")]
        public int? ThreadCount { get; set; }

        [Option("not-fold-consts", HelpText = "Do not propagate and fold constants")]
        public bool? NotFoldConstants { get; set; }

        [Option('s', "stage", HelpText = "End processing stage. By default: Match")]
        public string Stage { get; set; }

        [Option("max-stack-size", HelpText = "Thread stack size in bytes")]
        public uint? MaxStackSize { get; set; }

        [Option('m', "memory", HelpText = "Approximate max memory consumption in megabytes")]
        public uint? Memory { get; set; }

        [Option("timeout", HelpText = "Max spent time per file in seconds")]
        public uint? FileTimeout { get; set; }

        [Option("logs-dir", HelpText = "Logs directory")]
        public string LogsDir { get; set; }

        [Option("log-level", HelpText = "Log level (off, error, warn, info, debug)")]
        public string LogLevel { get; set; }

        [Option("no-log-to-file", HelpText = "Disable logging to file")]
        public bool? NoLogToFile { get; set; }

        [Option("temp-dir", HelpText = "Temp directory")]
        public string TempDir { get; set; }

        [Option("indented", HelpText = "Dump trees with indents")]
        public bool? IndentedDump { get; set; }

        [Option("no-text-spans", HelpText = "Do not include text spans in dump trees")]
        public bool? NotIncludeTextSpansInDump { get; set; }

        [Option("line-column", HelpText = "Use line-column format for text spans in dump")]
        public bool? LineColumnTextSpans { get; set; }

        [Option("dump-code", HelpText = "Dump content of source code file to dump")]
        public bool? IncludeCodeInDump { get; set; }

        [Option("strict", HelpText = "Strict json deserialization if set true")]
        public bool? StrictJson { get; set; }

        [Option('d', "dump", HelpText = "Stages to be dumped (ParseTree, Ust)", Separator = ',')]
        public string[] DumpStages { get; set; }

        [Option("dump-patterns", HelpText = "Dump patterns to Json")]
        public bool? DumpPatterns { get; set; }

        [Option('o', "json-output", HelpText = "Dump output info to json file")]
        public bool? IsDumpJsonOutput { get; set; }

        [Option('r', "render", HelpText = "Stages to be rendered", Separator = ',')]
        public string[] RenderStages { get; set; }

        [Option("render-format", HelpText = "Graph render format (Png, Svg, etc.)")]
        public string RenderFormat { get; set; }

        [Option("render-direction", HelpText = "Graph render direction (TopBottom, LeftRight, etc.)")]
        public string RenderDirection { get; set; }

        [Option("serialize-format", HelpText = "Serialize format (json, msgpack)")]
        public string SerializationFormat { get; set; }
    }
}
