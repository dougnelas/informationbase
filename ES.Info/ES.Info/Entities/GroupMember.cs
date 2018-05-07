namespace ES.Info.Entities
{
    internal class GroupMember:InformationBase
    {
        /// <summary>
        /// When True it will black list the member Id
        /// </summary>
        public bool ExclusionFlag { get; set; }

        /// <summary>
        /// Allow for numerical options as well initailize with intelligent defaults
        /// </summary>
        public double Weight { get; set; } = 1.0;
        public double Minimum { get; set; } = 0.0;
        public double Maximum { get; set; } = 1.0;
    }
}
