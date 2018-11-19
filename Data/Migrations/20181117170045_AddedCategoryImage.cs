using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddedCategoryImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("0173f197-5cae-4eb0-8cc2-9dd81e761983"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("02e3f92b-7efd-489a-996c-182499bc3f42"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("03143a1d-8a89-41e0-a890-d2e457308dae"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("6d852169-4c00-461d-9ece-a5f50b2c3c84"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("82bf2d9e-e854-44bb-9340-b94a10bf97ed"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("daf3babf-9d4a-438d-b3d5-f8f435843b6e"));

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("b249a5f1-832b-4cce-bf48-b493cf8513d5"), "S" },
                    { new Guid("6e118c98-e8ed-4422-8bd1-89349243ee1e"), "M" },
                    { new Guid("78b2acf1-4c85-4dbc-a0cf-d5bccaffb466"), "L" },
                    { new Guid("25d6b8e0-dab3-4945-864d-61ca90f99e86"), "XL" },
                    { new Guid("80252659-7ac2-4607-9cbf-c23a6f08675c"), "XS" },
                    { new Guid("f29c7b8c-04a8-4a63-8f64-b4374ec07652"), "XXL" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("25d6b8e0-dab3-4945-864d-61ca90f99e86"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("6e118c98-e8ed-4422-8bd1-89349243ee1e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("78b2acf1-4c85-4dbc-a0cf-d5bccaffb466"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("80252659-7ac2-4607-9cbf-c23a6f08675c"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("b249a5f1-832b-4cce-bf48-b493cf8513d5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "Id",
                keyValue: new Guid("f29c7b8c-04a8-4a63-8f64-b4374ec07652"));

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("6d852169-4c00-461d-9ece-a5f50b2c3c84"), "S" },
                    { new Guid("82bf2d9e-e854-44bb-9340-b94a10bf97ed"), "M" },
                    { new Guid("0173f197-5cae-4eb0-8cc2-9dd81e761983"), "L" },
                    { new Guid("03143a1d-8a89-41e0-a890-d2e457308dae"), "XL" },
                    { new Guid("02e3f92b-7efd-489a-996c-182499bc3f42"), "XS" },
                    { new Guid("daf3babf-9d4a-438d-b3d5-f8f435843b6e"), "XXL" }
                });
        }
    }
}
