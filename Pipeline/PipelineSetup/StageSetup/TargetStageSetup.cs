﻿using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.PipelineStage;
using System;
using System.Threading.Tasks.Dataflow;

namespace PipelineLauncher.PipelineSetup.StageSetup
{
    internal class TargetStageSetup<TIn> : StageSetup, ITargetStageSetup<TIn>
    {
        private readonly Func<StageCreationContext, ITargetBlock<PipelineStageItem<TIn>>> _executionBlockCreator;

        public TargetStageSetup(Func<StageCreationContext, ITargetBlock<PipelineStageItem<TIn>>> executionBlockCreator)
            : base(executionBlockCreator)
        {
            _executionBlockCreator = executionBlockCreator;
        }

        public new ITargetStageSetup<TIn> CreateDeepCopy()
            => new TargetStageSetup<TIn>(_executionBlockCreator);

        public new ITargetBlock<PipelineStageItem<TIn>> RetrieveExecutionBlock(StageCreationContext context)
            => (ITargetBlock<PipelineStageItem<TIn>>)base.RetrieveExecutionBlock(context);
    }
}