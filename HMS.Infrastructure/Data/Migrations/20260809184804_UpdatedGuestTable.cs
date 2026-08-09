using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedGuestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Guests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_ApplicationUserId",
                table: "Guests",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Users_ApplicationUserId",
                table: "Guests",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Users_ApplicationUserId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_ApplicationUserId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Guests");
        }
    }
}
