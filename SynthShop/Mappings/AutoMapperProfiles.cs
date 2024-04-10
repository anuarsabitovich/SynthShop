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
            CreateMap<CustomerDTO, Customer>().ReverseMap();
            CreateMap<UpdateCustomerDTO, Customer>().ReverseMap();
            CreateMap<AddOrderDTO, Order>().ReverseMap();
            CreateMap<AddProductDTO, Product>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
        }

    }
}
