using AutoMapper;
using SpoonacularAPI.Models;

namespace SpoonacularAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RecipeSummary, CuisineRecipeSummary>();
        }
    }
}
