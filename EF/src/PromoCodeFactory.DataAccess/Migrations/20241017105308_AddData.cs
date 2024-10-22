using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2880));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2660));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8560),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(7230));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8270),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(6960));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(1280));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(3130));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(2900));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 793, DateTimeKind.Utc).AddTicks(6240));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 792, DateTimeKind.Utc).AddTicks(7040));

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "email", "first_name", "last_name" },
                values: new object[,]
                {
                    { new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"), "ivan_sergeev@mail.ru", "Иван", "Петров" },
                    { new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"), "marivanna@somemail.ru", "Мария", "Ивановна" }
                });

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

            migrationBuilder.InsertData(
                table: "Preferences",
                columns: new[] { "Id", "name" },
                values: new object[,]
                {
                    { new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84"), "Дети" },
                    { new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd"), "Семья" },
                    { new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c"), "Театр" }
                });

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

            migrationBuilder.InsertData(
                table: "CustomerPreferences",
                columns: new[] { "CustomerId", "PreferenceId" },
                values: new object[,]
                {
                    { new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"), new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84") },
                    { new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"), new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd") },
                    { new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"), new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CustomerPreferences",
                keyColumns: new[] { "CustomerId", "PreferenceId" },
                keyValues: new object[] { new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"), new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84") });

            migrationBuilder.DeleteData(
                table: "CustomerPreferences",
                keyColumns: new[] { "CustomerId", "PreferenceId" },
                keyValues: new object[] { new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"), new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd") });

            migrationBuilder.DeleteData(
                table: "CustomerPreferences",
                keyColumns: new[] { "CustomerId", "PreferenceId" },
                keyValues: new object[] { new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"), new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c") });

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"));

            migrationBuilder.DeleteData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("76324c47-68d2-472d-abb8-33cfa8cc0c84"));

            migrationBuilder.DeleteData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("c4bda62e-fc74-4256-a956-4760b3858cbd"));

            migrationBuilder.DeleteData(
                table: "Preferences",
                keyColumn: "Id",
                keyValue: new Guid("ef7f299f-92d7-459f-896e-078ed53ef99c"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2880),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3860));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2660),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 506, DateTimeKind.Utc).AddTicks(3680));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(7230),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(6960),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(8270));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(1480),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1670));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Preferences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 798, DateTimeKind.Utc).AddTicks(1280),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 505, DateTimeKind.Utc).AddTicks(1410));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(3130),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1600));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(2900),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 503, DateTimeKind.Utc).AddTicks(1340));

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 793, DateTimeKind.Utc).AddTicks(6240),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 499, DateTimeKind.Utc).AddTicks(9640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 23, 43, 46, 792, DateTimeKind.Utc).AddTicks(7040),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 10, 17, 10, 53, 8, 498, DateTimeKind.Utc).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(2900), new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(3130) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(2900), new DateTime(2024, 10, 16, 23, 43, 46, 796, DateTimeKind.Utc).AddTicks(3130) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2660), new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2880) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2660), new DateTime(2024, 10, 16, 23, 43, 46, 799, DateTimeKind.Utc).AddTicks(2880) });
        }
    }
}
