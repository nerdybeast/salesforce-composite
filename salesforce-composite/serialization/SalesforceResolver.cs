using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using salesforce_composite.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace salesforce_composite.serialization
{
    public class SalesforceResolver : DefaultContractResolver
    {
        private MemberInfo _memberInfo;
        private SalesforceSerialization _salesforceSerialization;

        public SalesforceResolver(SalesforceSerialization salesforceSerialization) {
            _salesforceSerialization = salesforceSerialization;
        }

        protected override IValueProvider CreateMemberValueProvider(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
            return base.CreateMemberValueProvider(memberInfo);
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            //Will contain a value if your Sobject class property has the [SalesforceName] attribute
            var salesforceNameAttribute = _memberInfo.GetCustomAttribute<SalesforceNameAttribute>();

            return salesforceNameAttribute?.SalesforcePropertyName ?? propertyName;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            return properties.Where(prop =>
            {
                //Gather all the attributes from the current property 
                IList<Attribute> attributes = prop.AttributeProvider.GetAttributes(true);

                //Returns true if none of the attributes determine that this property should be ignored from the serialization
                return attributes.All(attr => !ShouldIgnore(attr));

            }).ToList();
        }

        private bool ShouldIgnore(Attribute attr)
        {
            if(attr is SalesforceIgnoreAttribute)
            {
                return true;
            }

            if(attr is SalesforceSerializationAttribute)
            {
                var salesforceSerializationAttribute = attr as SalesforceSerializationAttribute;

                if(_salesforceSerialization == SalesforceSerialization.CREATE && !salesforceSerializationAttribute.Create)
                {
                    return true;
                }

                if (_salesforceSerialization == SalesforceSerialization.RETRIEVE && !salesforceSerializationAttribute.Read)
                {
                    return true;
                }

                if ((_salesforceSerialization == SalesforceSerialization.UPDATE || _salesforceSerialization == SalesforceSerialization.PATCH) && !salesforceSerializationAttribute.Update)
                {
                    return true;
                }

                if (_salesforceSerialization == SalesforceSerialization.DELETE && !salesforceSerializationAttribute.Delete)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
