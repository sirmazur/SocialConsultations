using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class fix_user_manytomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Comments_CommentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CommentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "CommentUser",
                columns: table => new
                {
                    UpvotedCommentsId = table.Column<int>(type: "int", nullable: false),
                    UpvotesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUser", x => new { x.UpvotedCommentsId, x.UpvotesId });
                    table.ForeignKey(
                        name: "FK_CommentUser_Comments_UpvotedCommentsId",
                        column: x => x.UpvotedCommentsId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentUser_Users_UpvotesId",
                        column: x => x.UpvotesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentUser_UpvotesId",
                table: "CommentUser",
                column: "UpvotesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentUser");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CommentId",
                table: "Users",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Comments_CommentId",
                table: "Users",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
