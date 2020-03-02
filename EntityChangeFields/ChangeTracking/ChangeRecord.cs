using System.Collections.Generic;

namespace EntityChangeFields.ChangeTracking
{
    public class ChangeRecord
    {
        public string Title { get; set; }
        public string TargetName { get; set; }
        public ICollection<ChangeField> Fields { get; set; }
    }

    public class ChangeField
    {
        public string Name { get; set; }
        public string OriginValue { get; set; }
        public string NewValue { get; set; }
    }
}
