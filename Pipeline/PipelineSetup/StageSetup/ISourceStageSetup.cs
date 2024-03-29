﻿using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.PipelineStage;
using System.Threading.Tasks.Dataflow;

namespace PipelineLauncher.PipelineSetup.StageSetup
{
    internal interface ISourceStageSetup<TOutput> : IStageSetup
    {
        new ISourceStageSetup<TOutput> CreateDeepCopy();
        new ISourceBlock<PipelineStageItem<TOutput>> RetrieveExecutionBlock(StageCreationContext context);
    }
}