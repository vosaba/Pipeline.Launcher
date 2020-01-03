﻿using System;
using System.Threading;
using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Abstractions.PipelineEvents;
using PipelineLauncher.Abstractions.PipelineStage;
using PipelineLauncher.Abstractions.PipelineStage.Dto;
using PipelineLauncher.Abstractions.Services;
using PipelineLauncher.Abstractions.Stages;
using PipelineLauncher.Services;

namespace PipelineLauncher.PipelineSetup
{
    internal class PipelineSetupContext
    {
        private IStageService _stageService;
        private Action<DiagnosticItem> _diagnosticAction;
        public Func<object[], int[]> _getItemsIdentify;

        public bool TryUseDefaultServiceResolver { get; set; } = true;

        public CancellationToken CancellationToken { get; private set; }

        public IStageService StageService
        {
            get
            {
                if (_stageService == null)
                {
                    if (TryUseDefaultServiceResolver)
                    {
                        _stageService = new DefaultStageService();
                    }
                    else
                    {
                        throw new Exception($"'{nameof(IStageService)}' isn't provided, if you need to use Generic stage setups, provide service.");
                    }
                }

                return _stageService;
            }
        }

        public  PipelineSetupContext(IStageService stageService)
        {
            _stageService = stageService;
        }

        public PipelineSetupContext SetupStageService(IStageService stageService)
        {
            _stageService = stageService;
            return this;
        }

        public PipelineSetupContext SetupStageService(Func<Type, IStage> stageService)
        {
            _stageService = new DefaultLambdaStageService(stageService);
            return this;
        }

        public PipelineSetupContext SetupItemIdentify(Func<object[], int[]> getItemsIdentify)
        {
            _getItemsIdentify = getItemsIdentify;
            return this;
        }

        public PipelineSetupContext SetupDiagnosticAction(Action<DiagnosticItem> diagnosticAction)
        {
            _diagnosticAction = diagnosticAction;
            return this;
        }

        public PipelineSetupContext SetupCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            return this;
        }

        public PipelineStageContext GetPipelineStageContext(Action reExecute)
        {
            return new PipelineStageContext(
                CancellationToken, 
                reExecute != null || _diagnosticAction != null ? 
                    new ActionsSet(reExecute, _diagnosticAction, _getItemsIdentify) 
                    : null);
        }
    }
}