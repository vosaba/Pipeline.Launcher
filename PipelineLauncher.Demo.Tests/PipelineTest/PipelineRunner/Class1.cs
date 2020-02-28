﻿using System;
using System.Collections.Generic;
using System.Linq;
using PipelineLauncher.Demo.Tests.Extensions;
using PipelineLauncher.Demo.Tests.Items;
using PipelineLauncher.Exceptions;
using PipelineLauncher.Stages;
using Xunit;
using Xunit.Abstractions;

namespace PipelineLauncher.Demo.Tests.PipelineTest.PipelineRunner.DemoSamples
{
    public class StageS1 : StageS<Item>
    {
        public override Item Execute(Item item)
        {
            item.Process(GetType());

            return item;
        }
    }
    public class StageS2 : StageS<Item>
    {
        private Dictionary<int, bool> _throwForItem = new Dictionary<int, bool>()
        {
            {2, false},
            //{3, false}
        };

        public override Item Execute(Item item)
        {
            if (item.Index == 2)
            {
                item.Process(GetType());

                throw new Exception("Test Exception");
            }

            return item;
        }
    }
    public class StageS3 : StageS<Item, Item2>
    {
        public override Item2 Execute(Item item)
        {
            item.Process(GetType());

            return new Item2(item);
        }
    }
    public class StageS4 : StageS<Item2, Item>
    {
        public override Item Execute(Item2 item2)
        {
            var item = item2.GetItem();

            item.Process(GetType());

            return item;
        }
    }
    public class StageS5 : StageS<Item>
    {
        public override Item Execute(Item item)
        {
            item.Process(GetType());

            return item;
        }
    }
    public class StageS6 : StageS<Item>
    {
        public override Item Execute(Item item)
        {
            item.Process(GetType());

            return item;
        }
    }
    public class StageS7 : StageS<Item>
    {
        public override Item Execute(Item item)
        {
            if (item.Index == 7)
            {
                return Remove(item);
            }

            if (item.Index == 9)
            {
                return SkipTo<StageS9>(item);
            }

            item.Process(GetType());

            return item;
        }
    }
    public class StageS8 : StageS<Item>
    {
        public override Item Execute(Item item)
        {
            item.Process(GetType());

            return item;
        }
    }
    public class StageS9 : StageS<Item, Item3>
    {
        public override Item3 Execute(Item item)
        {
            item.Process(GetType());

            return new Item3(item);
        }
    }

    public class BulkStage_1 : BulkStage<Item>
    {
        public override IEnumerable<Item> Execute(Item[] items)
        {
            foreach (var item in items)
            {
                item.Process(GetType());

                yield return item;
            }
        }
    }

    public class BulkStage_2 : BulkStage<Item>
    {
        public override IEnumerable<Item> Execute(Item[] items)
        {
            foreach (var item in items)
            {
                item.Process(GetType());

                yield return item;
            }
        }
    }
    public class BulkStage_3 : BulkStage<Item>
    {
        private bool _throwException = true;

        public override IEnumerable<Item> Execute(Item[] items)
        {
            if (_throwException)
            {
                _throwException = false;
                throw new Exception("Test Exception");
            }

            foreach (var item in items)
            {
                item.Process(GetType());

                yield return item;
            }
        }
    }
    public class BulkStage_4 : BulkStage<Item, Item2>
    {
        public override IEnumerable<Item2> Execute(Item[] items)
        {
            foreach (var item in items)
            {
                item.Process(GetType());

                yield return new Item2(item);
            }
        }
    }
    public class BulkStage_5 : BulkStage<Item2, Item>
    {
        public override IEnumerable<Item> Execute(Item2[] item2s)
        {
            foreach (var item2 in item2s)
            {
                var item = item2.GetItem();

                item.Process(GetType());

                yield return item;
            }
        }
    }

    public class DemoSamplesTests : PipelineTestBase
    {
        public DemoSamplesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ComplexArchitecture()
        {
            // Test input 6 items
            List<Item> items = MakeItemsInput(11);

            var indexesForBranch1 = new[] { 2, 5 };
            var indexesForBranch2 = new[] { 6, 8 };
            var indexesForBranch3SubBranch1 = new[] { 3, 1, 0 };
            //var indexesForBranch3SubBranch2 = new [] { 7, 9, 4 };

            // Configure stages
            var pipelineSetup = PipelineCreator
                .Stage<StageS1, Item>()
                .Branch(
                    (x => indexesForBranch1.Contains(x.Index),
                        branch => branch
                            .BulkStage<BulkStage_1>()
                            .Stage<StageS2>()),
                    (x => indexesForBranch2.Contains(x.Index),
                        branch => branch
                            .Stage<StageS3, Item2>()
                            .Stage<StageS4, Item>()),
                    (x => true,
                        branch => branch
                            .BulkStage<BulkStage_2>()
                            .Branch(
                                (x => indexesForBranch3SubBranch1.Contains(x.Index),
                                    subBranch => subBranch
                                        .BulkStage<BulkStage_3>()
                                        .Stage<StageS5>()
                                        .Stage<StageS6>()),
                                (x => true,
                                    subBranch => subBranch
                                        .Stage<StageS7>()
                                        .BulkStage<BulkStage_4, Item2>()
                                        .BulkStage<BulkStage_5, Item>()
                                        .Stage<StageS8>()))))
                .Stage<StageS9, Item3>();

            // Make pipeline from stageSetup
            var pipelineRunner = pipelineSetup.CreateAwaitable();

            pipelineRunner.SetupInstantExceptionHandler(args =>
            {
                var itemsNames = args.Items.Cast<Item>().Select(x => x.Name).ToArray();
                var message = $"Stage: {args.StageType.Name} | Items: {{ {string.Join(" }; { ", itemsNames)} }} | Exception: {args.Exception.Message}";

                WriteLine(message);
                WriteSeparator();

                if (args.StageType == typeof(BulkStage_3))
                {
                    args.Retry();
                }
            });

            pipelineRunner.ExceptionItemsReceivedEvent += args =>
            {
                var itemsNames = args.Items.Cast<Item>().Select(x => x.Name).ToArray();
                var message = $"Stage: {args.StageType.Name} | Items: {{ {string.Join(" }; { ", itemsNames)} }} | Exception: {args.Exception.Message}";

                WriteLine(message);
                WriteSeparator();
            };

            pipelineRunner.SkippedItemReceivedEvent += args =>
            {
                var item = args.Item;

                WriteSeparator();
                WriteLine($"{item} is skipped or removed from {args.StageType.Name}");
            };

            // Process items and print result
            (this, pipelineRunner).ProcessAndPrintResults(items);
        }
    }
}
