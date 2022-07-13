namespace SpoonacularAPI.Dtos
{
    public class SearchRecipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Uri Image { get; set; }
        public string ImageType { get; set; }
    }
}
