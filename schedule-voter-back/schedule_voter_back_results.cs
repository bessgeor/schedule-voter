using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


using System;
using System.Linq;
using System.Threading.Tasks;

namespace schedule_voter_back
{
	public static class schedule_voter_back_results
	{
		[FunctionName("schedule_voter_back_results")]
		public static async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Anonymous, "post", "POST", Route = "results")] HttpRequest req,
				ILogger log)
		{
			var ct = req.HttpContext.RequestAborted;
			using var db = new _Db.Db();
			await db.Database.MigrateAsync(ct);

			var body = JsonHelper.Deserialize<ResultsRequest>(req);

			var filterDate = DateTime.UtcNow.AddMinutes(14).AddSeconds(50);

			var dates =
				body.ShowPast == true
				? body.Dates
				: body.Dates.Where(x => x > filterDate).ToArray();

			var dbResult =
				await db
					.VoteResults
					.Where(x => x.User.StaticName == body.StaticName && dates.Contains(x.Tourney))
					.Select(x => new
					{
						Date = x.Tourney,
						Vote = new VoteResultView
						{
							DisAcc = x.User.DisAccount,
							Gw2Acc = x.User.Gw2Account,
							Vote = x.Vote
						}
					})
					.ToArrayAsync(ct);

			var result =
				dbResult
					.GroupBy(x => x.Date, x => x.Vote)
					.Select(x => new ResultsResponse(x.Key, x.ToArray()))
					.OrderByDescending(x => x.Votes.Sum(x => (int)x.Vote));

			return new OkObjectResult(result);
		}
	}
}
