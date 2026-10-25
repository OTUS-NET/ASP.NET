using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pcf.ReceivingFromPartner.DataAccess.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Partners",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                ManagerId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Partners", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Preferences",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Preferences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PartnerPromoCodeLimits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                PartnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                EndAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                CanceledAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                Limit = table.Column<int>(type: "INTEGER", nullable: false),
                IssuedCount = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PartnerPromoCodeLimits", x => x.Id);
                table.ForeignKey(
                    name: "FK_PartnerPromoCodeLimits_Partners_PartnerId",
                    column: x => x.PartnerId,
                    principalTable: "Partners",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PromoCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                ServiceInfo = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                BeginDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                EndDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                PartnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                PreferenceId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PromoCodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_PromoCodes_Partners_PartnerId",
                    column: x => x.PartnerId,
                    principalTable: "Partners",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PromoCodes_Preferences_PreferenceId",
                    column: x => x.PreferenceId,
                    principalTable: "Preferences",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PartnerPromoCodeLimits_PartnerId",
            table: "PartnerPromoCodeLimits",
            column: "PartnerId");

        migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_PartnerId",
            table: "PromoCodes",
            column: "PartnerId");

        migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_PreferenceId",
            table: "PromoCodes",
            column: "PreferenceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PartnerPromoCodeLimits");

        migrationBuilder.DropTable(
            name: "PromoCodes");

        migrationBuilder.DropTable(
            name: "Partners");

        migrationBuilder.DropTable(
            name: "Preferences");
    }
}
