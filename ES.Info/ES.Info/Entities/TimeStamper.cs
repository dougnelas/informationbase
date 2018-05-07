using System;

namespace ES.Info.Entities
{
    internal class TimeStamper:InformationBase
    {
        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }

    internal class CreationOnlyStamper : InformationBase
    {
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

    }
}
