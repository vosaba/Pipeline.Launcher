﻿using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineStage;
using PipelineLauncher.Abstractions.PipelineStage.Configurations;
using PipelineLauncher.Stages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipelineLauncher.Abstractions.Stages;

namespace PipelineLauncher.PipelineSetup
{
    internal partial class PipelineSetup<TPipelineInput, TStageOutput>// : PipelineSetup<TPipelineInput>, IPipelineSetup<TPipelineInput, TStageOutput>
    {
        #region Generic

        #region BulkStages

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.BulkStage<TBulkStage, TNextStageOutput>(PipelinePredicate<TStageOutput> predicate = null)
            => BulkStage<TBulkStage, TNextStageOutput>(predicate);

        IPipelineSetupSource<TStageOutput> IPipelineSetupSource<TStageOutput>.BulkStage<TBulkStage>(PipelinePredicate<TStageOutput> predicate = null) 
            => BulkStage<TBulkStage>(predicate);

        #endregion

        #region Stages

        IPipelineSetupSource<TStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TStage>(PipelinePredicate<TStageOutput> predicate = null)
            => Stage<TStage>(predicate);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TStage, TNextStageOutput>(PipelinePredicate<TStageOutput> predicate = null)
            => Stage<TStage, TNextStageOutput>(predicate);

        #endregion

        #endregion

        #region Nongeneric

        #region BulkStages

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.BulkStage<TNextStageOutput>(IBulkStage<TStageOutput, TNextStageOutput> baseStageBulkStage, PipelinePredicate<TStageOutput> predicate = null)
            => BulkStage<TNextStageOutput>(baseStageBulkStage, predicate);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.BulkStage<TNextStageOutput>(Func<TStageOutput[], IEnumerable<TNextStageOutput>> bulkFunc, BulkStageConfiguration bulkStageConfiguration)
            => BulkStage(bulkFunc, bulkStageConfiguration);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.BulkStage<TNextStageOutput>(Func<TStageOutput[], Task<IEnumerable<TNextStageOutput>>> bulkFunc, BulkStageConfiguration bulkStageConfiguration)
            => BulkStage(bulkFunc, bulkStageConfiguration);

        #endregion

        #region Stages

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TNextStageOutput>(IStage<TStageOutput, TNextStageOutput> stage, PipelinePredicate<TStageOutput> predicate = null)
            => Stage<TNextStageOutput>(stage, predicate);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TNextStageOutput>(Func<TStageOutput, TNextStageOutput> func, StageConfiguration stageConfiguration = null)
            => Stage<TNextStageOutput>(func, stageConfiguration);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TNextStageOutput>(Func<TStageOutput, StageOption<TStageOutput, TNextStageOutput>, TNextStageOutput> funcWithOption, StageConfiguration stageConfiguration = null)
            => Stage<TNextStageOutput>(funcWithOption, stageConfiguration);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TNextStageOutput>(Func<TStageOutput, Task<TNextStageOutput>> func, StageConfiguration stageConfiguration = null)
            => Stage<TNextStageOutput>(func, stageConfiguration);

        IPipelineSetupSource<TNextStageOutput> IPipelineSetupSource<TStageOutput>.Stage<TNextStageOutput>(Func<TStageOutput, StageOption<TStageOutput, TNextStageOutput>, Task<TNextStageOutput>> funcWithOption, StageConfiguration stageConfiguration = null)
            => Stage<TNextStageOutput>(funcWithOption, stageConfiguration);

        #endregion

        #endregion

        #region Branches

        //public IPipelineSetupSource<TNextStageOutput> Broadcast<TNextStageOutput>(params (PipelinePredicate<TStageOutput> predicate, Func<IPipelineSetupSource<TStageOutput>, IPipelineSetupSource<TNextStageOutput>> branch)[] branches)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPipelineSetupSource<TNextStageOutput> Branch<TNextStageOutput>(params (PipelinePredicate<TStageOutput> predicate, Func<IPipelineSetupSource<TStageOutput>, IPipelineSetupSource<TNextStageOutput>> branch)[] branches)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPipelineSetupSource<TNextStageOutput> Branch<TNextStageOutput>(ConditionExceptionScenario conditionExceptionScenario,
        //    params (PipelinePredicate<TStageOutput> predicate, Func<IPipelineSetupSource<TStageOutput>, IPipelineSetupSource<TNextStageOutput>> branch)[] branches)
        //{
        //    return Branch(conditionExceptionScenario, branches);
        //}

        #endregion
    }
}