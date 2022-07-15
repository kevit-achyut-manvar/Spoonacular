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

        // Fetches and stores 50 pizza and pasta recipes from Spoonacular API
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
                    return StatusCode(StatusCodes.Status500InternalServerError, response;

                return Ok(response);
            }

            var alternateResponse = await _recipeService.FetchRecipe(OffsetValue.BurgerOffset, OffsetValue.PizzaOffset);
            OffsetValue.BurgerOffset += 10;
            OffsetValue.PizzaOffset += 10;

            if (!alternateResponse.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, alternateResponse);

            return Ok(alternateResponse);
        }

        // Fetches and stores recipes of specific cuisines
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

    public class OffsetValue
    {
        public static int BurgerOffset { get; set; } = 0;
        public static int PizzaOffset { get; set; } = 0;
    }
}