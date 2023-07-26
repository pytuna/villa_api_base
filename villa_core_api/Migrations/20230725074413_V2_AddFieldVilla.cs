using Microsoft.EntityFrameworkCore.Migrations;
using VillaApi.Models;

#nullable disable

namespace villa_core_api.Migrations
{
    /// <inheritdoc />
    public partial class V2_AddFieldVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amentity",
                table: "Villas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Villas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Villas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Villas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Clear Data in Table
            migrationBuilder.Sql("DELETE FROM Villas");
            migrationBuilder.Sql("ALTER TABLE Villas AUTO_INCREMENT = 1");
            // // Insert data into table
            var faker = new Bogus.Faker<Villa>()
                .RuleFor(c => c.Name, f => "Villa " + f.Name.FirstName())
                .RuleFor(c => c.Sqft, f => f.Random.Int(10, 1000))
                .RuleFor(c => c.Occupancy, f => f.Random.Int(0, 10))
                .RuleFor(c => c.Rate, f => f.Random.Int(0, 10))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(c => c.Amentity, f => f.Lorem.Sentence())
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent())
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());

            for (int i = 0; i < 40; i++)
            {
                Villa generate = faker.Generate();
                migrationBuilder.InsertData(
                    table: "Villas",
                    columns: new[] { "Name", "Sqft", "Occupancy", "CreatedAt", "UpdatedAt", "Rate", "Description", "ImageUrl", "Amentity" },
                    values: new object[]
                    {
                        generate.Name,
                        generate.Sqft,
                        generate.Occupancy ,
                        generate.CreatedAt,
                        generate.UpdatedAt,
                        generate.Rate,
                        generate.Description,
                        generate.ImageUrl, 
                        generate.Amentity,

                    }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amentity",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Villas");
        }
    }
}
