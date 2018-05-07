using System;
using ES.Info.Entities;
using ES.Info.SearchConfiguration;
using Nest;

namespace ES.Info
{
    internal class ConnectionSettingProvider
    {

        public ConnectionSettings Get()
        {
            // You will need to change the cluster location here to point your elastic cluster
            var connectionSettings = new ConnectionSettings(new Uri("http://192.168.0.150:9200"));

            connectionSettings.DefaultFieldNameInferrer(i => i);
            connectionSettings.DefaultTypeNameInferrer(i => i.Name);

            connectionSettings.InferMappingFor<Note>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<TimeStamper>(m => m.IdProperty(p => p.ItemId));

            connectionSettings.InferMappingFor<Label>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<Value>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<Group>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<GroupMember>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<ChildGroup>(m => m.IdProperty(p => p.ItemId));

            connectionSettings.InferMappingFor<Site>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<SearchIndex>(m => m.IdProperty(p => p.ItemId));
            connectionSettings.InferMappingFor<Source>(m => m.IdProperty(p => p.ItemId));


            return connectionSettings;
        }
}
}
