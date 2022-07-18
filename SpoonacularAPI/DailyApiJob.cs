using Quartz;

namespace SpoonacularAPI
{
    public class DailyApiJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage getData = await client.GetAsync("https://localhost:7200/api/Recipes/FetchRecipe"); //Change this URL as per Project Hosting Site
            }
        }
    }
}
