using System;
using System.Collections.Generic;

namespace EntityChangeFields.FieldChanges
{
    public class ChangeConfig<TSource>
    {
        public string Name { get; set; }
        public ICollection<TrackingField<TSource>> TrackingFields { get; set; }
    }

    public class TrackingField<TSource>
    {
        public string Name { get; set; }
        public Func<TSource, string> GetValue { get; set; }
        public Func<TSource, string> GetDescription { get; set; }
    }
}
