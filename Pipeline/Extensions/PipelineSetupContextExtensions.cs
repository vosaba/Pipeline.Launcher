﻿using System;
using PipelineLauncher.Abstractions.Services;
using PipelineLauncher.PipelineSetup;

namespace PipelineLauncher.Extensions
{
    public static class PipelineSetupContextExtensions
    {
        public static IPipelineSetup<TInput, TNextOutput> ExtensionContext<TInput, TOutput, TNextOutput>(this IPipelineSetup<TInput, TOutput> pipelineSetup, Func<IPipelineSetupOut<TOutput>, IPipelineSetupOut<TNextOutput>> extension)
        {
            return (IPipelineSetup<TInput, TNextOutput>)extension(pipelineSetup);
        }

        public static IPipelineSetupOut<TCast> Cast<TOutput, TCast>(this IPipelineSetupOut<TOutput> pipelineSetup)
            where TCast : class
        {
            return pipelineSetup.Stage(output => output as TCast);
        }

        public static IJobService AccessJobService<TOutput>(this IPipelineSetupOut<TOutput> pipelineSetup)
        {
            var ty = pipelineSetup.GetType();
            var pi = ty.GetProperty("JobService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
            object o = pi.GetValue(pipelineSetup, null);

            return (IJobService)o;
        }
    }
}