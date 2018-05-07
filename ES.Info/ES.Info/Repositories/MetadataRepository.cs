using System;
using System.Collections.Generic;
using System.Linq;
using ES.Info.Entities;
using Nest;

namespace ES.Info.Repositories
{
    internal interface IMetadataRepository
    {
        List<LabelValue> GetLabelValues();
        List<Label> GetLabels();
        List<Value> GetValues();
    }

    internal class MetadataRepository : IMetadataRepository
    {
        private readonly IElasticClient _elasticClient;

        public MetadataRepository(IElasticClient elasticClient)
        {
            if (elasticClient == null) throw new ArgumentNullException(nameof(elasticClient));
            _elasticClient = elasticClient;
        }

        public List<LabelValue> GetLabelValues()
        {
            var result = new List<LabelValue>();
            var labels = GetLabels();
            foreach (var value in GetValues())
            {
                var label = labels.FirstOrDefault(x => x.ItemId == value.Attachments.First());
                if (label == null) continue;

                var labelValue = new LabelValue
                {
                    LabelId = label.ItemId,
                    LabelText = label.Title,
                    ValueId = value.ItemId,
                    ValueText = value.Title
                };
                result.Add(labelValue);
            }
            return result.OrderBy(x => x.LabelText).ThenBy(x => x.ValueText).ToList();
        }

        public List<Label> GetLabels()
        {
            var labelResponse = _elasticClient.Search<Label>(s => s.Query(q => q.MatchAll()).Size(1000));
            return labelResponse.Documents.ToList();
        }

        public List<Value> GetValues()
        {
            var valueResponse = _elasticClient.Search<Value>(s => s.Query(q => q.MatchAll()).Size(10000));
            return valueResponse.Documents.ToList();
        }
    }
}
