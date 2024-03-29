﻿using System.Collections.Generic;
using System.Linq;
using PipelineLauncher.Abstractions.Dto;
using PipelineLauncher.Demo.Tests.Extensions;
using PipelineLauncher.Demo.Tests.Items;
using PipelineLauncher.Demo.Tests.PipelineTest;
using PipelineLauncher.Demo.Tests.Stages.Bulk;
using PipelineLauncher.Demo.Tests.Stages.Single;
using PipelineLauncher.Stages;
using Xunit;
using Xunit.Abstractions;

namespace PipelineLauncher.Demo.Tests.PipelineSetup.AwaitablePipelineRunner
{
    public class BranchStagesTests : PipelineTestBase
    {
        public BranchStagesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Stages_Branch()
        {
            // Test input 6 items
            List<Item> items = MakeItemsInput(1);

            // Configure stages
            var pipelineSetup = PipelineCreator
                .Prepare<Item>()
                .BulkStage<BulkStage>(x => PredicateResult.Skip)
                //.Stage((Item x, StageOption<Item,Item> options) =>
                //{
                //    if (x.Index == 6)
                //    {
                //        return options.SkipTo<Stage_5>(x);
                //    }

                //    if (x.Index == 7)
                //    {
                //        return options.SkipTo<Stage_5>(x);
                //    }

                //    return x;
                //})
                .Branch(
                    (x => x.Index == 80,
                        branch => branch
                            .Stage<Stage_1>()
                            .Branch(
                                (x => x.Index > 1,
                                    subBranchSetup => subBranchSetup
                                        .Stage<Stage_3>()),
                                (x => true,
                                    subBranchSetup => subBranchSetup
                                        .Stage<Stage_2>()))),
                    (x => true,
                        branch => branch));
                //.Stage<Stage_Conditional>()
                //.Stage<Stage_2>();
                //.Stage<Stage_5>();

            // Make pipeline from stageSetup
            var pipelineRunner = pipelineSetup.CreateAwaitable();

            pipelineRunner.DiagnosticEvent += diagnosticItem =>
            {
                var itemsNames = diagnosticItem.Items.Cast<Item>().Select(x => x.Name).ToArray();
                var message =
                    $"Stage: {diagnosticItem.StageType.Name} | Items: {{ {string.Join(" }; { ", itemsNames)} }} | State: {diagnosticItem.State}";

                if (!string.IsNullOrEmpty(diagnosticItem.Message))
                {
                    message += $" | Message: {diagnosticItem.Message}";
                }

                //WriteLine(message);
            };

            //Process items and print result
            (this, pipelineRunner).ProcessAndPrintResults(items);

            // Process items and print result
            //(this, pipelineRunner).ProcessAndPrintResults(items);
        }
    }
}
