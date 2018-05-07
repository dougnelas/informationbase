using System;
using System.Collections.Generic;
using System.Linq;
using ES.Info.Entities;
using ES.Info.Repositories;

namespace ES.Info.Processors
{
    internal interface IItemMetadataProvider
    {
        List<Value> GetItemMetadataValues(InformationBase info);
        void InvalidateCache();
    }
    internal class ItemMetadataProvider:IItemMetadataProvider
    {
        private readonly IMetadataRepository _metadataRepository;
        public ItemMetadataProvider(IMetadataRepository metadataRepository)
        {
            if (metadataRepository == null)
                throw new ArgumentNullException(nameof(metadataRepository));
            _metadataRepository = metadataRepository;
        }

        private List<Value> _metadataValues;
        private List<Value> MetadataValues
        {
            get { return _metadataValues ?? (_metadataValues = _metadataRepository.GetValues()); }
        }
        public List<Value> GetItemMetadataValues(InformationBase info)
        {
            var result = new List<Value>();

            foreach(var valueId in info.Values)
            {
                var value = MetadataValues.FirstOrDefault(x => x.ItemId == valueId);
                if (value == null) continue;
                result.Add(value);
            }
            return result;
        }

        public void InvalidateCache()
        {
            _metadataValues = null;
        }
    }
}
