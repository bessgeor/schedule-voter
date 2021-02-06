using System.Collections.Generic;

#nullable enable

namespace schedule_voter_back._Db
{
	public class User
	{
		public int Id { get; set; }
		public string StaticName { get; set; }
		public string Gw2Account { get; set; }
		public string DisAccount { get; set; }

		public virtual ICollection<VoteResult> Votes { get; set; } = new List<VoteResult>();
	}
}