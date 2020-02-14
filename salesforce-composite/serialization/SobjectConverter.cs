using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;

namespace salesforce_composite.serialization
{
    public class SobjectConverter : CustomCreationConverter<Sobject>
    {
        public override Sobject Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        
    }
}
