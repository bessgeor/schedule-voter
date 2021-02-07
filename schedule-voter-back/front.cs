using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schedule_voter_back
{
	public class front
	{
		private const string _route = "front";
		private static readonly DateTimeOffset _mod = DateTimeOffset.UtcNow;

		private static readonly Dictionary<string, MediaTypeHeaderValue> _mime =
			new []
			{
				(".html", "text/html"),
				(".js", "text/javascript"),
				(".css", "text/css"),
				(".map", "application/json"),
				(".json", "application/json"),
				(".txt", "text/plain"),
				(".ico", "image/vnd.microsoft.icon"),
				(".png", "image/png"),
			}.ToDictionary(
				x => x.Item1,
				x => new MediaTypeHeaderValue(x.Item2)
				{
					Encoding =
						x.Item2 == "application/json" || x.Item2.StartsWith("text")
						? Encoding.UTF8
						: null
				});


		private readonly string _contentRoot;
		private const string ConfigurationKeyApplicationRoot = "AzureWebJobsScriptRoot";
		private const string staticFilesFolder = "wwwroot";
		// The configuration is available for injection.
		// The used settings can be in any config (environment, host.json local.settings.json)
		public front(IConfiguration configuration)
		{
			_contentRoot = Path.GetFullPath(Path.Combine(
				configuration.GetValue<string>(ConfigurationKeyApplicationRoot),
				staticFilesFolder));
		}

		[FunctionName("front")]
		public async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = _route + "/{*file}")] HttpRequest req,
				ILogger log)
		{
			var path = req.Path.Value;
			var iofFront = path.IndexOf(_route) + _route.Length;
			if (iofFront < path.Length && path[iofFront] == '/')
				iofFront++;
			var file = path.Substring(iofFront);

			if (String.IsNullOrEmpty(file))
				file = "index.html";

			var extIdx = file.LastIndexOf('.');
			var ext = file.Substring(extIdx);

			var mime =
				_mime.TryGetValue(ext, out var mimeType)
				? mimeType
				: new MediaTypeHeaderValue("text/plain") { Encoding = Encoding.UTF8 };

			log.LogInformation("Static file {0} requested. Serving with mime type {1}", file, mime);

			var filePath = Path.Combine(_contentRoot, file);
			var isScriptKiddy =
				file.Contains("./")
				|| file.Contains("..")
				|| file.Contains("~")
				|| file.Contains("*")
				|| !File.Exists(filePath)
				;
			if (isScriptKiddy)
				log.LogWarning("Script kiddy detected");

			var stream =
				isScriptKiddy
				? new MemoryStream(Encoding.UTF8.GetBytes("you're a shit of a netrunner, ya know?"))
				: (Stream)File.OpenRead(filePath);

			var res = new FileStreamResult(stream, mime) { LastModified = _mod, EnableRangeProcessing = true };

			return res;
		}
	}
}
