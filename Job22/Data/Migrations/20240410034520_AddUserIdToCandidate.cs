using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job22.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Candidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Candidate");
        }
    }
}
