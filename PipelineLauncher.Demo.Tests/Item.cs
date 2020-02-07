﻿using System.Collections.Generic;

namespace PipelineLauncher.Demo.Tests
{
    public class Item
    {
        public string Value { get; set; }
        public List<int> ProcessedBy { get; set; }
        public Item(string value)
        {
            Value = value;
            ProcessedBy = new List<int>();
        }

        public override string ToString()
        {
            return $"Handler by:  '{{{string.Join("}, {", ProcessedBy.ToArray())}}}'; Result: '{Value}'";
        }
    }

    public class Item2 : Item {
        public Item2(string value) : base(value)
        {
        }
    }
}
