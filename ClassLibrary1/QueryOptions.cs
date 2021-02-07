using System;
using System.Collections.Generic;

namespace OliWorkshop.Turbo.Data
{
    public class QueryOptions
    {
        public List<Tuple<string,string>> Match { get; set; }

        public List<string> Includes { get; set; }

        public List<string> Select { get; set; }

        public List<SortClosure> Orders { get; set; }
    }

    public readonly struct SortClosure
    {
        public SortClosure(string field, Order type, bool text)
        {
            Field = field;
            Type = type;
            Text = text;
        }

        public string Field { get; }

        public Order Type { get; }

        public bool Text { get; }
    }

    public enum Order
    {
        Asc,
        Desc,
    }
}