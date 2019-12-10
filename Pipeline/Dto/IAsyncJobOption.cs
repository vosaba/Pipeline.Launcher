using System;
using PipelineLauncher.Abstractions.Pipeline;
using PipelineLauncher.Attributes;
using PipelineLauncher.Exceptions;

namespace PipelineLauncher.Dto
{
    public struct AsyncJobOption<TInput, TOutput> 
    {
        public TOutput Remove(TInput input)
        {
            throw new NonParamException<TOutput>(new RemoveItem<TOutput>(input, GetType()));
        }

        public TOutput Skip(TInput input)
        {
            throw new NonParamException<TOutput>(new SkipItem<TOutput>(input, GetType()));
        }

        public TOutput SkipTo<TSkipToJob>(TInput input) where TSkipToJob : IPipelineJobIn<TInput>
        {
            throw new NonParamException<TOutput>(new SkipItemTill<TOutput>(typeof(TSkipToJob), input, GetType()));
        }
    }
}