using Microsoft.EntityFrameworkCore;
using SpoonacularAPI.Models;

namespace SpoonacularAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<RecipeSummary> RecipeSummaries { get; set; }
    }
}
