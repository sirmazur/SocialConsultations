using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues");

            migrationBuilder.DropTable(
                name: "Polls");

            migrationBuilder.AddColumn<int>(
                name: "SolutionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "Issues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentStateEndDate",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IssueStatus",
                table: "Issues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "FileData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SolutionId",
                table: "FileData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SolutionId",
                table: "Users",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileData_IssueId",
                table: "FileData",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_FileData_SolutionId",
                table: "FileData",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IssueId",
                table: "Comments",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_IssueId",
                table: "Solutions",
                column: "IssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Issues_IssueId",
                table: "Comments",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_Solutions_SolutionId",
                table: "FileData",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Solutions_SolutionId",
                table: "Users",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Issues_IssueId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_AuthorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FileData_Issues_IssueId",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_FileData_Solutions_SolutionId",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Solutions_SolutionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Users_SolutionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_FileData_IssueId",
                table: "FileData");

            migrationBuilder.DropIndex(
                name: "IX_FileData_SolutionId",
                table: "FileData");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_IssueId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "SolutionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "CurrentStateEndDate",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "IssueStatus",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "SolutionId",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "Issues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");
        }
    }
}
