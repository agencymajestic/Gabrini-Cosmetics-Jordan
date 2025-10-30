using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabriniCosmetics.Migrations
{
    /// <inheritdoc />
    public partial class Added_WishListGroup_To_WishList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WishListGroup",
                table: "Wishlists",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WishListGroup",
                table: "Wishlists");
        }
    }
}
