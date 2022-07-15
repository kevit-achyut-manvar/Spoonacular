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
        string baseAddress = "https://api.spoonacular.com/recipes/";
        string apiKey = "?apiKey=e4e9f26fa7b24612bd1ba4af5f651332";

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RecipeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<List<RecipeSummary>>> FetchRecipe(int burgerOffset, int pizzaOffset)
        {
            var response = new Response<List<RecipeSummary>>();
            var returnItem = new RecipeSummary();

            // If database is empty, store 50 recipes of pizza and pasta
            if (!_context.RecipeSummaries.Any())
            {
                #region Call for Default Pizza and Pasta Recipe
                // Call for 25 Pizza Recipe
                try
                {
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
                                Mapping(returnItem, item, Cuisines, DishTypes);

                                _context.RecipeSummaries.Add(returnItem); 
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
                                Mapping(returnItem, item, Cuisines, DishTypes);

                                _context.RecipeSummaries.Add(returnItem);
                                await _context.SaveChangesAsync();
                            }

                            response.Message = " 50 Pizza and Burger recipe successfully stored.";
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
                #endregion
                return response;
            }

            // If database is not empty, store 20 recipes of burger and pizza
            else
            {
                #region Call For Daily Pizza and Pasta Recipe
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Call for 10 Pizza Recipe
                        var pizzaQuery = "&number=10&query=Pizza&addRecipeInformation=true&offset=";
                        HttpResponseMessage getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + pizzaQuery + pizzaOffset);

                        if (getData.IsSuccessStatusCode)
                        {
                            string results = getData.Content.ReadAsStringAsync().Result;
                            var data = RecipeInformation.FromJson(results);

                            if (data.Results.Count > 0)
                            {
                                foreach (var item in data.Results)
                                {
                                    var Cuisines = String.Join(", ", item.Cuisines);
                                    var DishTypes = String.Join(", ", item.DishTypes);
                                    Mapping(returnItem, item, Cuisines, DishTypes);

                                    _context.RecipeSummaries.Add(returnItem);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error in fetching Pizza recipes. No new recipes are stored.";

                            return response;
                        }

                        // Call for 10 Burger Recipe
                        var burgerQuery = "&number=10&query=Burger&addRecipeInformation=true&offset=";
                        getData = await client.GetAsync(baseAddress + "complexSearch" + apiKey + burgerQuery + burgerOffset);

                        if (getData.IsSuccessStatusCode)
                        {
                            string results = getData.Content.ReadAsStringAsync().Result;
                            var data = RecipeInformation.FromJson(results);

                            if (data.Results.Count > 0)
                            {
                                foreach (var item in data.Results)
                                {
                                    var Cuisines = String.Join(", ", item.Cuisines);
                                    var DishTypes = String.Join(", ", item.DishTypes);
                                    Mapping(returnItem, item, Cuisines, DishTypes);

                                    _context.RecipeSummaries.Add(returnItem);
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
                #endregion
                return response;
            }
        }

        public async Task<Response<CuisineRecipeSummary>> GetRecipeByCuisine(string Cuisine)
        {
            var response = new Response<CuisineRecipeSummary>();
            var cuisine = Cuisine.ToLower();

            // Only French, Thai, Irish, Italian and Indian Cuisine are allowed
            if (!CheckCuisine(cuisine))
            {
                response.Success = false;
                response.Message = "Bad Request. Cuisine can only be from 'Indian', 'Italian', 'French', 'Irish' or 'Thai'.";

                return response;
            }

            var firstData = await _context.RecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            var secondData = await _context.CuisineRecipeSummaries.Where(x => x.Cuisines.Contains(Cuisine)).ToListAsync();
            if (!firstData.Any()) // Checks if mentioned cuisines recipe is available in First Table
            {
                if (!secondData.Any()) // Checks if mentioned cuisines recipe is available in Second Table
                {
                    // Call for random recipe of mentioned cuisine
                    var queryParam = "&number=1&tags=" + cuisine;
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var returnItem = new CuisineRecipeSummary();
                            HttpResponseMessage getData = await client.GetAsync(baseAddress + "random" + apiKey + queryParam);

                            if (getData.IsSuccessStatusCode)
                            {
                                string results = getData.Content.ReadAsStringAsync().Result;
                                var data = JsonConvert.DeserializeObject<RandomRecipeInformation>(results);
                                var item = data.Recipes.First();

                                var Cuisines = String.Join(", ", item.Cuisines);
                                var DishTypes = String.Join(", ", item.DishTypes);

                                Mapping(returnItem, item, Cuisines, DishTypes);

                                _context.CuisineRecipeSummaries.Add(returnItem);
                                await _context.SaveChangesAsync();

                                response.Data = returnItem;
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
                    var index = new Random().Next(secondData.Count);
                    response.Data = secondData[index];

                    return response;
                }

            }
            else
            {
                var index = new Random().Next(firstData.Count);
                response.Data = _mapper.Map<CuisineRecipeSummary>(firstData[index]);

                return response;
            }
        }

        // Check if cuisine is from one of the allowed cuisines
        private static bool CheckCuisine(string cuisine)
        {
            return cuisine.Equals("indian") || cuisine.Equals("irish") || cuisine.Equals("french") || cuisine.Equals("thai") || cuisine.Equals("italian");
        }

        // Maps RecipeInformation (View Model) to RecipeSummary (Data Model)
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

        // Maps RandomRecipeInfomation (View Model) to CuisineRecipeSummary (DataModel)
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
}
