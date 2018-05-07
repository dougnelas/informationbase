using ES.Info.Entities;

namespace ES.Info.Entities
{
    internal class ChildGroup:InformationBase
    {
        public int SortOrder { get; set; }
    }

    internal class RootGroup:InformationBase
    { }
}
