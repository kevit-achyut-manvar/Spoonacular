using Microsoft.AspNetCore.Mvc;
using SpoonacularAPI.Models;

namespace SpoonacularAPI.Services
{
    public interface IRecipeService
    {
        Task<Response<List<RecipeSummary>>> FetchRecipe();
        Task<Response<CuisineRecipeSummary>> GetRecipeByCuisine(string Cuisine);
    }
}
