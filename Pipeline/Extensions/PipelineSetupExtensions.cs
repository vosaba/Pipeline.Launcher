﻿using PipelineLauncher.Abstractions.Services;
using PipelineLauncher.PipelineSetup;
using PipelineLauncher.Stages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PipelineLauncher.Extensions
{
    public static class PipelineSetupExtensions
    {
        private static IPipelineSetup<TInput, TOutput> RemoveDuplicates<TInput, TOutput>(this IPipelineSetup<TInput, TOutput> pipelineSetup, Func<TOutput, int> hashFunc = null)
        {
            var processedHash = new ConcurrentDictionary<int, byte>();

            return pipelineSetup.Stage((TOutput input, StageOption<TOutput, TOutput> stageOption) =>
            {
                var hash = hashFunc?.Invoke(input) ?? input.GetHashCode();

                return !processedHash.TryAdd(hash, byte.MinValue) ? stageOption.Remove(input) : input;
            });
        }

        private static IPipelineSetup<TInput, TOutput>RemoveDuplicatesPermanent<TInput, TOutput>(this IPipelineSetup<TInput, TOutput> pipelineSetup, Func<TOutput, int> hashFunc = null)
        {
            var processedHash = new ConcurrentDictionary<int, byte>();

            return pipelineSetup.BulkStage(inputs =>
            {
                var result = new List<TOutput>();

                foreach (var input in inputs)
                {
                    var hash = hashFunc?.Invoke(input) ?? input.GetHashCode();

                    if (processedHash.TryAdd(hash, byte.MinValue))
                    {
                        result.Add(input);
                    }
                }

                return result;
            });
        }

        public static IStageService AccessStageService<TOutput>(this IPipelineSetupSource<TOutput> pipelineSetup)
        {
            var ty = pipelineSetup.GetType();
            var pi = ty.GetProperty(nameof(PipelineCreationContext.StageService), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
            object o = pi.GetValue(pipelineSetup, null);

            return (IStageService)o;
        }

        private static IPipelineSetup<TInput, TOutput> DelayWithLock<TInput, TOutput>(this IPipelineSetup<TInput, TOutput> pipelineSetup, long millisecondsDelay)
        {
            object @lock = new object();
            DateTime performedTime = DateTime.Now;

            return pipelineSetup.Stage(input =>
            {
                lock (@lock)
                {
                    var lastPerformedTime = performedTime;
                    while (lastPerformedTime.AddMilliseconds(millisecondsDelay) > DateTime.Now)
                    { }

                    performedTime = DateTime.Now;
                    return input;
                }
            });
        }
    }
}