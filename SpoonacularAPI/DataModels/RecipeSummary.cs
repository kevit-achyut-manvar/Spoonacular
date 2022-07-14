namespace SpoonacularAPI.Models
{
    public class RecipeSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Cuisines { get; set; }
        public string DishTypes { get; set; }
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
        public bool GlutenFree { get; set; }
        public bool DairyFree { get; set; }
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public double PricePerServing { get; set; }
        public Uri SourceUrl { get; set; }
        public Uri SpoonacularSourceUrl { get; set; } 
    }
}
