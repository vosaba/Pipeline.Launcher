﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineRunner;
using PipelineLauncher.Abstractions.PipelineRunner.Configurations;
using PipelineLauncher.Abstractions.PipelineStage;

namespace PipelineLauncher.PipelineRunner
{
    internal class PipelineRunner<TInput, TOutput> : PipelineRunnerBase<TInput, TOutput>, IPipelineRunner<TInput, TOutput>
    {
        private readonly ITargetBlock<PipelineStageItem<TInput>> _firstBlock;
        private readonly ISourceBlock<PipelineStageItem<TOutput>> _lastBlock;

        protected sealed override StageCreationOptions CreationOptions => new StageCreationOptions(PipelineType.Normal, true);
        protected readonly PipelineConfig PipelineConfig;

        public bool Post(TInput input)
        {
            return Post(new [] {input});
        }

        public bool Post(IEnumerable<TInput> input)
        {
            return input.Select(x => new PipelineStageItem<TInput>(x)).All(x => _firstBlock.Post(x));
        }

        internal PipelineRunner(
            Func<StageCreationOptions, bool, ITargetBlock<PipelineStageItem<TInput>>> retrieveFirstBlock,
            Func<StageCreationOptions, bool, ISourceBlock<PipelineStageItem<TOutput>>> retrieveLastBlock,
            CancellationToken cancellationToken,
            PipelineConfig pipelineConfig):
            base(retrieveFirstBlock, retrieveLastBlock, cancellationToken)
        {
            PipelineConfig = pipelineConfig ?? new PipelineConfig();

            _firstBlock = RetrieveFirstBlock(CreationOptions, false);
            _lastBlock = retrieveLastBlock(CreationOptions, false);

            GenerateSortingBlock(_lastBlock);
        }
    }
}