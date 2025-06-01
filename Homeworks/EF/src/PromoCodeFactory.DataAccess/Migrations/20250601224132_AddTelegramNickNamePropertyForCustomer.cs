using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramNickNamePropertyForCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramNickName",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                column: "TelegramNickName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("de687431-e400-41a9-a0bd-c484b0d977dc"),
                column: "TelegramNickName",
                value: null);

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("4605293a-64d2-48a0-bcc7-ad250fae1e20"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 1, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(5034), new DateTime(2025, 6, 21, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(5035) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("92a6e829-ec25-43a0-9a28-c9988370160a"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 8, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(4930), new DateTime(2025, 7, 1, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(4938) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("d840538c-80db-4953-b6ee-51b02e03f710"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 7, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(5051), new DateTime(2025, 7, 11, 22, 41, 32, 356, DateTimeKind.Utc).AddTicks(5052) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramNickName",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("4605293a-64d2-48a0-bcc7-ad250fae1e20"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 1, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6496), new DateTime(2025, 6, 21, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6496) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("92a6e829-ec25-43a0-9a28-c9988370160a"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 8, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6453), new DateTime(2025, 7, 1, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6461) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("d840538c-80db-4953-b6ee-51b02e03f710"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2025, 6, 7, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6514), new DateTime(2025, 7, 11, 22, 25, 6, 422, DateTimeKind.Utc).AddTicks(6514) });
        }
    }
}
