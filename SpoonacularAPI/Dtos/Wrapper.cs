namespace SpoonacularAPI.Dtos
{
    public class Wrapper<T>
    {
        public T Results { get; set; }
        public int Offset { get; set; }
        public int Number { get; set; }
        public int TotalResults { get; set; }
    }
}
