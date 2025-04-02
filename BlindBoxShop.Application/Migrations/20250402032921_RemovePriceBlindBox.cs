using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindBoxShop.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriceBlindBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "BlindBoxes");

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(7691));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(7689));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(7687));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(7674));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("1a0eb1ee-dce0-4f32-b5c9-e4066f89e74c"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1609));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("60a7be19-36e6-47c6-9f13-2d36937ea5e5"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1613));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("8c5ca06a-831e-4b6c-a4ca-436fd6aa4bee"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1614));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("93e49ce5-46a9-457f-be54-8b45e14dc6aa"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1611));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("ba4e485f-6ce5-4c6d-950c-10f3c70a7b3a"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1596));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("c5e97f5e-c2c7-4c25-8e6e-3887d76be0e9"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1601));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("d8b1e04f-7e07-4253-93d1-3ab8c5cb5a7d"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1604));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("f3f40eda-3d18-4698-b70a-b9f3d5aa9769"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1606));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("3a0e9a5b-1b1e-4c27-8c59-267a174c7b64"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1707));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("4b6fd56e-c089-4222-88e8-4cda5e37a853"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1700));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("5218b0c5-0891-477d-9ae5-d6e35e7c2c13"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1705));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("6c09ae3a-9156-4de3-9508-4ffb2c5c196e"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1698));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("8f72c4b6-78bf-4967-b5dc-c2a45a9ac8c7"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1696));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("9d781c58-cb88-4c03-ad79-ec159f4c91c6"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1703));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("c8c1a4c6-46fb-4058-b9ca-bd5c0302a276"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1709));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("f211a1f3-e4c7-4aa5-b319-48b013f92e07"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(1693));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8353));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8356));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8348));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("56792b87-5156-4959-82e3-25a12b66b267"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8341));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8345));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8350));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8333));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 515, DateTimeKind.Local).AddTicks(8338));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("68f2ac54-5118-4711-9f22-97167b5b5a9a"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(6643));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(6635));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(6638));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 2, 10, 29, 19, 516, DateTimeKind.Local).AddTicks(6627));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BlindBoxes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("6c9aa2b5-8cef-4621-b526-d94b08c17e46"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7922));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7920));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7918));

            migrationBuilder.UpdateData(
                table: "BlindBoxCategories",
                keyColumn: "Id",
                keyValue: new Guid("c8c3ec17-0a76-49d0-b274-994d15848f39"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(7905));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("1a0eb1ee-dce0-4f32-b5c9-e4066f89e74c"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2575));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("60a7be19-36e6-47c6-9f13-2d36937ea5e5"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2580));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("8c5ca06a-831e-4b6c-a4ca-436fd6aa4bee"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2581));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("93e49ce5-46a9-457f-be54-8b45e14dc6aa"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2577));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("ba4e485f-6ce5-4c6d-950c-10f3c70a7b3a"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2550));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("c5e97f5e-c2c7-4c25-8e6e-3887d76be0e9"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2559));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("d8b1e04f-7e07-4253-93d1-3ab8c5cb5a7d"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2561));

            migrationBuilder.UpdateData(
                table: "BlindBoxImages",
                keyColumn: "Id",
                keyValue: new Guid("f3f40eda-3d18-4698-b70a-b9f3d5aa9769"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2573));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("3a0e9a5b-1b1e-4c27-8c59-267a174c7b64"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2685));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("4b6fd56e-c089-4222-88e8-4cda5e37a853"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2678));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("5218b0c5-0891-477d-9ae5-d6e35e7c2c13"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2682));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("6c09ae3a-9156-4de3-9508-4ffb2c5c196e"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2674));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("8f72c4b6-78bf-4967-b5dc-c2a45a9ac8c7"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2671));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("9d781c58-cb88-4c03-ad79-ec159f4c91c6"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2680));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("c8c1a4c6-46fb-4058-b9ca-bd5c0302a276"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2687));

            migrationBuilder.UpdateData(
                table: "BlindBoxPriceHistories",
                keyColumn: "Id",
                keyValue: new Guid("f211a1f3-e4c7-4aa5-b319-48b013f92e07"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(2668));

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8613), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8617), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("3db50dc1-b3aa-4088-b083-d8823235120b"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8607), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("56792b87-5156-4959-82e3-25a12b66b267"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8589), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8604), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("6b34d818-8e04-4d63-9c40-2aeb68a60a90"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8611), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("7594c261-b8d9-43a0-a2ea-095214afc2a9"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8582), 0m });

            migrationBuilder.UpdateData(
                table: "BlindBoxes",
                keyColumn: "Id",
                keyValue: new Guid("8109eb24-4086-42a3-9d20-8e07a321b905"),
                columns: new[] { "CreatedAt", "Price" },
                values: new object[] { new DateTime(2025, 3, 30, 3, 14, 17, 247, DateTimeKind.Local).AddTicks(8586), 0m });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("68f2ac54-5118-4711-9f22-97167b5b5a9a"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7403));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("9a47fcb2-4910-4589-baea-6f8698c9ceab"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7391));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7392));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: new Guid("d2e1d185-02c9-479c-ae8b-0031a447389f"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 30, 3, 14, 17, 248, DateTimeKind.Local).AddTicks(7386));
        }
    }
}
