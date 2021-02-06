using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.IO;
using System.Text;

namespace schedule_voter_back
{
	public static class JsonHelper
	{
		private static readonly JsonSerializer _ser =
			JsonSerializer.Create(new JsonSerializerSettings
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				}
			});

		public static T Deserialize<T>(HttpRequest req)
		{
			using StreamReader textReader = new StreamReader(req.Body, Encoding.UTF8);
			using JsonTextReader jsonReader = new JsonTextReader(textReader);
			T r = _ser.Deserialize<T>(jsonReader);
			return r;
		}
	}
}
