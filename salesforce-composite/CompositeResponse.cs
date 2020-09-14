using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using salesforce_composite.serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace salesforce_composite
{
	public class CompositeSubrequestResult
	{
		///// <summary>
		///// Empty constructor for use by Newtonsoft
		///// </summary>
		//[JsonConstructor]
		//public CompositeSubrequestResult()
		//{

		//}

		//public CompositeSubrequestResult(T body, CompositeSubrequestResult<string> compositeSubrequestResult)
		//{
		//    Body = body;
		//    HttpHeaders = compositeSubrequestResult.HttpHeaders;
		//    HttpStatusCode = compositeSubrequestResult.HttpStatusCode;
		//    ReferenceId = compositeSubrequestResult.ReferenceId;
		//}

		[JsonProperty("body")]
		public JObject RawBody { get; set; }
		
		public Dictionary<string, string> HttpHeaders { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }
		public string ReferenceId { get; set; }

		public T Body<T>() where T : class {

			if(RawBody == null) {
				return null; //Maybe throw an exception here ???
			}

			//return RawBody.ToObject<T>();
			return JsonConvert.DeserializeObject<T>(RawBody.ToString(), new SobjectConverter());
			// return JsonConvert.DeserializeObject<T>(RawBody.ToString(), new SobjectCreationConverter());
		}
	}

	//public class CompositeSubrequestResultBase
	//{
	//    //public object Body { get; set; }
	//    //public string Body { get; set; }
	//    public Dictionary<string, string> HttpHeaders { get; set; }
	//    public HttpStatusCode HttpStatusCode { get; set; }
	//    public string ReferenceId { get; set; }
	//}

	//public class CompositeSubrequestResultOfString : CompositeSubrequestResultBase
	//{
	//    public string Body { get; set; }
	//}

	//public class CompositeSubrequestResultOfT<T> : CompositeSubrequestResultBase where T : class
	//{
	//    public T Body { get; set; }
	//}
}
