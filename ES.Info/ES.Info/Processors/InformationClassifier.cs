using System;
using ES.Info.Entities;

namespace ES.Info.Processors
{
    internal interface IInformationClassifier
    {
        void Tag(InformationBase info, Value value);
        void UnTag(InformationBase info, Value value);
    }

    internal class InformationClassifier:IInformationClassifier
    {
        public void Tag(InformationBase info, Value value)
        {
            //Metadata label values are not allowed to be tagged
            if (info is Value || info is Label) return;
            if (info.Values.Contains(value.ItemId)) return;
            info.Values.Add(value.ItemId);
            info.IsModified = true;
        }

        public void UnTag(InformationBase info, Value value)
        {
            if (info.Values.Contains(value.ItemId) == false) return;
            info.Values.Remove(value.ItemId);
            info.IsModified = true;
        }
    }
}
