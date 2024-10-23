using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChitTalk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUserIdToBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Blog",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Blog");
        }
    }
}
