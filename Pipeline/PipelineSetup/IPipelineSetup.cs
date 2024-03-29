﻿using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineRunner;
using PipelineLauncher.Abstractions.PipelineRunner.Configurations;
using PipelineLauncher.Abstractions.PipelineStage;
using PipelineLauncher.Abstractions.PipelineStage.Configurations;
using PipelineLauncher.Stages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipelineLauncher.Abstractions.Stages;

namespace PipelineLauncher.PipelineSetup
{

    public interface IPipelineSetup<TPipelineInput, TStageOutput> : IPipelineSetupSource<TStageOutput>
    {
        #region Generic

        #region BulkStages

        new IPipelineSetup<TPipelineInput, TNextStageOutput> BulkStage<TBulkStage, TNextStageOutput>(PipelinePredicate<TStageOutput> predicate = null)
            where TBulkStage : class, IBulkStage<TStageOutput, TNextStageOutput>;

        new IPipelineSetup<TPipelineInput, TStageOutput> BulkStage<TBulkStage>(PipelinePredicate<TStageOutput> predicate = null)
            where TBulkStage : class, IBulkStage<TStageOutput, TStageOutput>;

        #endregion

        #region Stages

        new IPipelineSetup<TPipelineInput, TStageOutput> Stage<TStage>(PipelinePredicate<TStageOutput> predicate = null)
           where TStage : class, IStage<TStageOutput, TStageOutput>;

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TStage, TNextStageOutput>(PipelinePredicate<TStageOutput> predicate = null)
           where TStage : class, IStage<TStageOutput, TNextStageOutput>;

        #endregion

        #endregion

        #region Nongeneric

        #region BulkStages

        new IPipelineSetup<TPipelineInput, TNextStageOutput> BulkStage<TNextStageOutput>(IBulkStage<TStageOutput, TNextStageOutput> bulkStage, PipelinePredicate<TStageOutput> predicate = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> BulkStage<TNextStageOutput>(Func<TStageOutput[], IEnumerable<TNextStageOutput>> func, BulkStageConfiguration bulkStageConfiguration = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> BulkStage<TNextStageOutput>(Func<TStageOutput[], Task<IEnumerable<TNextStageOutput>>> func, BulkStageConfiguration bulkStageConfiguration = null);

        #endregion

        #region Stages

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TNextStageOutput>(IStage<TStageOutput, TNextStageOutput> stage, PipelinePredicate<TStageOutput> predicate = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TNextStageOutput>(Func<TStageOutput, TNextStageOutput> func, StageConfiguration stageConfiguration = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TNextStageOutput>(Func<TStageOutput, StageOption<TStageOutput, TNextStageOutput>, TNextStageOutput> func, StageConfiguration stageConfiguration = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TNextStageOutput>(Func<TStageOutput, Task<TNextStageOutput>> func, StageConfiguration stageConfiguration = null);

        new IPipelineSetup<TPipelineInput, TNextStageOutput> Stage<TNextStageOutput>(Func<TStageOutput, StageOption<TStageOutput, TNextStageOutput>, Task<TNextStageOutput>> func, StageConfiguration stageConfiguration = null);

        #endregion

        #endregion

        #region Branches

        IPipelineSetup<TPipelineInput, TNextStageOutput> Broadcast<TNextStageOutput>(params (Predicate<TStageOutput> predicate,
            Func<IPipelineSetup<TPipelineInput, TStageOutput>, IPipelineSetup<TPipelineInput, TNextStageOutput>> branch)[] branches);

        IPipelineSetup<TPipelineInput, TNextStageOutput> Branch<TNextStageOutput>(params (Predicate<TStageOutput> predicate,
            Func<IPipelineSetup<TPipelineInput, TStageOutput>, IPipelineSetup<TPipelineInput, TNextStageOutput>> branch)[] branches);

        IPipelineSetup<TPipelineInput, TNextStageOutput> Branch<TNextStageOutput>(ConditionExceptionScenario conditionExceptionScenario,
            params (Predicate<TStageOutput> predicate,
            Func<IPipelineSetup<TPipelineInput, TStageOutput>, IPipelineSetup<TPipelineInput, TNextStageOutput>> branch)[] branches);

        #endregion

        #region Native



        #endregion

        IPipelineSetup<TPipelineInput, TNextStageOutput> MergeWith<TNextStageOutput>(IPipelineSetup<TStageOutput, TNextStageOutput> pipelineSetup);

        IAwaitablePipelineRunner<TPipelineInput, TStageOutput> CreateAwaitable(AwaitablePipelineCreationConfig pipelineCreationConfig = null);

        IPipelineRunner<TPipelineInput, TStageOutput> Create(PipelineCreationConfig pipelineCreationConfig = null);
    }
}