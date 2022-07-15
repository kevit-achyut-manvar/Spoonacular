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
                var temp = await _recipeService.FetchRecipe(OffsetValue.BurgerOffset, OffsetValue.PizzaOffset);

                OffsetValue.PizzaOffset = 25;
                OffsetValue.BurgerOffset = 0;

                if (!temp.Success)
                    return StatusCode(StatusCodes.Status500InternalServerError, temp);

                return Ok(temp);
            }

            var temp2 = await _recipeService.FetchRecipe(OffsetValue.BurgerOffset, OffsetValue.PizzaOffset);
            OffsetValue.BurgerOffset += 10;
            OffsetValue.PizzaOffset += 10;

            if (!temp2.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, temp2);

            return Ok(temp2);
        }

        // Fetches and stores recipes of specific cuisines
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetRecipeByCuisine")]
        public async Task<ActionResult<RecipeSummary>> FetchRecipeByCuisine(string Cuisine)
        {
            var temp = await _recipeService.GetRecipeByCuisine(Cuisine);

            if (!temp.Success)
            {
                if (temp.Message.Contains("Bad Request"))
                    return BadRequest(temp);

                return StatusCode(StatusCodes.Status500InternalServerError, temp);
            }

            return Ok(temp);
        }
    }

    public class OffsetValue
    {
        public static int BurgerOffset { get; set; } = 0;
        public static int PizzaOffset { get; set; } = 0;
    }
}