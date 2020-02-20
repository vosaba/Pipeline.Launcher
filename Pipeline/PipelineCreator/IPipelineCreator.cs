﻿using PipelineLauncher.Abstractions.PipelineStage.Configurations;
using PipelineLauncher.Abstractions.Services;
using PipelineLauncher.Abstractions.Stages;
using PipelineLauncher.PipelineSetup;
using PipelineLauncher.Stages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineLauncher
{
    public interface IPipelineCreator
    {
        IPipelineCreator WithStageService(IStageService stageService);

        IPipelineCreator WithStageService(Func<Type, IPipelineStage> stageService);
        
        IPipelineCreator UseDefaultServiceResolver(bool useDefaultServiceResolver);

        IPipelineSetup<TInput, TInput> Prepare<TInput>();

        IPipelineSetup<TInput, TInput> BulkPrepare<TInput>(BulkStageConfiguration stageConfiguration = null);

        #region Generic

        #region BulkStages

        IPipelineSetup<TInput, TOutput> BulkStage<TBulkStage, TInput, TOutput>()
            where TBulkStage : BulkStage<TInput, TOutput>;

        IPipelineSetup<TInput, TInput> BulkStage<TBulkStage, TInput>()
            where TBulkStage : BulkStage<TInput, TInput>;

        #endregion

        #region Stages

        IPipelineSetup<TInput, TOutput> Stage<TStage, TInput, TOutput>()
            where TStage : Stage<TInput, TOutput>;

        IPipelineSetup<TInput, TInput> Stage<TStage, TInput>()
            where TStage : Stage<TInput, TInput>;

        #endregion

        #endregion

        #region Nongeneric

        #region BulkStages

        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(BulkStage<TInput, TOutput> bulkStage);

        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(Func<TInput[], IEnumerable<TOutput>> bulkFunc, BulkStageConfiguration bulkStageConfiguration = null);

        IPipelineSetup<TInput, TOutput> BulkStage<TInput, TOutput>(Func<TInput[], Task<IEnumerable<TOutput>>> bulkFunc, BulkStageConfiguration bulkStageConfiguration = null);

        #endregion

        #region Stages

        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Stage<TInput, TOutput> stage);

        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, TOutput> func);

        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, StageOption<TInput, TOutput>,  TOutput> funcWithOption);

        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, Task<TOutput>> func);

        IPipelineSetup<TInput, TOutput> Stage<TInput, TOutput>(Func<TInput, StageOption<TInput, TOutput>, Task<TOutput>> funcWithOption);

        #endregion

        #endregion
    }
}