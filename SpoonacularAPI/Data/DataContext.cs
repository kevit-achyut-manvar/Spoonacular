using Microsoft.EntityFrameworkCore;
using SpoonacularAPI.Dtos;
using SpoonacularAPI.Models;

namespace SpoonacularAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<SearchRecipe> SearchRecipes { get; set; }
        public DbSet<RecipeSummary> RecipeSummaries { get; set; }
        public DbSet<CuisineSummary> CuisineRecipes { get; set; }
    }
}
