using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SpoonacularAPI.Data;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("FetchRecipe")]
        public async Task<ActionResult<List<RecipeSummary>>> FetchRecipes()
        {
            var temp = await _recipeService.FetchRecipe();

            if (!temp.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, temp);

            return NoContent();
        }

        // Fetches and stores recipes of specific cuisines
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetRecipeByCuisine")]
        public async Task<ActionResult<RecipeSummary>> FetchRecipeByCuisine(string Cuisine)
        {
            var temp = await _recipeService.GetRecipeByCuisine(Cuisine);

            if (!temp.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, temp);

            return Ok(temp);
        }
    }
}