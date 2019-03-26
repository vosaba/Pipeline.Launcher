﻿using System;

namespace PipelineLauncher.Jobs
{
    internal class AsyncLambdaJob<TInput, TOutput> : AsyncJob<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _func;

        internal AsyncLambdaJob(Func<TInput, TOutput> func)
        {
            _func = func;
        }

        public override TOutput Execute(TInput input)
        {
            return _func(input);
        }
    }
}
