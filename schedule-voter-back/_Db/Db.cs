using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace schedule_voter_back._Db
{
	class Db : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<VoteResult> VoteResults { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Host=localhost;Port=5432;Database=schedule_voter_db;Username=schedule_voter_db;Password=AsdasafASDasdasdASDsa2312@#!qd1@#@WS");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.HasDefaultSchema("schedule_voter");
			modelBuilder.Entity<User>(users =>
			{
				users.ToTable("users").HasKey(x => x.Id);

				users.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().UseNpgsqlSerialColumn();
				users.Property(x => x.StaticName).HasColumnName("static_name").IsRequired();
				users.Property(x => x.DisAccount).HasColumnName("dis_account").IsRequired();
				users.Property(x => x.Gw2Account).HasColumnName("gw2_account").IsRequired();

				users.HasIndex(x => new { x.StaticName, x.DisAccount, x.Gw2Account }).IsUnique();

				users.HasMany(x => x.Votes).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
			});
			modelBuilder.Entity<VoteResult>(vr =>
			{
				vr.ToTable("vote_results").HasKey(x => new { x.UserId, x.Tourney });

				vr.Property(x => x.UserId).HasColumnName("user_id");
				vr.Property(x => x.Tourney).HasColumnName("tourney");
				vr.Property(x => x.Vote).HasColumnName("vote");
			});
		}
	}
}
