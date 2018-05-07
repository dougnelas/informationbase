using ES.Info.Entities;

namespace ES.Info.FuzzyEngine
{
    //Classifications
    // Fuzzy Linquistics
    // Fuzzy Element - Type Left,Center,Right,Singleton
    internal class FuzzyElement:InformationBase
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Slope { get; set; }
        public double Intercept { get; set; }
        public double Midpoint { get; set; }
        public double Weight { get; set; }
        public int MemoryOffset { get; set; }
    }
}
