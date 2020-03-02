using System.Collections.Generic;

namespace EntityChangeFields.ChangeTracking
{
    public class ChangeRecord
    {
        public string Title { get; set; }
        public string TargetName { get; set; }
        public ChangeType Type { get; set; }
        public ICollection<ChangeField> Fields { get; set; }
    }

    public class ChangeField
    {
        public string Name { get; set; }
        public string OriginValue { get; set; }
        public string NewValue { get; set; }
    }

    public enum ChangeType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Adding = 1,
        /// <summary>
        /// 修改
        /// </summary>
        Modifying = 2,
        /// <summary>
        /// 删除
        /// </summary>
        Deleting = 3,
    }
}
