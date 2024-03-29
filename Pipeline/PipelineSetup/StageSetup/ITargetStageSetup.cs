﻿using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.PipelineStage;
using System.Threading.Tasks.Dataflow;

namespace PipelineLauncher.PipelineSetup.StageSetup
{
    internal interface ITargetStageSetup<TInput> : IStageSetup
    {
        new ITargetStageSetup<TInput> CreateDeepCopy();
        new ITargetBlock<PipelineStageItem<TInput>> RetrieveExecutionBlock(StageCreationContext context);
    }
}
