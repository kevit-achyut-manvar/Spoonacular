using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SpoonacularAPI.Data;
using SpoonacularAPI.Dtos;
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
        public RecipesController(DataContext context)
        {
            _context = context;
        }

        // Fetches and stores 50 pizza and 50 pasta recipes from Spoonacular API
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("FetchDefaultRecipe")]
        public async Task<ActionResult<List<SearchRecipe>>> FetchRecipes()
        {
            if (!_context.RecipeSummaries.Any())
            {
                var queryParam = "&number=50&query=Pizza";
                var data = new Wrapper<List<SearchRecipe>>();
                var moreData = new RecipeSummary();

                // Call for 50 Pizza Recipe
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        data = JsonConvert.DeserializeObject<Wrapper<List<SearchRecipe>>>(results);

                        foreach (var item in data.Results)
                        {
                            HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + item.Id + "/summary" + apiKey);

                            if (getMoreData.IsSuccessStatusCode)
                            {
                                string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
                                moreData = JsonConvert.DeserializeObject<RecipeSummary>(moreResults);

                                _context.RecipeSummaries.Add(moreData);
                                await _context.SaveChangesAsync();
                            }
                            _context.SearchRecipes.Add(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    } 
                }

                // Call for 50 Pasta Recipe
                var queryParam2 = "&number=50&query=Pasta";
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam2);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        data = JsonConvert.DeserializeObject<Wrapper<List<SearchRecipe>>>(results);

                        foreach (var item in data.Results)
                        {
                            HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + item.Id + "/summary" + apiKey);

                            if (getMoreData.IsSuccessStatusCode)
                            {
                                string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
                                moreData = JsonConvert.DeserializeObject<RecipeSummary>(moreResults);

                                _context.RecipeSummaries.Add(moreData);
                                await _context.SaveChangesAsync();
                            }
                            _context.SearchRecipes.Add(item);
                            await _context.SaveChangesAsync();
                        }
                        return NoContent();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                }
            }
            else
            {
                return BadRequest("Default 50 recipe already present. Use other API.");
            }
        }

        // Fetches and stores recipes of specific cuisines
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("FetchRecipeByCuisine")]
        public async Task<ActionResult<CuisineSummary>> FetchRecipeByCuisine(string Cuisine)
        {
            if (!_context.CuisineRecipes.Where(x => x.Cuisine == Cuisine).Any())
            {
                var queryParam = "&number=1&tags=" + Cuisine;
                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "random" + apiKey + queryParam);

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var data = RandomRecipeInformation.FromJson(results);
                        var temp = data.Recipes.First();

                        HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + temp.Id + "/summary" + apiKey);

                        if (getMoreData.IsSuccessStatusCode)
                        {
                            string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
                            var moreData = JsonConvert.DeserializeObject<CuisineSummary>(moreResults);

                            moreData.Cuisine = Cuisine;

                            _context.CuisineRecipes.Add(moreData);
                            await _context.SaveChangesAsync();

                            return Ok(await _context.CuisineRecipes.FirstAsync(x => x.Id == moreData.Id));
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                    }
                } 
            }
            else
            {
                var data = await _context.CuisineRecipes.Where(x => x.Cuisine == Cuisine).FirstAsync();
                return Ok(data);
            }
        }

        [HttpGet("FetchDaily")]
        public async Task<ActionResult<List<SearchRecipe>>> FetchRecipesDaily()
        {
            var queryParam = "&number=20&query=burger";
            var data = new Wrapper<List<SearchRecipe>>();
            var moreData = new RecipeSummary();

            // Call for 20 Burger Recipe
            using (var client = new HttpClient())
            {
                HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam);

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<Wrapper<List<SearchRecipe>>>(results);

                    foreach (var item in data.Results)
                    {
                        HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + item.Id + "/summary" + apiKey);

                        if (getMoreData.IsSuccessStatusCode)
                        {
                            string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
                            moreData = JsonConvert.DeserializeObject<RecipeSummary>(moreResults);

                            _context.RecipeSummaries.Add(moreData);
                            await _context.SaveChangesAsync();
                        }
                        _context.SearchRecipes.Add(item);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                }
            }

            var queryParam2 = "&number=20&query=pizza";
            // Call for 20 Pizza Recipe
            using (var client = new HttpClient())
            {
                HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + queryParam2);

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<Wrapper<List<SearchRecipe>>>(results);

                    foreach (var item in data.Results)
                    {
                        HttpResponseMessage getMoreData = await client.GetAsync(baseAddress + item.Id + "/summary" + apiKey);

                        if (getMoreData.IsSuccessStatusCode)
                        {
                            string moreResults = getMoreData.Content.ReadAsStringAsync().Result;
                            moreData = JsonConvert.DeserializeObject<RecipeSummary>(moreResults);

                            _context.RecipeSummaries.Add(moreData);
                            await _context.SaveChangesAsync();
                        }
                        _context.SearchRecipes.Add(item);
                        await _context.SaveChangesAsync();
                    }
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Please try again after some time.");
                }
            }
        }

        [HttpGet("RecipeFromDatabase")]
        public async Task<ActionResult<List<RecipeSummary>>> GetAllRecipeSummary()
        {
            return Ok(await _context.RecipeSummaries.ToListAsync());
        }
    }
}
