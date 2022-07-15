using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpoonacularAPI.Migrations
{
    public partial class CuisineRecipeSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuisineRecipeSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cuisines = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DishTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vegetarian = table.Column<bool>(type: "bit", nullable: false),
                    Vegan = table.Column<bool>(type: "bit", nullable: false),
                    GlutenFree = table.Column<bool>(type: "bit", nullable: false),
                    DairyFree = table.Column<bool>(type: "bit", nullable: false),
                    ReadyInMinutes = table.Column<int>(type: "int", nullable: false),
                    Servings = table.Column<int>(type: "int", nullable: false),
                    PricePerServing = table.Column<double>(type: "float", nullable: false),
                    SourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpoonacularSourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuisineRecipeSummaries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuisineRecipeSummaries");
        }
    }
}
