using System;
using System.Collections.Generic;
using System.Linq;
using ES.Info.Entities;
using ES.Info.Processors;
using Nest;

namespace ES.Info.Repositories
{
    internal interface IInformationRepository
    {
        List<InformationBase> GetItemWithAttachments(Guid itemId, IEnumerable<Type> types, bool includeItemValues = false);
        void Save(InformationBase item);
        void Delete(InformationBase item);
    }

    internal class InformationRepository : IInformationRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly IAttachmentProcessor _attachmentProcessor;
        private readonly IItemMetadataProvider _itemMetadataProvider;


        public InformationRepository(
            IElasticClient elasticClient, 
            IAttachmentProcessor attachmentProcessor,
            IItemMetadataProvider itemMetadataProvider)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
            _attachmentProcessor = attachmentProcessor ?? throw new ArgumentNullException(nameof(attachmentProcessor));
            _itemMetadataProvider = itemMetadataProvider ?? throw new ArgumentNullException(nameof(itemMetadataProvider));
         }

        public List<InformationBase> GetItemWithAttachments(Guid itemId, IEnumerable<Type> types, bool includeItemValues = false)
        {
            var result = new List<InformationBase>();
            var typeNames = new List<TypeName>();
            foreach (var t in types)
            {
                typeNames.Add(TypeName.Create(t));
            }                                                                                                                                                                                                                                                                                                                                                    
            var response = _elasticClient.Search<dynamic>(s => s
                .Type(Types.Type(typeNames))
                //.ConcreteTypeSelector((h,t) => h.GetType())
                .Size(1000)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(itemId.ToString())
                        .Fields(new[] {"ItemId^10", "Attachments"}))));

            foreach (var item in response.Documents)
            {
                item.IsNew = false;
                result.Add(item);
                if (item.ItemId == itemId && includeItemValues)
                {
                    result.AddRange(_itemMetadataProvider.GetItemMetadataValues(item));
                }
            }
            return result;
        }

        public void Save(InformationBase item)
        {
            //Honor the control flags
            if (item.IsModified || item.IsNew)
            {
                item.IsModified = false;
                item.IsNew = false;
                _elasticClient.Index(item, x => x.Type(item.GetType()));
                         }
        }

        public void Delete(InformationBase item)
        {
            //Honor the control flags
            if (item.IsDeleted == false) return;

            //First remove the item from other item's attachments
            var attachedTo = _elasticClient.Search<dynamic>(s => s
                .Query(q => q
                    .Term(t => t.Field("Attachments").Value(item.ItemId))));

            foreach (InformationBase detach in attachedTo.Documents)
            {
                _attachmentProcessor.Detach(detach, item);
                Save(detach);
            }
            //Second remove the item from the index
            _elasticClient.Delete<dynamic>(item.ItemId);
        }

    }
}
