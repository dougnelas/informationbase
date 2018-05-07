using System;

namespace ES.Info.Entities
{
    internal class Telemetry:InformationBase
    {
        public Guid Who { get; set; }
        public Guid What { get; set; }
        public DateTime When { get; set; }
        public Guid Where { get; set; }
    }
}
