using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabriniCosmetics.Migrations
{
    /// <inheritdoc />
    public partial class Added_Subproducts_To_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Products_ProductId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductID",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistDetail_Products_ProductId",
                table: "WishlistDetail");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "IsAvailability",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "WishlistDetail",
                newName: "SubproductId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistDetail_ProductId",
                table: "WishlistDetail",
                newName: "IX_WishlistDetail_SubproductId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductImages",
                newName: "SubproductId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "OrderItems",
                newName: "SubproductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductID",
                table: "OrderItems",
                newName: "IX_OrderItems_SubproductId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "CartDetails",
                newName: "SubproductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_ProductId",
                table: "CartDetails",
                newName: "IX_CartDetails_SubproductId");

            migrationBuilder.CreateTable(
                name: "Subproducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailability = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subproducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subproducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_SubproductId",
                table: "ProductImages",
                column: "SubproductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subproducts_ProductId",
                table: "Subproducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Subproducts_SubproductId",
                table: "CartDetails",
                column: "SubproductId",
                principalTable: "Subproducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Subproducts_SubproductId",
                table: "OrderItems",
                column: "SubproductId",
                principalTable: "Subproducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Subproducts_SubproductId",
                table: "ProductImages",
                column: "SubproductId",
                principalTable: "Subproducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistDetail_Subproducts_SubproductId",
                table: "WishlistDetail",
                column: "SubproductId",
                principalTable: "Subproducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Subproducts_SubproductId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Subproducts_SubproductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Subproducts_SubproductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistDetail_Subproducts_SubproductId",
                table: "WishlistDetail");

            migrationBuilder.DropTable(
                name: "Subproducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_SubproductId",
                table: "ProductImages");

            migrationBuilder.RenameColumn(
                name: "SubproductId",
                table: "WishlistDetail",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistDetail_SubproductId",
                table: "WishlistDetail",
                newName: "IX_WishlistDetail_ProductId");

            migrationBuilder.RenameColumn(
                name: "SubproductId",
                table: "ProductImages",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "SubproductId",
                table: "OrderItems",
                newName: "ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_SubproductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductID");

            migrationBuilder.RenameColumn(
                name: "SubproductId",
                table: "CartDetails",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_SubproductId",
                table: "CartDetails",
                newName: "IX_CartDetails_ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailability",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Products_ProductId",
                table: "CartDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductID",
                table: "OrderItems",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistDetail_Products_ProductId",
                table: "WishlistDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
