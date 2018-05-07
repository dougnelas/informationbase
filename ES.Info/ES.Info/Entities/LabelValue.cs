using System;

namespace ES.Info.Entities
{

    internal class Label : InformationBase
    {}

    internal class Value : InformationBase
    {}

    internal class LabelValue
    {
        public Guid LabelId { get; set; }
        public string LabelText { get; set; }
        public Guid ValueId { get; set; }
        public string ValueText { get; set; }
    }

}
