using System;
using System.Collections.Generic;

namespace ES.Info.Entities
{
    internal class InformationBase
    {
        private Guid? _itemId;
        public Guid ItemId
        {
            get
            {
                if (_itemId.HasValue == false)
                    _itemId = Guid.NewGuid();
                return _itemId.Value;
            }
            set
            {
                _itemId = value;
            }           
        }

        public string Title { get; set; } = string.Empty;

        private List<Guid> _values;
        public List<Guid> Values
        {
            get { return _values ?? (_values = new List<Guid>()); }
            set { _values = value; }
        }

        private List<Guid> _attachments;
        public List<Guid> Attachments
        {
            get { return _attachments ?? (_attachments = new List<Guid>()); }
            set { _attachments = value; }
        }

        internal bool IsNew = true;
        internal bool IsModified = false;
        internal bool IsDeleted = false;
    }
}
