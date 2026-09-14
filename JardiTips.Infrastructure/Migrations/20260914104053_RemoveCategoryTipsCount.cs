using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JardiTips.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryTipsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipsCount",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipsCount",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
