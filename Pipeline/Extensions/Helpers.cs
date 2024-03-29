﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineRunner;
using PipelineLauncher.Abstractions.PipelineStage.Dto;
using PipelineLauncher.PipelineSetup;
using PipelineLauncher.PipelineSetup.StageSetup;
using PipelineLauncher.PipelineStage;

namespace PipelineLauncher.Extensions
{
    internal static partial class Helpers
    {
        public static ITargetStageSetup<TInput> GetFirstStage<TInput>(this IPipelineSetup pipelineSetup)
        {
            IStageSetup stageSetup = ((PipelineSetup<TInput>)pipelineSetup).StageSetup;

            while (stageSetup.PreviousStageSetup != null)
            {
                stageSetup = stageSetup.PreviousStageSetup;
            }

            return (ITargetStageSetup<TInput>)stageSetup;
        }

        public static ITargetStageSetup<TInput> CopyFullPipelineReturnNewRoot<TInput>(this IPipelineSetup pipelineSetup)
        {
            ITargetStageSetup<TInput> stageSetup = pipelineSetup.GetFirstStage<TInput>();
            ITargetStageSetup<TInput> stageSetupNew = stageSetup.CreateDeepCopy();

            var asBase = (IStageSetup)stageSetupNew;

            CopyWithChild(stageSetup, stageSetupNew, null, ref asBase);

            return (ITargetStageSetup<TInput>)stageSetupNew;
        }

        public static ISourceStageSetup<TInput> CopyFullPipelineReturnNewCurrent<TInput>(this IPipelineSetup pipelineSetup)
        {
            IStageSetup stageSetupCurrent = ((PipelineSetup<TInput>)pipelineSetup).StageSetup;

            IStageSetup stageSetup = pipelineSetup.GetFirstStage<TInput>();
            IStageSetup stageSetupNew = stageSetup.CreateDeepCopy();

            IStageSetup stageSetupCurrentNew = null;

            CopyWithChild(stageSetup, stageSetupNew, stageSetupCurrent, ref stageSetupCurrentNew);

            return (ISourceStageSetup<TInput>)((PipelineSetup<TInput>) pipelineSetup).StageSetup;
        }

        public static IStageSetup CopyWithChild(IStageSetup stageSetup, IStageSetup stageSetupNew, IStageSetup copyFromStageSetup, ref IStageSetup copyFromStageSetupNew)
        {
            stageSetupNew.PreviousStageSetup = stageSetup.PreviousStageSetup?.CreateDeepCopy();

            if (copyFromStageSetup != null && stageSetup.Equals(copyFromStageSetup))
            {
                copyFromStageSetupNew = stageSetupNew;
            }

            foreach (var nextStageSetup in stageSetup.NextStageSetup)
            {
                stageSetupNew.NextStageSetup.Add(CopyWithChild(nextStageSetup, nextStageSetup.CreateDeepCopy(), copyFromStageSetup, ref copyFromStageSetupNew));
            }

            return stageSetupNew;
        }

        public static void DestroyStageBlocks(this IStageSetup stageSetup)
        {
            if (stageSetup == null)
            {
                return;
            }

            stageSetup.DestroyExecutionBlock();

            if (stageSetup.NextStageSetup == null || !stageSetup.NextStageSetup.Any()) return;
            foreach (var nextStage in stageSetup.NextStageSetup)
            {
                DestroyStageBlocks(nextStage);
            }
        }

        public static Predicate<PipelineStageItem<TOutput>> GetPredicate<TOutput>(
            this PipelinePredicate<TOutput> predicate,
            ITargetBlock<PipelineStageItem<TOutput>> target)
        {
            return x =>
            {
                if (x == null)
                {
                    return false;
                }

                if (x.Item == null)
                {
                    return true;
                }

                try
                {
                    var result = predicate(x.Item);

                    switch (result)
                    {
                        case PredicateResult.Keep:
                            return true;
                        case PredicateResult.Skip:
                            target.Post(new SkipStageItem<TOutput>(x.Item, predicate.GetType()));
                            return false;
                        case PredicateResult.Remove:
                            target.Post(new RemoveStageItem<TOutput>(x.Item, predicate.GetType()));
                            return false;
                        default:
                            target.Post(new ExceptionStageItem<TOutput>(new ArgumentOutOfRangeException(), null, predicate.GetType(), new object[] { x.Item }));
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    target.Post(new ExceptionStageItem<TOutput>(ex, null, predicate.GetType(), new object[] { x.Item }));
                    return false;
                }
            };
        }

        public static Predicate<PipelineStageItem<TOutput>> GetPredicate<TOutput>(
            this PipelinePredicate<TOutput> predicate,
            ITargetBlock<IEnumerable<PipelineStageItem<TOutput>>> target,
            Type stageType)
        {
            return x =>
            {
                if (x == null)
                {
                    return false;
                }

                if (x.Item == null)
                {
                    switch (x)
                    {
                        case NonResultStageItem<TOutput> noneItem when noneItem.ReadyToProcess<TOutput>(stageType):
                            return predicate.ExecutePredicate((TOutput)noneItem.OriginalItem, target);
                        default:
                            target.Post(new[] { x });
                            return false;
                    }
                }

                return predicate.ExecutePredicate(x.Item, target);
            };
        }

        private static bool ExecutePredicate<TItem>(this PipelinePredicate<TItem> predicate, TItem item, ITargetBlock<IEnumerable<PipelineStageItem<TItem>>> target)
        {
            try
            {
                var result = predicate(item);

                switch (result)
                {
                    case PredicateResult.Keep:
                        return true;
                    case PredicateResult.Skip:
                        target.Post(new[] { new SkipStageItem<TItem>(item, predicate.GetType()) });
                        return false;
                    case PredicateResult.Remove:
                        target.Post(new[] { new RemoveStageItem<TItem>(item, predicate.GetType()) });
                        return false;
                    default:
                        target.Post(new[] { new ExceptionStageItem<TItem>(new ArgumentOutOfRangeException(), null, predicate.GetType(), new object[]{item}) });
                        return false;
                }
            }
            catch (Exception ex)
            {
                target.Post(new[] { new ExceptionStageItem<TItem>(ex, null, predicate.GetType(), new object[] { item }) });
                return false;
            }
        }
    }


    internal static partial class Helpers
    {
        internal static class Strings
        {
            public static string RetryOnAwaitable =
                $"{nameof(ActionsSet.Retry)} action cannot be used on '{nameof(IAwaitablePipelineRunner<object, object>)}', " +
                $"try use '{nameof(IAwaitablePipelineRunner<object, object>.SetupInstantExceptionHandler)}' " +
                $"on '{nameof(IAwaitablePipelineRunner<object, object>)}' to perform that action.";

            public static string RetriesMaxCountReached =
                $"{nameof(ActionsSet.Retry)} was called" + " more than '{0}' times.";
        }
    }
}
