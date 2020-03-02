using System;
using System.Collections.Generic;

namespace EntityChangeFields.ChangeTracking
{
    public class TrackingConfig<TSource>
    {
        public string Name { get; set; }
        public string CustomName { get; set; }
        public ICollection<TrackingField<TSource>> TrackingFields { get; set; }
    }

    public class TrackingField<TSource>
    {
        public string Name { get; set; }
        public string CustomName { get; set; }
        public Func<TSource, string> GetValue { get; set; }
        public Func<TSource, string> GetDescription { get; set; }
    }
}
