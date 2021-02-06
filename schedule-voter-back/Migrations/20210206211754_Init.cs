using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace schedule_voter_back.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "schedule_voter");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "schedule_voter",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    static_name = table.Column<string>(nullable: false),
                    gw2_account = table.Column<string>(nullable: false),
                    dis_account = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vote_results",
                schema: "schedule_voter",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false),
                    tourney = table.Column<DateTime>(nullable: false),
                    vote = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vote_results", x => new { x.user_id, x.tourney });
                    table.ForeignKey(
                        name: "FK_vote_results_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "schedule_voter",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_static_name_dis_account_gw2_account",
                schema: "schedule_voter",
                table: "users",
                columns: new[] { "static_name", "dis_account", "gw2_account" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vote_results",
                schema: "schedule_voter");

            migrationBuilder.DropTable(
                name: "users",
                schema: "schedule_voter");
        }
    }
}
