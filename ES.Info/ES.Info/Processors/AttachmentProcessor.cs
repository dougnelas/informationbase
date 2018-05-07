using System.Linq;
using ES.Info.Entities;

namespace ES.Info.Processors
{
    internal interface IAttachmentProcessor
    {
        void Attach(InformationBase info, InformationBase item);
        void Detach(InformationBase info, InformationBase item);
     }

    internal class AttachmentProcessor:IAttachmentProcessor
    {
        public void Attach(InformationBase info, InformationBase item)
        {
            if (info.Attachments.Contains(item.ItemId)) return;
            info.Attachments.Add(item.ItemId);
            info.IsModified = true;
        }

        public void Detach(InformationBase info, InformationBase item)
        {
            if (info.Attachments.Any(x => x == item.ItemId) == false) return;
            info.Attachments.Remove(item.ItemId);
            info.IsModified = true;
        }

    }
}
