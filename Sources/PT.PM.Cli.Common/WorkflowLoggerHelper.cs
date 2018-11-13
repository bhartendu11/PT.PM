﻿using PT.PM.Common;
using PT.PM.Matching;

namespace PT.PM.Cli.Common
{
    public class WorkflowLoggerHelper : WorkflowLoggerHelperBase<Stage, WorkflowResult, PatternRoot, Stage>
    {
        public WorkflowLoggerHelper(ILogger logger, WorkflowResult workflowResult)
            : base(logger, workflowResult)
        {
        }

        protected override void LogAdvancedStageInfo()
        {
            if (WorkflowResult.Stage >= Stage.Match)
            {
                LogStageTime(nameof(Stage.Match));
            }
        }
    }
}
