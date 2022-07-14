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

namespace SpoonacularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly DataContext _context;
        string baseAddress = "https://api.spoonacular.com/recipes/";
        string apiKey = "?apiKey=13ff94a3949d442ba79606af5aa5dc33";
        OffsetValue Burger = new OffsetValue { Offset = 0 };
        OffsetValue Pizza = new OffsetValue { Offset = 25 };

        public RecipesController(DataContext context)
        {
            _context = context;
        }

        // Fetches and stores 50 pizza and pasta recipes from Spoonacular API
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("FetchRecipe")]
        public async Task<ActionResult<List<RecipeSummary>>> FetchRecipes()
        {
            var thing = new RecipeSummary();

            // If database is empty, store 50 recipes of pizza and pasta
            if (!_context.RecipeSummaries.Any())
            {
                #region Call for Default Pizza Recipe
                // Call for 25 Pizza Recipe
                var queryParam = "&number=5&query=Pizza&addRecipeInformation=true";
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var data = RecipeInformation.FromJson(results);

                        foreach (var item in data.Results)
                        {
                            var Cuisines = String.Join(", ", item.Cuisines);
                            var DishTypes = String.Join(", ", item.DishTypes);
                            Mapping(thing, item, Cuisines, DishTypes);

                            _context.RecipeSummaries.Add(thing);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                } 
                #endregion

                #region Call for Default Pasta Recipe
                // Call for 25 Pasta Recipe
                var queryParam2 = "&number=5&query=Pasta&addRecipeInformation=true";
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam2);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var data = RecipeInformation.FromJson(results);

                        foreach (var item in data.Results)
                        {
                            var Cuisines = String.Join(", ", item.Cuisines);
                            var DishTypes = String.Join(", ", item.DishTypes);
                            Mapping(thing, item, Cuisines, DishTypes);

                            _context.RecipeSummaries.Add(thing);
                            await _context.SaveChangesAsync();
                        }
                        return NoContent();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                } 
                #endregion
            }

            // If database is not empty, store 20 recipes of burger and pizza
            else
            {
                #region Call For Daily Pizza Recipe
                // Call for 10 Pizza Recipe
                var queryParam = "&number=1&query=Pizza&addRecipeInformation=true&offset=";
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam + Pizza.Offset);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var data = RecipeInformation.FromJson(results);

                        foreach (var item in data.Results)
                        {
                            var Cuisines = String.Join(", ", item.Cuisines);
                            var DishTypes = String.Join(", ", item.DishTypes);
                            Mapping(thing, item, Cuisines, DishTypes);

                            _context.RecipeSummaries.Add(thing);
                            await _context.SaveChangesAsync();
                        }
                        Pizza.Offset += 10;
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                }
                #endregion

                #region Call for Daily Burger Recipe
                // Call for 10 Burger Recipe
                var queryParam2 = "&number=1&query=Burger&addRecipeInformation=true&offset=";
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam2 + Burger.Offset);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var data = RecipeInformation.FromJson(results);

                        foreach (var item in data.Results)
                        {
                            var Cuisines = String.Join(", ", item.Cuisines);
                            var DishTypes = String.Join(", ", item.DishTypes);
                            Mapping(thing, item, Cuisines, DishTypes);

                            _context.RecipeSummaries.Add(thing);
                            await _context.SaveChangesAsync();
                        }
                        Burger.Offset += 10;
                        return NoContent();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                } 
                #endregion
            }
        }

        // Fetches and stores recipes of specific cuisines
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetRecipeByCuisine")]
        public async Task<ActionResult<RecipeSummary>> FetchRecipeByCuisine(string Cuisine)
        {
            var temp = await _context.RecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            //if (!temp.Any())
            //{
            //    var queryParam = "&number=1&tags=" + Cuisine.ToLower();
            //    using (var client = new HttpClient())
            //    {
            //        HttpResponseMessage getData = await client.GetAsync(baseAddress + "random" + apiKey + queryParam);

            //        if (getData.IsSuccessStatusCode)
            //        {
            //            string results = getData.Content.ReadAsStringAsync().Result;
            //            var data = JsonConvert.DeserializeObject<RandomRecipeInformation>(results);
            //            var temp2 = data.RecipeInformations.First();

            //            HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + temp2.Id + "/summary" + apiKey);

            //            if (getMoreData.IsSuccessStatusCode)
            //            {
            //                string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
            //                var moreData = JsonConvert.DeserializeObject<RecipeSummary>(moreResults);

            //                moreData.Cuisines = Cuisine;

            //                _context.RecipeSummaries.Add(moreData);
            //                await _context.SaveChangesAsync();

            //                return Ok(await _context.RecipeSummaries.FirstAsync(x => x.Id == moreData.Id));
            //            }
            //            else
            //            {
            //                return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
            //            }
            //        }
            //        else
            //        {
            //            return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
            //        }
            //    }
            //}
            //else
            //{
                var index = new Random().Next(temp.Count);
                return Ok(temp[index]);
            //}
        }

        private static void Mapping(RecipeSummary thing, Result item, string cuisines, string dishTypes)
        {
            thing.Id = (int) item.Id;
            thing.Title = item.Title;
            thing.Summary = item.Summary;
            thing.Cuisines = cuisines;
            thing.SourceUrl = item.SourceUrl;
            thing.SpoonacularSourceUrl = item.SpoonacularSourceUrl;
            thing.PricePerServing = Math.Round(item.PricePerServing/100, 2);
            thing.Servings = (int) item.Servings;
            thing.DairyFree = item.DairyFree;
            thing.DishTypes = dishTypes;
            thing.GlutenFree = item.GlutenFree;
            thing.ReadyInMinutes = (int) item.ReadyInMinutes;
            thing.Vegan = item.Vegan;
            thing.Vegetarian = item.Vegetarian;
        }
    }

    public class OffsetValue
    {
        public int Offset { get; set; }
    }
 
}