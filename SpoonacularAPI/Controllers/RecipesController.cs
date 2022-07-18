using Microsoft.AspNetCore.Mvc;
using SpoonacularAPI.Models;
using SpoonacularAPI.Services;

namespace SpoonacularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        } 

        /// <summary>
        ///     Fetches and stores 50 pizza and pasta recipes in database from Spoonacular API (Source API)
        /// </summary>
        /// <returns>Status Code with message. No data is returned.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("FetchRecipe")]
        public async Task<ActionResult<List<RecipeSummary>>> FetchRecipes()
        {
            if(OffsetValue.BurgerOffset == 0 && OffsetValue.PizzaOffset == 0)
            {
                var response = await _recipeService.FetchRecipe(OffsetValue.BurgerOffset, OffsetValue.PizzaOffset);

                OffsetValue.PizzaOffset = 25;
                OffsetValue.BurgerOffset = 0;

                if (!response.Success)
                    return StatusCode(StatusCodes.Status500InternalServerError, response);

                return Ok(response);
            }

            var alternateResponse = await _recipeService.FetchRecipe(OffsetValue.BurgerOffset, OffsetValue.PizzaOffset);
            OffsetValue.BurgerOffset += 10;
            OffsetValue.PizzaOffset += 10;

            if (!alternateResponse.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, alternateResponse);

            return Ok(alternateResponse);
        }

        /// <summary>
        ///     Gets recipe of given cuisine from database or fetches and stores a random recipe of given cuisine from source API
        /// </summary>
        /// <param name="Cuisine">"Cuisine" whose recipe is to be returned</param>
        /// <returns>Recipe of given cuisine</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetRecipeByCuisine")]
        public async Task<ActionResult<RecipeSummary>> FetchRecipeByCuisine(string Cuisine)
        {
            var response = await _recipeService.GetRecipeByCuisine(Cuisine);

            if (!response.Success)
            {
                if (response.Message.Contains("Bad Request"))
                    return BadRequest(response);

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return Ok(response);
        }
    }

    /// <summary>
    /// Provides offset value (recipes to be skipped)
    /// </summary>
    public class OffsetValue
    {
        public static int BurgerOffset { get; set; } = 0;
        public static int PizzaOffset { get; set; } = 0;
    }
}