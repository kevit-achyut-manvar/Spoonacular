namespace SpoonacularAPI
{
    public class MyBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using(var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.GetAsync("https://localhost:7200/api/Recipes/FetchRecipe"); //Change this URL as per Project Hosting Site
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
