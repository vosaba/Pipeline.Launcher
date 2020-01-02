﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineEvents;
using PipelineLauncher.Abstractions.PipelineStage;
using PipelineLauncher.PipelineStage;

namespace PipelineLauncher.PipelineRunner
{
    internal class PipelineRunner<TInput, TOutput> : IPipelineRunner<TInput, TOutput>
    {
        protected virtual StageCreationOptions CreationOptions => new StageCreationOptions(PipelineType.Normal);

        protected readonly CancellationToken CancellationToken;

        protected Func<StageCreationOptions, bool, ITargetBlock<PipelineStageItem<TInput>>> RetrieveFirstBlock;
        protected Func<StageCreationOptions, bool, ISourceBlock<PipelineStageItem<TOutput>>> RetrieveLastBlock;
        protected ActionBlock<PipelineStageItem<TOutput>> SortingBlock;

        public event ItemReceivedEventHandler<TOutput> ItemReceivedEvent;
        public event ExceptionItemsReceivedEventHandler ExceptionItemsReceivedEvent;
        public event SkippedItemReceivedEventHandler SkippedItemReceivedEvent;


        public bool Post(TInput input)
        {
            return Post(new [] {input});
        }

        public bool Post(IEnumerable<TInput> input)
        {
            var firstBlock = RetrieveFirstBlock(CreationOptions, false);
            return input.Select(x => new PipelineStageItem<TInput>(x)).All(x => firstBlock.Post(x));
        }

        internal PipelineRunner(
            Func<StageCreationOptions, bool, ITargetBlock<PipelineStageItem<TInput>>> retrieveFirstBlock,
            Func<StageCreationOptions, bool, ISourceBlock<PipelineStageItem<TOutput>>> retrieveLastBlock,
            CancellationToken cancellationToken,
            bool initSortingBlock = true)
        {
            CancellationToken = cancellationToken;

            RetrieveFirstBlock = retrieveFirstBlock;
            RetrieveLastBlock = retrieveLastBlock;

            if (initSortingBlock)
            {
                InitSortingBlock();
            }
        }

        protected void InitSortingBlock()
        {
            SortingBlock = new ActionBlock<PipelineStageItem<TOutput>>(input =>
            {
                switch (input)
                {
                    case ExceptionStageItem<TOutput> exceptionItem:
                        ExceptionItemsReceivedEvent?.Invoke(new ExceptionItemsEventArgs(exceptionItem.FailedItems, exceptionItem.StageType, exceptionItem.Exception, exceptionItem.ReProcessItems));
                        return;
                    case NoneResultStageItem<TOutput> nonResultItem:
                        SkippedItemReceivedEvent?.Invoke(new SkippedItemEventArgs(nonResultItem.OriginalItem, nonResultItem.StageType));
                        return;
                    default:
                        ItemReceivedEvent?.Invoke(input.Item);
                        return;
                }
            });

            RetrieveLastBlock(CreationOptions, false).LinkTo(SortingBlock, new DataflowLinkOptions { PropagateCompletion = false });
        }
    }
}
