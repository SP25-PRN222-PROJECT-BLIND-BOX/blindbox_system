using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindBoxShop.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddReplyReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReplyReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reply = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplyReviews_CustomerReviews_CustomerReviewsId",
                        column: x => x.CustomerReviewsId,
                        principalTable: "CustomerReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReplyReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplyReviews_CustomerReviewsId",
                table: "ReplyReviews",
                column: "CustomerReviewsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReplyReviews_UserId_CustomerReviewsId",
                table: "ReplyReviews",
                columns: new[] { "UserId", "CustomerReviewsId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplyReviews");
        }
    }
}
