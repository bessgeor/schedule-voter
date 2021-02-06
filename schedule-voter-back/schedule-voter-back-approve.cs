using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace schedule_voter_back
{
	public static class schedule_voter_back_approve
	{
		private static _Db.VoteResult Map(int userId, Voting voting) => new _Db.VoteResult
		{
			UserId = userId,
			Tourney = voting.Tourney,
			Vote = voting.Vote
		};

		[FunctionName("schedule_voter_back_approve")]
		public static async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Anonymous, "put", "PUT", Route = "/approve")] HttpRequest req,
				ILogger log)
		{
			var ct = req.HttpContext.RequestAborted;
			using var db = new _Db.Db();
			await db.Database.MigrateAsync(ct);

			var body = JsonHelper.Deserialize<ApproveModel>(req);

			using var tran = await db.Database.BeginTransactionAsync(ct);

			var user =
				await db
					.Users
					.Where(x => x.StaticName == body.StaticName && x.DisAccount == body.DisAccount && x.Gw2Account == body.Gw2Account)
					.SingleOrDefaultAsync(ct);

			var noUserFound = user == null;
			if (user == null)
			{
				user = new _Db.User
				{
					StaticName = body.StaticName,
					DisAccount = body.DisAccount,
					Gw2Account = body.Gw2Account
				};
				db.Users.Add(user);
				await db.SaveChangesAsync(ct);
			}

			var dates = body.Votes.Select(x => x.Tourney);
			var minDate = dates.Min();
			var maxDate = dates.Max();

			var existingVotes =
				await db.VoteResults
					.Where(x => x.UserId == user.Id && minDate <= x.Tourney && x.Tourney <= maxDate)
					.ToArrayAsync(ct);

			var newVotes = DiffUpdateAndReturnNewVotes(existingVotes, user.Id, body.Votes);

			db.VoteResults.AddRange(newVotes);

			await db.SaveChangesAsync(ct);

			tran.Commit();

			return new OkResult();

			static IEnumerable<_Db.VoteResult> DiffUpdateAndReturnNewVotes(_Db.VoteResult[] existing, int userId, Voting[] votes)
			{
				var dict = existing.ToDictionary(x => x.Tourney);
				foreach (var vote in votes)
				{
					if (dict.TryGetValue(vote.Tourney, out var ex))
						ex.Vote = vote.Vote;
					else
						yield return Map(userId, vote);
				}
			}
		}
	}
}
