using Microsoft.AspNetCore.Mvc;
using SpoonacularAPI.Models;
using SpoonacularAPI.ViewModels;

namespace SpoonacularAPI.Services
{
    public interface IRecipeService
    {
        Task<Response<List<RecipeSummary>>> FetchRecipe(int burgerOffset, int pizzaOffset);
        Task<Response<CuisineRecipeSummary>> GetRecipeByCuisine(string Cuisine);
    }
}
