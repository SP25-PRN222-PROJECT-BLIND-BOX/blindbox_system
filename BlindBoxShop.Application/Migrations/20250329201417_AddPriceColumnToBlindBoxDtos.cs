using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlindBoxShop.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceColumnToBlindBoxDtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlindBoxPriceHistories_BlindBoxes_BlindBoxId",
                table: "BlindBoxPriceHistories");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BlindBoxes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BlindBoxCategories",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BlindBoxCategories",
                type: "nvarchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "BlindBoxItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlindBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsSecret = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindBoxItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindBoxItems_BlindBoxes_BlindBoxId",
                        column: x => x.BlindBoxId,
                        principalTable: "BlindBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BlindBoxCategories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7922), "Các mô hình động vật và thú cưng đáng yêu.", "Thú cưng" },
                    { new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7920), "Nhân vật từ các bộ phim nổi tiếng của Hollywood và thế giới.", "Phim" },
                    { new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7918), "Các nhân vật được yêu thích từ thế giới game.", "Game" },
                    { new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7905), "Bộ sưu tập BlindBox dựa trên các nhân vật anime nổi tiếng.", "Anime" }
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "Barcode", "CreatedAt", "CurrentTotalBlindBox", "Name", "TotalBlindBox", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("68f2ac54-5118-4711-9f22-97167b5b5a9a"), "PKG-STD-004", new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7403), 20, "Hộp Giới hạn", 20, "Standard", null },
                    { new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"), "PKG-STD-002", new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7391), 20, "Hộp Premium", 20, "Standard", null },
                    { new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"), "PKG-STD-003", new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7392), 20, "Hộp Deluxe", 20, "Standard", null },
                    { new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"), "PKG-STD-001", new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7386), 20, "Hộp Tiêu Chuẩn", 20, "Standard", null }
                });

            migrationBuilder.InsertData(
                table: "BlindBoxes",
                columns: new[] { "Id", "BlindBoxCategoryId", "CreatedAt", "Description", "Name", "PackageId", "Price", "Probability", "Rarity", "Status", "TotalRatingStar", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"), new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8613), "Mô hình BlindBox mèo con đáng yêu với nhiều kiểu dáng khác nhau.", "Mèo con dễ thương", new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"), 0m, 25f, "Common", "Available", 4.9f, null },
                    { new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"), new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8617), "Mô hình BlindBox chó Shiba Inu nổi tiếng với nhiều tư thế vui nhộn.", "Chó Shiba Inu", new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"), 0m, 7f, "Uncommon", "Sold_Out", 4.6f, null },
                    { new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"), new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8607), "Mô hình BlindBox Iron Man từ Marvel Cinematic Universe.", "Iron Man", new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"), 0m, 8f, "Uncommon", "Available", 4.7f, null },
                    { new Guid("56792b87-5156-4959-82e3-25a12b66b267"), new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8589), "Mô hình BlindBox Mario từ series game Super Mario Bros của Nintendo.", "Mario", new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"), 0m, 20f, "Common", "Available", 4.2f, null },
                    { new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"), new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8604), "Mô hình BlindBox Kratos từ series game God of War.", "Kratos", new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"), 0m, 10f, "Uncommon", "Available", 4.8f, null },
                    { new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"), new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8611), "Mô hình BlindBox Darth Vader từ Star Wars.", "Darth Vader", new Guid("68f2ac54-5118-4711-9f22-97167b5b5a9a"), 0m, 3f, "Rare", "Coming_Soon", 0f, null },
                    { new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"), new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8582), "Mô hình BlindBox Naruto Uzumaki từ series anime/manga Naruto.", "Naruto Uzumaki", new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"), 0m, 15f, "Common", "Available", 4.5f, null },
                    { new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"), new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"), new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8586), "Mô hình BlindBox Goku trong trạng thái Super Saiyan từ Dragon Ball Z.", "Goku Super Saiyan", new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"), 0m, 5f, "Rare", "Available", 5f, null }
                });

            migrationBuilder.InsertData(
                table: "BlindBoxImages",
                columns: new[] { "Id", "BlindBoxId", "CreatedAt", "ImageUrl" },
                values: new object[,]
                {
                    { new Guid("1a0eb1ee-dce0-4f32-b5c9-e4066f89e74c"), new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2575), "https://i.imgur.com/X1ekcDk.png" },
                    { new Guid("60a7be19-36e6-47c6-9f13-2d36937ea5e5"), new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2580), "https://i.imgur.com/4fOQeNR.png" },
                    { new Guid("8c5ca06a-831e-4b6c-a4ca-436fd6aa4bee"), new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2581), "https://i.imgur.com/Dt0vpyi.png" },
                    { new Guid("93e49ce5-46a9-457f-be54-8b45e14dc6aa"), new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2577), "https://i.imgur.com/Gr83jxH.png" },
                    { new Guid("ba4e485f-6ce5-4c6d-950c-10f3c70a7b3a"), new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2550), "https://i.imgur.com/7H4fftM.png" },
                    { new Guid("c5e97f5e-c2c7-4c25-8e6e-3887d76be0e9"), new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2559), "https://i.imgur.com/L5xfBcB.png" },
                    { new Guid("d8b1e04f-7e07-4253-93d1-3ab8c5cb5a7d"), new Guid("56792b87-5156-4959-82e3-25a12b66b267"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2561), "https://i.imgur.com/CQJnDLt.png" },
                    { new Guid("f3f40eda-3d18-4698-b70a-b9f3d5aa9769"), new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2573), "https://i.imgur.com/Mk40l4L.png" }
                });

            migrationBuilder.InsertData(
                table: "BlindBoxPriceHistories",
                columns: new[] { "Id", "BlindBoxId", "CreatedAt", "DefaultPrice", "Price" },
                values: new object[,]
                {
                    { new Guid("3a0e9a5b-1b1e-4c27-8c59-267a174c7b64"), new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2685), 100000m, 100000m },
                    { new Guid("4b6fd56e-c089-4222-88e8-4cda5e37a853"), new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2678), 280000m, 280000m },
                    { new Guid("5218b0c5-0891-477d-9ae5-d6e35e7c2c13"), new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2682), 500000m, 500000m },
                    { new Guid("6c09ae3a-9156-4de3-9508-4ffb2c5c196e"), new Guid("56792b87-5156-4959-82e3-25a12b66b267"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2674), 120000m, 120000m },
                    { new Guid("8f72c4b6-78bf-4967-b5dc-c2a45a9ac8c7"), new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2671), 350000m, 350000m },
                    { new Guid("9d781c58-cb88-4c03-ad79-ec159f4c91c6"), new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2680), 320000m, 320000m },
                    { new Guid("c8c1a4c6-46fb-4058-b9ca-bd5c0302a276"), new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2687), 250000m, 250000m },
                    { new Guid("f211a1f3-e4c7-4aa5-b319-48b013f92e07"), new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"), new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2668), 150000m, 150000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlindBoxCategories_Name",
                table: "BlindBoxCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlindBoxItems_BlindBoxId",
                table: "BlindBoxItems",
                column: "BlindBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlindBoxPriceHistories_BlindBoxes_BlindBoxId",
                table: "BlindBoxPriceHistories",
                column: "BlindBoxId",
                principalTable: "BlindBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlindBoxPriceHistories_BlindBoxes_BlindBoxId",
                table: "BlindBoxPriceHistories");

            migrationBuilder.DropTable(
                name: "BlindBoxItems");

            migrationBuilder.DropIndex(
                name: "IX_BlindBoxCategories_Name",
                table: "BlindBoxCategories");

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("1a0eb1ee-dce0-4f32-b5c9-e4066f89e74c"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("60a7be19-36e6-47c6-9f13-2d36937ea5e5"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("8c5ca06a-831e-4b6c-a4ca-436fd6aa4bee"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("93e49ce5-46a9-457f-be54-8b45e14dc6aa"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("ba4e485f-6ce5-4c6d-950c-10f3c70a7b3a"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("c5e97f5e-c2c7-4c25-8e6e-3887d76be0e9"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("d8b1e04f-7e07-4253-93d1-3ab8c5cb5a7d"));

            migrationBuilder.DeleteData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("f3f40eda-3d18-4698-b70a-b9f3d5aa9769"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("3a0e9a5b-1b1e-4c27-8c59-267a174c7b64"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("4b6fd56e-c089-4222-88e8-4cda5e37a853"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("5218b0c5-0891-477d-9ae5-d6e35e7c2c13"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("6c09ae3a-9156-4de3-9508-4ffb2c5c196e"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("8f72c4b6-78bf-4967-b5dc-c2a45a9ac8c7"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("9d781c58-cb88-4c03-ad79-ec159f4c91c6"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("c8c1a4c6-46fb-4058-b9ca-bd5c0302a276"));

            migrationBuilder.DeleteData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("f211a1f3-e4c7-4aa5-b319-48b013f92e07"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("56792b87-5156-4959-82e3-25a12b66b267"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"));

            migrationBuilder.DeleteData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"));

            migrationBuilder.DeleteData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"));

            migrationBuilder.DeleteData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"));

            migrationBuilder.DeleteData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"));

            migrationBuilder.DeleteData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"));

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("68f2ac54-5118-4711-9f22-97167b5b5a9a"));

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"));

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"));

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"));

            migrationBuilder.DropColumn(
                name: "Price",
                table: "BlindBoxes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BlindBoxCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BlindBoxCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)");

            migrationBuilder.AddForeignKey(
                name: "FK_BlindBoxPriceHistories_BlindBoxes_BlindBoxId",
                table: "BlindBoxPriceHistories",
                column: "BlindBoxId",
                principalTable: "BlindBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
