using System;

namespace ES.Info.Entities
{
    internal class GroupRelation:InformationBase
    {
        public Guid ParentGroupId { get; set; }
        public Guid ChildGroupId { get; set; }
        public int SortOrder { get; set; }
    }
}
