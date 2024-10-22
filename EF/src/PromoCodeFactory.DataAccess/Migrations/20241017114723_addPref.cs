using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPref : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(860),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(690),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 719, DateTimeKind.Utc).AddTicks(5190),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 719, DateTimeKind.Utc).AddTicks(4920),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8270));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(8060),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(7780),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6630),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6410),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 713, DateTimeKind.Utc).AddTicks(7060),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 712, DateTimeKind.Utc).AddTicks(8100),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 712, DateTimeKind.Utc).AddTicks(8100), new DateTime(2024, 10, 17, 11, 47, 22, 713, DateTimeKind.Utc).AddTicks(7060) });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 712, DateTimeKind.Utc).AddTicks(8100), new DateTime(2024, 10, 17, 11, 47, 22, 713, DateTimeKind.Utc).AddTicks(7060) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6410), new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6630) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6410), new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6630) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(7780), new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(7780), new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(7780), new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(690), new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(860) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(690), new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(860) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(860));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 720, DateTimeKind.Utc).AddTicks(690));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8560),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 719, DateTimeKind.Utc).AddTicks(5190));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8270),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 719, DateTimeKind.Utc).AddTicks(4920));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(8060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 718, DateTimeKind.Utc).AddTicks(7780));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6630));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 716, DateTimeKind.Utc).AddTicks(6410));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 713, DateTimeKind.Utc).AddTicks(7060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 11, 47, 22, 712, DateTimeKind.Utc).AddTicks(8100));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720), new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640) });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720), new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340), new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340), new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410), new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410), new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670) });

            migrationBuilder.UpdateData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410), new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680), new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680), new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860) });
        }
    }
}
