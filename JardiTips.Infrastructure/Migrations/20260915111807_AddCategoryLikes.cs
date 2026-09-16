using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JardiTips.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryLikes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryLikes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLikes_CategoryId_UserId",
                table: "CategoryLikes",
                columns: new[] { "CategoryId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLikes_UserId",
                table: "CategoryLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryLikes");
        }
    }
}
