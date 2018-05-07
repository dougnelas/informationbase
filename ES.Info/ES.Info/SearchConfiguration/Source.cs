using System;
using System.Collections.Generic;
using ES.Info.Entities;

namespace ES.Info.SearchConfiguration
{
    internal class Source:InformationBase
    {
        /// <classifications>
        /// Source has the following classifications
        ///     SearchConfig.SourceType
        ///     SearchConfig.HostIndexCluster
        ///     SearchConfig.IndexColor
        ///     SearchConfig.Language
        /// </classifications>

        public string Url { get; set; }

        private List<Guid> _destinationIndices;
        public List<Guid> DestinationIndices {
            get => _destinationIndices ?? (_destinationIndices = new List<Guid>());
            set => _destinationIndices = value;
        }
    }

    internal class Index : InformationBase
    {
        /// <information>
        /// Index uses the SearchConfig.Index group
        ///     SearchConfig.HostIndexCluster
        ///     SearchConfig.IndexColor
        ///     SearchCongig.Language
        /// </information>
       public string IndexAlias { get; set; }
    }
}
