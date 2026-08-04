using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JardiTips.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryInternalMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Categories",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Categories",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipsCount",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TipsCount",
                table: "Categories");
        }
    }
}
