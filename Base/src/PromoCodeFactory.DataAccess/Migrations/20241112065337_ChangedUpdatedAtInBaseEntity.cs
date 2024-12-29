using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUpdatedAtInBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "roles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8470));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "promo_codes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(4240));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "promo_codes",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(3810));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "preferences",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "preferences",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "employees",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(330));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employees",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(9830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "customers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(6340));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(5890));

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(860), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(863) });

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(866), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(867) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("001a42d4-401e-4222-b0a3-f0b7c020bc2b"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1687), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1687) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("848deaf5-b346-4f7a-b903-cdb97d274e99"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1684), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1684) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("9dd84697-36d2-43f8-bdcc-1ebcd61da316"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1689), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1689) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("db29f052-9b5e-46cc-8db7-0c24f51be0a9"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1690), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(1691) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(5821), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(5824) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(5827), new DateTime(2024, 11, 12, 6, 53, 37, 389, DateTimeKind.Utc).AddTicks(5827) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8910),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8470),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "promo_codes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(4240),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "promo_codes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(3810),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(330),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(9830),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(6340),
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(5890),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(9830), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(330) });

            migrationBuilder.UpdateData(
                table: "employees",
                keyColumn: "id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 97, DateTimeKind.Utc).AddTicks(9830), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(330) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("001a42d4-401e-4222-b0a3-f0b7c020bc2b"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("848deaf5-b346-4f7a-b903-cdb97d274e99"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("9dd84697-36d2-43f8-bdcc-1ebcd61da316"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910) });

            migrationBuilder.UpdateData(
                table: "preferences",
                keyColumn: "id",
                keyValue: new Guid("db29f052-9b5e-46cc-8db7-0c24f51be0a9"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2450), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(2910) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8470), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8910) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8470), new DateTime(2024, 10, 22, 18, 19, 4, 98, DateTimeKind.Utc).AddTicks(8910) });
        }
    }
}
