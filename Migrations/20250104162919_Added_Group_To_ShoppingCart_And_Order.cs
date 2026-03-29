using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabriniCosmetics.Migrations
{
    /// <inheritdoc />
    public partial class Added_Group_To_ShoppingCart_And_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CartGroup",
                table: "ShoppingCarts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderGroup",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartGroup",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "OrderGroup",
                table: "Orders");
        }
    }
}
