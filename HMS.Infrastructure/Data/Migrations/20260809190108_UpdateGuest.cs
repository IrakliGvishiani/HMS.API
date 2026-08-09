using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Guests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_Email",
                table: "Guests",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Guests_Email",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Guests");
        }
    }
}
