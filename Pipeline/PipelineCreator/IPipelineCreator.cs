﻿using PipelineLauncher.Abstractions.PipelineStage.Configurations;
using PipelineLauncher.Abstractions.Services;
using PipelineLauncher.PipelineSetup;
using PipelineLauncher.Stages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipelineLauncher.Abstractions.PipelineStage;
using PipelineLauncher.Abstractions.Stages;

namespace PipelineLauncher
{
    public interface IPipelineCreator
    {
        IPipelineCreator WithStageService(IStageService stageService);

        IPipelineCreator WithStageService(Func<Type, IStage> stageService);
        
        IPipelineCreator UseDefaultServiceResolver(bool useDefaultServiceResolver);

        IPipelineSetup<TInput, TInput> Prepare<TInput>();

        IPipelineSetup<TInput, TInput> BulkPrepare<TInput>(BulkStageConfiguration stageConfiguration = null);

        #region Generic

        #region BulkStages

        [Obsolete("Use 'BulkPrepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> BulkStage<TBulkStage, TInput, TOutput>()
            where TBulkStage : class, IBulkStage<TInput, TOutput>;
        [Obsolete("Use 'BulkPrepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TInput> BulkStage<TBulkStage, TInput>()
            where TBulkStage : class, IBulkStage<TInput, TInput>;

        #endregion

        #region Stages
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TStage, TInput, TOutput>()
            where TStage : class, IStage<TInput, TOutput>;
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TInput> Stage<TStage, TInput>()
            where TStage : class, IStage<TInput, TInput>;

        #endregion

        #endregion

        #region Nongeneric

        #region BulkStages
        [Obsolete("Use 'BulkPrepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(IBulkStage<TInput, TOutput> bulkStage);
        [Obsolete("Use 'BulkPrepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(Func<TInput[], IEnumerable<TOutput>> func, BulkStageConfiguration bulkStageConfiguration = null);
        [Obsolete("Use 'BulkPrepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(Func<TInput[], Task<IEnumerable<TOutput>>> func, BulkStageConfiguration bulkStageConfiguration = null);

        #endregion

        #region Stages
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(IStage<TInput, TOutput> stage);
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, TOutput> func);
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, StageOption<TInput, TOutput>,  TOutput> funcWithOption);
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, Task<TOutput>> func);
        [Obsolete("Use 'Prepare' instead, since first functional stage is unnecessary.")]
        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, StageOption<TInput, TOutput>, Task<TOutput>> funcWithOption);

        #endregion

        #endregion
    }
}