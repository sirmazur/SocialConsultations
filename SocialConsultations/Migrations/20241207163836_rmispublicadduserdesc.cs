using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class rmispublicadduserdesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Communities");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Communities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
