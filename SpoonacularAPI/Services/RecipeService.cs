using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SpoonacularAPI.Data;
using SpoonacularAPI.Models;
using SpoonacularAPI.ViewModels;

namespace SpoonacularAPI.Services
{
    public class RecipeService : IRecipeService
    {
        #region Properties
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public RecipeService(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Fetches and stores recipes in database from source API
        /// </summary>
        /// <param name="burgerOffset">The number of recipes to be skipped while requesting data from source API</param>
        /// <param name="pizzaOffset">The number of recipes to be skipped while requesting data from source API</param>
        /// <returns>Respective status code with message</returns>
        public async Task<Response<List<RecipeSummary>>> FetchRecipe(int burgerOffset, int pizzaOffset)
        {
            var baseAddress = _configuration.GetSection("SourceApi").Value;
            var apiKey = _configuration.GetSection("ApiKey").Value;

            var response = new Response<List<RecipeSummary>>();
            var recipeSummary = new RecipeSummary();

            // If database is empty, store 50 recipes of pizza and pasta
            if (!_context.RecipeSummaries.Any())
            {
                // Call for Default Pizza and Pasta Recipe
                try
                {
                    using (var client = new HttpClient())
                    {
                        var pizzaQuery = "&number=25&query=Pizza&addRecipeInformation=true";

                        HttpResponseMessage getRecipeData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pizzaQuery);

                        if (getRecipeData.IsSuccessStatusCode)
                        {
                            await SaveRecipe(recipeSummary, getRecipeData);
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching Pizza recipes. No recipes are stored.";

                            return response;
                        }

                        var pastaQuery = "&number=25&query=Pasta&addRecipeInformation=true";

                        getRecipeData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pastaQuery);

                        if (getRecipeData.IsSuccessStatusCode)
                        {
                            await SaveRecipe(recipeSummary, getRecipeData);

                            response.Message = "50 Pizza and Burger recipe successfully stored.";
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching pasta recipe. Only Pizza recipes are fetched and stored.";

                            return response;
                        }
                    }
                }
                catch (Exception e)
                {
                    response.Success = false;
                    response.Message = e.Message;

                    return response;
                }

                return response;
            }

            // If database is not empty, store 20 recipes of burger and pizza
            else
            {
                // Call For Daily Pizza and Pasta Recipe
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Call for 10 Pizza Recipe
                        var pizzaQuery = "&number=10&query=Pizza&addRecipeInformation=true&offset=";
                        HttpResponseMessage getRecipeData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pizzaQuery + pizzaOffset);

                        if (getRecipeData.IsSuccessStatusCode)
                        {
                            await SaveRecipe(recipeSummary, getRecipeData);
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching Pizza recipes. No new recipes are stored.";

                            return response;
                        }

                        // Call for 10 Burger Recipe
                        var burgerQuery = "&number=10&query=Burger&addRecipeInformation=true&offset=";
                        getRecipeData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + burgerQuery + burgerOffset);

                        if (getRecipeData.IsSuccessStatusCode)
                        {
                            string results = getRecipeData.Content.ReadAsStringAsync().Result;
                            var recipeData = RecipeInformation.FromJson(results);

                            if (recipeData.Results.Count > 0)
                            {
                                foreach (var recipe in recipeData.Results)
                                {
                                    var Cuisines = String.Join(", ", recipe.Cuisines);
                                    var DishTypes = String.Join(", ", recipe.DishTypes);
                                    Mapping(recipeSummary, recipe, Cuisines, DishTypes);

                                    _context.RecipeSummaries.Add(recipeSummary);
                                    await _context.SaveChangesAsync();
                                }

                                response.Message = "New recipes fetched and stored successfully.";
                            }
                            else
                            {
                                response.Message = "Recipes finished. All Pizza and Burger recipes are already fetched and stored.";
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching Burger recipes. New Pizza recipes are stored.";

                            return response;
                        }
                    }
                }
                catch (Exception e)
                {
                    response.Success = false;
                    response.Message = e.Message;

                    return response;
                }

                return response;
            }
        }

        /// <summary>
        ///     Gets recipe of given from database or fetches and stores recipe of given cuisine from source API
        /// </summary>
        /// <param name="Cuisine">"Cuisine" whose recipe is to be returned</param>
        /// <returns>Recipe of given cuisine</returns>
        public async Task<Response<CuisineRecipeSummary>> GetRecipeByCuisine(string Cuisine)
        {
            var baseAddress = _configuration.GetSection("SourceApi").Value;
            var apiKey = _configuration.GetSection("ApiKey").Value;

            var response = new Response<CuisineRecipeSummary>();
            var cuisine = Cuisine.ToLower();

            // Only French, Thai, Irish, Italian and Indian Cuisine are allowed
            if (!CheckCuisine(cuisine))
            {
                response.Success = false;
                response.Message = "Bad Request. Cuisine can only be from 'Indian', 'Italian', 'French', 'Irish' or 'Thai'.";

                return response;
            }

            var recipeSummaries = await _context.RecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            var cuisineRecipeSummaries = await _context.CuisineRecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();

            if (!recipeSummaries.Any()) // Checks if mentioned cuisines recipe is available in First Table
            {
                if (!cuisineRecipeSummaries.Any()) // Checks if mentioned cuisines recipe is available in Second Table
                {
                    // Call for random recipe of mentioned cuisine
                    var query = "&number=1&tags=" + cuisine;
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var cuisineRecipeSummary = new CuisineRecipeSummary();
                            HttpResponseMessage getRecipeData = await client.GetAsync(baseAddress + "random" + apiKey + query);

                            if (getRecipeData.IsSuccessStatusCode)
                            {
                                string results = getRecipeData.Content.ReadAsStringAsync().Result;
                                var recipeData = JsonConvert.DeserializeObject<RandomRecipeInformation>(results);
                                var recipe = recipeData.Recipes.First();

                                var Cuisines = String.Join(", ", recipe.Cuisines);
                                var DishTypes = String.Join(", ", recipe.DishTypes);

                                Mapping(cuisineRecipeSummary, recipe, Cuisines, DishTypes);

                                _context.CuisineRecipeSummaries.Add(cuisineRecipeSummary);
                                await _context.SaveChangesAsync();

                                response.Data = cuisineRecipeSummary;
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Error in fetching recipe. Please try again later.";
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        response.Success = false;
                        response.Message = e.Message;

                        return response;
                    }
                    return response;
                }
                else
                {
                    var index = new Random().Next(cuisineRecipeSummaries.Count);
                    response.Data = cuisineRecipeSummaries[index];

                    return response;
                }

            }
            else
            {
                var index = new Random().Next(recipeSummaries.Count);
                response.Data = _mapper.Map<CuisineRecipeSummary>(recipeSummaries[index]);

                return response;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///     Saves recipe in database after deserializing recipe data obtained from source API
        /// </summary>
        /// <param name="recipeSummary"></param>
        /// <param name="getRecipeData"></param>
        /// <returns></returns>
        private async Task SaveRecipe(RecipeSummary recipeSummary, HttpResponseMessage getRecipeData)
        {
            string results = getRecipeData.Content.ReadAsStringAsync().Result;
            var recipeData = RecipeInformation.FromJson(results);

            if (recipeData.Results.Count > 0)
            {
                foreach (var recipe in recipeData.Results)
                {
                    var Cuisines = String.Join(", ", recipe.Cuisines);
                    var DishTypes = String.Join(", ", recipe.DishTypes);
                    Mapping(recipeSummary, recipe, Cuisines, DishTypes);

                    _context.RecipeSummaries.Add(recipeSummary);
                    await _context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        ///     Check if cuisine is from one of the allowed cuisines
        /// </summary>
        /// <param name="cuisine">"Cuisine" to be checked</param>
        /// <returns>true or false</returns>
        private static bool CheckCuisine(string cuisine)
        {
            return cuisine.Equals("indian") || cuisine.Equals("irish") || cuisine.Equals("french") || cuisine.Equals("thai") || cuisine.Equals("italian");
        }

        /// <summary>
        ///     Maps RecipeInformation (View Model) to RecipeSummary (Data Model)
        /// </summary>
        /// <param name="thing">View Model object</param>
        /// <param name="item">Data Model object</param>
        /// <param name="cuisines"></param>
        /// <param name="dishTypes"></param>
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

        /// <summary>
        ///     Maps RandomRecipeInfomation (View Model) to CuisineRecipeSummary (DataModel)
        /// </summary>
        /// <param name="thing">View Model object</param>
        /// <param name="item">Data Model object</param>
        /// <param name="cuisines"></param>
        /// <param name="dishTypes"></param>
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
        #endregion
    }
}
