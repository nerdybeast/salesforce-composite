using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace salesforce_composite.serialization
{
	public class SobjectCreationConverter<T> : CustomCreationConverter<Sobject> where T : Sobject, new()
	{
		public override bool CanRead => base.CanRead;

		public override bool CanWrite => base.CanWrite;

		public override bool CanConvert(Type objectType)
		{
			return base.CanConvert(objectType);
		}

		public override Sobject Create(Type objectType)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			// JProperty attributesProperty = jobject.Property("attributes");
			// JObject attributesJobject = attributesProperty.Value as JObject;
			// var propertyType = attributesJobject.Property("type");
			
			// Type t = TypeSystem.FindTypeByName(propertyType.Value.ToString());
			// dynamic instance = Activator.CreateInstance(t);

			// dynamic instance = Activator.CreateInstance(typeof(Sobject));

			var sobject = new T();
			serializer.Populate(jobject.CreateReader(), sobject);
			return sobject;

			//return base.ReadJson(reader, objectType, existingValue, serializer);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			base.WriteJson(writer, value, serializer);
		}
	}
}