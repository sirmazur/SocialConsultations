using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialConsultations.Migrations
{
    /// <inheritdoc />
    public partial class create_communities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                table: "Communities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BackgroundId",
                table: "Communities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Communities",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CommunityUser",
                columns: table => new
                {
                    AdminCommunitiesId = table.Column<int>(type: "int", nullable: false),
                    AdministratorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUser", x => new { x.AdminCommunitiesId, x.AdministratorsId });
                    table.ForeignKey(
                        name: "FK_CommunityUser_Communities_AdminCommunitiesId",
                        column: x => x.AdminCommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityUser_Users_AdministratorsId",
                        column: x => x.AdministratorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityUser1",
                columns: table => new
                {
                    MemberCommunitiesId = table.Column<int>(type: "int", nullable: false),
                    MembersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityUser1", x => new { x.MemberCommunitiesId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_CommunityUser1_Communities_MemberCommunitiesId",
                        column: x => x.MemberCommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityUser1_Users_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinRequest_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinRequest_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CommunityId",
                table: "Issues",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_AvatarId",
                table: "Communities",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_BackgroundId",
                table: "Communities",
                column: "BackgroundId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUser_AdministratorsId",
                table: "CommunityUser",
                column: "AdministratorsId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityUser1_MembersId",
                table: "CommunityUser1",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinRequest_CommunityId",
                table: "JoinRequest",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinRequest_UserId",
                table: "JoinRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_FileData_AvatarId",
                table: "Communities",
                column: "AvatarId",
                principalTable: "FileData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_FileData_BackgroundId",
                table: "Communities",
                column: "BackgroundId",
                principalTable: "FileData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communities_FileData_AvatarId",
                table: "Communities");

            migrationBuilder.DropForeignKey(
                name: "FK_Communities_FileData_BackgroundId",
                table: "Communities");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Communities_CommunityId",
                table: "Issues");

            migrationBuilder.DropTable(
                name: "CommunityUser");

            migrationBuilder.DropTable(
                name: "CommunityUser1");

            migrationBuilder.DropTable(
                name: "JoinRequest");

            migrationBuilder.DropIndex(
                name: "IX_Issues_CommunityId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Communities_AvatarId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_BackgroundId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "BackgroundId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Communities");
        }
    }
}
