﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipelineLauncher.Abstractions.Configurations;

namespace PipelineLauncher.Jobs
{
    internal class LambdaJob<TInput, TOutput> : Job<TInput, TOutput>
    {
        private readonly Func<IEnumerable<TInput>, Task<IEnumerable<TOutput>>> _funcAsync;
        private readonly Func<IEnumerable<TInput>, IEnumerable<TOutput>> _func;


        public LambdaJob(Func<IEnumerable<TInput>, Task<IEnumerable<TOutput>>> funcAsync)
        {
            _funcAsync = funcAsync;
        }

        public LambdaJob(Func<IEnumerable<TInput>, IEnumerable<TOutput>> func)
        {
            _func = func;
        }

        public override async Task<IEnumerable<TOutput>> ExecuteAsync(IEnumerable<TInput> input)
        {
            return _funcAsync != null ? await _funcAsync(input) : _func(input);
        }
    }
}