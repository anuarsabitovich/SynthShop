using AutoMapper;
using SynthShop.Data.DTO;
using SynthShop.Data.Entities;

namespace SynthShop.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddCategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<UpdateCategoryDTO, Category>().ReverseMap();
            CreateMap<AddCustomerDTO, Customer>().ReverseMap();
        }

    }
}
