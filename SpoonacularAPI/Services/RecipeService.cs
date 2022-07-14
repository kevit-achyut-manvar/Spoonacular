using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SpoonacularAPI.Data;
using SpoonacularAPI.Models;
using SpoonacularAPI.ViewModels;

namespace SpoonacularAPI.Services
{
    public class RecipeService : IRecipeService
    {
        string baseAddress = "https://api.spoonacular.com/recipes/";
        string apiKey = "?apiKey=13ff94a3949d442ba79606af5aa5dc33";
        OffsetValue Burger = new OffsetValue { Offset = 0 };
        OffsetValue Pizza = new OffsetValue { Offset = 25 };

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public RecipeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<List<RecipeSummary>>> FetchRecipe()
        {
            var response = new Response<List<RecipeSummary>>();
            var thing = new RecipeSummary();

            // If database is empty, store 50 recipes of pizza and pasta
            if (!_context.RecipeSummaries.Any())
            {
                #region Call for Default Pizza and Pasta Recipe
                // Call for 25 Pizza Recipe
                using (var client = new HttpClient())
                {
                    var pizzaQuery = "&number=25&query=Pizza&addRecipeInformation=true";

                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pizzaQuery);

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
                        response.Success = false;
                        response.Message = "Error in fetching Pizza recipes. No recipes are stored.";

                        return response;
                    }

                    var pastaQuery = "&number=25&query=Pasta&addRecipeInformation=true";

                    getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pastaQuery);

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
                        response.Success = false;
                        response.Message = "Error in fetching pasta recipe. Only Pizza recipes are fetched and stored.";

                        return response;
                    }
                }
                #endregion
                return response;
            }

            // If database is not empty, store 20 recipes of burger and pizza
            else
            {
                #region Call For Daily Pizza and Pasta Recipe
                using (var client = new HttpClient())
                {
                    // Call for 10 Pizza Recipe
                    var pizzaQuery = "&number=10&query=Pizza&addRecipeInformation=true&offset=";
                    HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pizzaQuery + Pizza.Offset);

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
                        response.Success = false;
                        response.Message = "Error in fetching Pizza recipes. No new recipes are stored.";

                        return response;
                    }

                    // Call for 10 Burger Recipe
                    var burgerQuery = "&number=10&query=Burger&addRecipeInformation=true&offset=";
                    getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + burgerQuery + Burger.Offset);

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
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Error in fetching Burger recipes. New Pizza recipes are stored.";

                        return response;
                    }
                }
                #endregion
                return response;
            }
        }

        public async Task<Response<CuisineRecipeSummary>> GetRecipeByCuisine(string Cuisine)
        {
            var response = new Response<CuisineRecipeSummary>();

            var temp = await _context.RecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            var temp2 = await _context.CuisineRecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            if (!temp.Any())
            {
                if (!temp2.Any())
                {
                    var queryParam = "&number=1&tags=" + Cuisine.ToLower();
                    using (var client = new HttpClient())
                    {
                        var thing = new CuisineRecipeSummary();
                        HttpResponseMessage getData = await client.GetAsync(baseAddress + "random" + apiKey + queryParam);

                        if (getData.IsSuccessStatusCode)
                        {
                            string results = getData.Content.ReadAsStringAsync().Result;
                            var data = JsonConvert.DeserializeObject<RandomRecipeInformation>(results);
                            var item = data.Recipes.First();

                            var Cuisines = String.Join(", ", item.Cuisines);
                            var DishTypes = String.Join(", ", item.DishTypes);

                            Mapping(thing, item, Cuisines, DishTypes);

                            _context.CuisineRecipeSummaries.Add(thing);
                            await _context.SaveChangesAsync();

                            response.Data = thing;
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching recipe. Please try again later.";
                        }

                    }
                    return response;
                }
                else
                {
                    var index = new Random().Next(temp2.Count);
                    response.Data = temp2[index];

                    return response;
                }
                
            }
            else
            {
                var index = new Random().Next(temp.Count);
                response.Data = _mapper.Map<CuisineRecipeSummary>(temp[index]);

                return response;
            }
        }

        private static void Mapping(RecipeSummary thing, Result item, string cuisines, string dishTypes)
        {
            thing.Id = (int)item.Id;
            thing.Title = item.Title;
            thing.Summary = item.Summary;
            thing.Cuisines = cuisines;
            thing.SourceUrl = item.SourceUrl;
            thing.SpoonacularSourceUrl = item.SpoonacularSourceUrl;
            thing.PricePerServing = Math.Round(item.PricePerServing / 100, 2);
            thing.Servings = (int)item.Servings;
            thing.DairyFree = item.DairyFree;
            thing.DishTypes = dishTypes;
            thing.GlutenFree = item.GlutenFree;
            thing.ReadyInMinutes = (int)item.ReadyInMinutes;
            thing.Vegan = item.Vegan;
            thing.Vegetarian = item.Vegetarian;
        }
        private static void Mapping(CuisineRecipeSummary thing, Recipe item, string cuisines, string dishTypes)
        {
            thing.Id = (int)item.Id;
            thing.Title = item.Title;
            thing.Summary = item.Summary;
            thing.Cuisines = cuisines;
            thing.SourceUrl = item.SourceUrl;
            thing.SpoonacularSourceUrl = item.SpoonacularSourceUrl;
            thing.PricePerServing = Math.Round(item.PricePerServing / 100, 2);
            thing.Servings = (int)item.Servings;
            thing.DairyFree = item.DairyFree;
            thing.DishTypes = dishTypes;
            thing.GlutenFree = item.GlutenFree;
            thing.ReadyInMinutes = (int)item.ReadyInMinutes;
            thing.Vegan = item.Vegan;
            thing.Vegetarian = item.Vegetarian;
        }
    }

    public class OffsetValue
    {
        public int Offset { get; set; }
    }
}
