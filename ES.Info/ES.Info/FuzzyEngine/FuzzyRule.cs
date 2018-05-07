using ES.Info.Entities;

namespace ES.Info.FuzzyEngine
{
    //Classification - Linquistic
    internal class FuzzyRule:InformationBase
    {
        public int SortOrder { get; set; }
        public double Weight { get; set; }
        public double Membership { get; set; }
    }
}
