using System;
using System.Linq;
using ES.Info.Entities;
using ES.Info.Processors;
using ES.Info.Repositories;
using ES.Info.SearchConfiguration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace ES.Info.UnitTests
{
    [TestClass]
    public class MainTalkDemos
    {
        [TestMethod]
        public void Start_With_A_Simple_Note()
        {

            //Connect to elastic
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(connection.Get().DefaultIndex("es.info"));

            //wire up information repository and the attacher
            var attacher = new AttachmentProcessor();
            var infoRepo = new InformationRepository(
                elastic, attacher, 
                new ItemMetadataProvider(new MetadataRepository(elastic)));
            
            //create a Note
            var note = new Note
            {
                ItemId = new Guid("3d9da9e7-5362-4432-b2f8-1da21bc5e17b"),
                NoteBody = "Hello Elastic{ON} 2017 from information!"
            };
            //Let's timestamp it
            var timeStamp = new TimeStamper
            {
                Created = DateTime.UtcNow,
                CreatedBy = "doug"
            };

            //hook note and timestamp together
            attacher.Attach(timeStamp, note);

            //save the items
            infoRepo.Save(note);
            infoRepo.Save(timeStamp);
        }

        [TestMethod]
        public void Returning_an_object_graph()
        {
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get().DefaultIndex("es*"));

            var response = elastic.Search<dynamic>(s => s.Query(q => q.MatchAll())
                .Type(
                    Types.Type(
                        typeof(Note), 
                        typeof(Label), 
                        typeof(Value))));

        }

        [TestMethod]
        public void Create_Host_Index_Cluster_Metadata()
        {

            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get().DefaultIndex("es.info.metadata"));

            var attacher = new AttachmentProcessor();
            var infoRepo = new InformationRepository(
                elastic, attacher,
                new ItemMetadataProvider(new MetadataRepository(elastic)));

            var label = new Label {
                Title = "SearchConfig.HostElasticCluster",
                ItemId = new Guid("c0271af8-32b3-4098-9f60-76e2ac48841f") };

            var labelDescription = new Note {
                NoteBody = "Elastic hosts name formats to allow connections to the elastic cluster."};
            attacher.Attach(labelDescription, label);

            var value1 = new Value {
                Title = "elasticindexV2_{0}_es.com"};
             attacher.Attach(value1, label);

            var value2 = new Value
            {
                Title = "elasticindexV5_{0}_es.com",
                ItemId = new Guid("8820432d-1b28-4f1c-bb70-9a1d0d4972fd")
            };

            attacher.Attach(value2, label);

            //Commit changes
            infoRepo.Save(label);
            infoRepo.Save(value1);
            infoRepo.Save(value2);
            infoRepo.Save(labelDescription);
        }

        [TestMethod]
        public void Create_Index_Color_Metadata()
        {
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get().DefaultIndex("es.info.metadata"));

            var attacher = new AttachmentProcessor();
            var infoRepo = new InformationRepository(
                elastic, attacher,
                new ItemMetadataProvider(new MetadataRepository(elastic)));

            var label = new Label {
                Title = "SearchConfig.IndexColor",
                ItemId = new Guid("c985d59f-6298-437b-a886-3255da8291be") };
            var labelDescription = new Note {
                NoteBody = "Index colors allow a seperate indices.  These are denoted 'Red' or 'Blue'." };

            attacher.Attach(labelDescription, label);

            var value1 = new Value
            {
                Title = "Red",
                ItemId = new Guid("c4caf9c3-3b75-46c1-b8c2-96f48fbe2f04")
            };
            //values and labels are cross attached 
            attacher.Attach(value1, label);

            var value2 = new Value { Title = "Blue" };
            attacher.Attach(value2, label);

            //Commit changes
            infoRepo.Save(label);
            infoRepo.Save(value1);
            infoRepo.Save(value2);
            infoRepo.Save(labelDescription);
        }

        [TestMethod]
        public void Create_Index_Language_Metadata()
        {
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get().DefaultIndex("es.info.metadata"));

            var attacher = new AttachmentProcessor();
            var metadataRepo = new MetadataRepository(elastic);
            var infoRepo = new InformationRepository(elastic, attacher, new ItemMetadataProvider(metadataRepo));

            var label = new Label {
                Title = "SearchConfig.Language",
                ItemId = new Guid("e12dd9e4-db07-4c3f-8e02-b1c0c4dd8022") };

            var labelDescription = new Note { NoteBody = "Metadata language." };

            attacher.Attach(labelDescription, label);

            var value1 = new Value
            {
                Title = "en",
                ItemId = new Guid("de7932b9-a0b6-4f2f-ac45-0315ea841d65")
            };
            attacher.Attach(value1, label);

            var value2 = new Value { Title = "es" };
            attacher.Attach(value2, label);

            //Commit changes
            infoRepo.Save(label);
            infoRepo.Save(value1);
            infoRepo.Save(value2);
            infoRepo.Save(labelDescription);
        }

        [TestMethod]
        public void Create_SearchConfig_Index_MetadataGroup()
        {
            //connect to elastic
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get().DefaultIndex("es.info.metadata"));

            //wire up repositories and attacher
            var attacher = new AttachmentProcessor();
            var metadataRepo = new MetadataRepository(elastic);
            var infoRepo = new InformationRepository(elastic, attacher, new ItemMetadataProvider(metadataRepo));

            //simple metadata lookup
            var metadataLookup = metadataRepo.GetLabelValues();
            var metadataValues = metadataRepo.GetValues();
            var metadataLabels = metadataRepo.GetLabels();

            //first start with the group
            var group = new Group
            {
                Title = "Site Search Index Connection Settings",
                GroupFunction = "SearchIndex",
                GroupOwner = "SearchConfig",
                ItemId = new Guid("9c60f30c-edc4-4a30-b583-9bc9b9b82eda")
            };

            //we are going to use three labels (host, color, language)
            var hostLabel = metadataLabels.First(x => x
                .ItemId == new Guid("c0271af8-32b3-4098-9f60-76e2ac48841f"));
            var colorLabel = metadataLabels.First(x => x
                .ItemId == new Guid("c985d59f-6298-437b-a886-3255da8291be"));
            var langLabel = metadataLabels.First(x => x
                .ItemId == new Guid("e12dd9e4-db07-4c3f-8e02-b1c0c4dd8022"));

            //create the host group member and create object graph
            var hostMember = new GroupMember
            {
                ExclusionFlag = false,
                Title = "Elastic Cluster Hosts"
            };
            attacher.Attach(hostMember, group);
            attacher.Attach(hostLabel, group);
            attacher.Attach(hostLabel, hostMember);

            //Attach values to group for each Label
            foreach (var valueId in metadataLookup.Where(x => x
                .LabelId == hostLabel.ItemId).Select(x => x.ValueId))
            {
                var value = metadataValues.FirstOrDefault(x => x.ItemId == valueId);
                attacher.Attach(value, group);
                infoRepo.Save(value);
            } 

            //color group member
            var colorMember = new GroupMember
            {
                ExclusionFlag = false,
                Title = "Search Index Color"
            };

            attacher.Attach(colorMember, group);
            attacher.Attach(colorLabel, group);
            attacher.Attach(colorLabel, colorMember);
            foreach (var valueId in metadataLookup.Where(x => x
                .LabelId == colorLabel.ItemId).Select(x => x.ValueId))
            {
                var value = metadataValues.FirstOrDefault(x => x.ItemId == valueId);
                attacher.Attach(value, group);
                infoRepo.Save(value);
            }

            //language group member
            var langMember = new GroupMember
            {
                ExclusionFlag = false,
                Title = "Search Index Language"
            };
            attacher.Attach(langMember, group);
            attacher.Attach(langLabel, group);
            attacher.Attach(langLabel, langMember);
            foreach (var valueId in metadataLookup.Where(x => x
                .LabelId == langLabel.ItemId).Select(x => x.ValueId))
            {
                var value = metadataValues.FirstOrDefault(x => x.ItemId == valueId);
                attacher.Attach(value, group);
                infoRepo.Save(value);
            }

            //save everything that was touched
            infoRepo.Save(group);
            infoRepo.Save(hostMember);
            infoRepo.Save(hostLabel);
            infoRepo.Save(colorMember);
            infoRepo.Save(colorLabel);
            infoRepo.Save(langMember);
            infoRepo.Save(langLabel);
        }

        [TestMethod]
        public void Get_Search_Index_Object_Graph()
        {
            var connection = new ConnectionSettingProvider();
            var elastic = new ElasticClient(
                connection.Get()
                    .DefaultIndex("gbs.info*"));
            var metadataRepo = new MetadataRepository(elastic);

            var infoRepo = new InformationRepository(elastic, new AttachmentProcessor(), new ItemMetadataProvider(metadataRepo));

            var result = infoRepo.GetItemWithAttachments(
                new Guid("62169a15-d2a1-4741-a8c4-c9d4f7f08f7c"), 
                new []{
                    typeof(SearchIndex),
                    typeof(Site)
                }, true);

            var temp = "testing";

            result = infoRepo.GetItemWithAttachments(
                new Guid("62169a15-d2a1-4741-a8c4-c9d4f7f08f7c"),
                new[] { typeof(SearchIndex), typeof(Site) }, true);
}

        [TestMethod]
        public void Add_Site_With_SearchIndex()
        {
            var connection = new ConnectionSettingProvider();

            //Working across indices
            var searchConfigElastic = new ElasticClient(
                connection.Get()
                    .DefaultIndex("es.info.searchconfig"));

            var metadataElastic = new ElasticClient(
                connection.Get()
                    .DefaultIndex("es.info.metadata"));

            var attachmentProcessor = new AttachmentProcessor();
            var tagger = new InformationClassifier();
            var metadataRepo = new MetadataRepository(metadataElastic);
            var itemMetadataProvider = new ItemMetadataProvider(metadataRepo);
 
            var searchConfigInfo = new InformationRepository(searchConfigElastic, attachmentProcessor, itemMetadataProvider );
            var metadataInfo = new InformationRepository(metadataElastic, attachmentProcessor, itemMetadataProvider);

            //Get metadata values defined for the group
            var groupValues =  metadataInfo.GetItemWithAttachments(
                new Guid("9c60f30c-edc4-4a30-b583-9bc9b9b82eda"), 
                new[] { typeof(Value) });


            //Define a site
            var site = new Site {
                ItemId = new Guid("c667e190-6177-4248-9bbf-dd27633c6ed5"),
                Title = "informationcentral.com",
                IsNew = true };

            searchConfigInfo.Save(site);

            //Get the desired metadata values for the index
            // Cluster A , English, Red
            var indexHost = groupValues.First(x => 
                x.ItemId == new Guid("8820432d-1b28-4f1c-bb70-9a1d0d4972fd"));
            var indexLanguage = groupValues.First(x => 
                x.ItemId == new Guid("de7932b9-a0b6-4f2f-ac45-0315ea841d65"));
            var indexColor = groupValues.First(x => 
                x.ItemId == new Guid("c4caf9c3-3b75-46c1-b8c2-96f48fbe2f04"));

            //Define a search index for the site
            var index = new SearchIndex {
                ItemId = new Guid("62169a15-d2a1-4741-a8c4-c9d4f7f08f7c"),
                Title = "IA Red Cluster A",
                IndexAlias = string.Format("xxx.site.{0}.{1}.{2}", 
                    indexLanguage.Title.ToLower(), 
                    site.ItemId, 
                    indexColor.Title.ToLower()) };

            tagger.Tag(index, (Value)indexHost);
            tagger.Tag(index, (Value)indexLanguage);
            tagger.Tag(index, (Value)indexColor);
              
            //Attach index to site and metadata values
            attachmentProcessor.Attach(site, index);

            //Save the site and search index
            searchConfigInfo.Save(site);
            searchConfigInfo.Save(index);
        }

        [TestMethod]
        public void CreateParentChildGroups()
        {
            var connection = new ConnectionSettingProvider();
            var metadataElastic = new ElasticClient(
                connection.Get()
                    .DefaultIndex("es.info.metadata"));

            var attachmentProcessor = new AttachmentProcessor();

            var metadataRepo = new MetadataRepository(metadataElastic);
            var metadataInfo = new InformationRepository(metadataElastic, attachmentProcessor, new ItemMetadataProvider(metadataRepo));


            var childGroup1 = new ChildGroup
            {
                SortOrder = 1,
                Title = "child 1"
            };

            var childGroup2 = new ChildGroup
            {
                SortOrder = 2,
                Title = "child 2"
            };

            var grandChild = new ChildGroup
            {
                SortOrder = 1,
                Title = "grandchild"
            };

            var rootGroup = new RootGroup
            {
                Title = "root"
            };

            attachmentProcessor.Attach(childGroup1, rootGroup);
            attachmentProcessor.Attach(childGroup2, rootGroup);
            attachmentProcessor.Attach(grandChild, childGroup1);

            attachmentProcessor.Attach(grandChild, rootGroup);
            attachmentProcessor.Attach(childGroup1, rootGroup);
            attachmentProcessor.Attach(childGroup2, rootGroup);

            metadataInfo.Save(rootGroup);
            metadataInfo.Save(childGroup1);
            metadataInfo.Save(childGroup2);
            metadataInfo.Save(grandChild);
        }
    }   

}
