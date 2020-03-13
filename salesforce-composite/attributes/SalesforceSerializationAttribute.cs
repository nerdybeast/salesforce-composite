using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite.attributes
{
    public sealed class SalesforceSerializationAttribute : Attribute
    {
        public readonly bool Create;
        public readonly bool Read;
        public readonly bool Update;
        public readonly bool Delete;

        public SalesforceSerializationAttribute(bool create = false, bool read = true, bool update = false, bool delete = false)
        {
            Create = create;
            Read = read;
            Update = update;
            Delete = delete;
        }
    }
}
