using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VillaApi.Models;

#nullable disable

namespace villa_core_api.Migrations
{
    /// <inheritdoc />
    public partial class V0_Init_DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Villas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
            
            migrationBuilder.Sql("DELETE FROM Villas");
            
            migrationBuilder.Sql("ALTER TABLE Villas AUTO_INCREMENT = 1");
            // Insert data into table
            var faker = new Bogus.Faker<Villa>()
                .RuleFor(c => c.Name, f => "Villa " + f.Name.FirstName())
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent())
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());
            
            for (int i = 0; i < 160; i++)
            {   
                Villa generate = faker.Generate();
                migrationBuilder.InsertData(
                    table: "Villas",
                    columns: new[] { "Name", "CreatedAt", "UpdatedAt" },
                    values: new object[] { generate.Name, generate.CreatedAt, generate.UpdatedAt }
                );
            }

        }

        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Villas");
        }
    }
}
