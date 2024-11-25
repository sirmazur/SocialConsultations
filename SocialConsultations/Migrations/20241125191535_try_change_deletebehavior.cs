using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class try_change_deletebehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues");

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues");

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
