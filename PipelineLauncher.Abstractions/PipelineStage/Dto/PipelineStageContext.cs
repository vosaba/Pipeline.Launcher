using System.Threading;

namespace PipelineLauncher.Abstractions.PipelineStage.Dto
{
    public class PipelineStageContext
    {
        public CancellationToken CancellationToken { get; }
        public ActionsSet ActionsSet { get; }

        public PipelineStageContext(CancellationToken cancellationToken, ActionsSet actionsSet)
        {
            CancellationToken = cancellationToken;
            ActionsSet = actionsSet;
        }
    }
}