using Microsoft.EntityFrameworkCore.Migrations;
using VillaApi.Entities;

#nullable disable

namespace villa_core_api.Migrations
{
    /// <inheritdoc />
    public partial class V1_AddFieldVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Occupancy",
                table: "Villas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sqft",
                table: "Villas",
                type: "int",
                nullable: false,
                defaultValue: 0);
            
            // Clear Data in Table
            migrationBuilder.Sql("DELETE FROM Villas");
            migrationBuilder.Sql("ALTER TABLE Villas AUTO_INCREMENT = 1");
            // Insert data into table
            var faker = new Bogus.Faker<Villa>()
                .RuleFor(c => c.Name, f => "Villa " + f.Name.FirstName())
                .RuleFor(c => c.Sqft, f => f.Random.Int(10, 1000))
                .RuleFor(c => c.Occupancy, f => f.Random.Int(0, 10))
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent())
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());
            
            for (int i = 0; i < 160; i++)
            {   
                Villa generate = faker.Generate();
                migrationBuilder.InsertData(
                    table: "Villas",
                    columns: new[] { "Name", "Sqft", "Occupancy","CreatedAt", "UpdatedAt" },
                    values: new object[] { generate.Name, generate.Sqft, generate.Occupancy ,generate.CreatedAt, generate.UpdatedAt }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Occupancy",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Sqft",
                table: "Villas");
        }
    }
}
