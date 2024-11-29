using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class joinrequesttryfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequest_Communities_CommunityId",
                table: "JoinRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequest_Users_UserId",
                table: "JoinRequest");

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "JoinRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequest_Communities_CommunityId",
                table: "JoinRequest",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequest_Users_UserId",
                table: "JoinRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequest_Communities_CommunityId",
                table: "JoinRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequest_Users_UserId",
                table: "JoinRequest");

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "JoinRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequest_Communities_CommunityId",
                table: "JoinRequest",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequest_Users_UserId",
                table: "JoinRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
