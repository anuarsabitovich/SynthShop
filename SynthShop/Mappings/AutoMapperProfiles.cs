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

            CreateMap<AddProductDTO, Product>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();

            CreateMap<AddOrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<OrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<UpdateOrderItemDTO, OrderItem>().ReverseMap();

            CreateMap<AddOrderDTO, Order>().ReverseMap();
            CreateMap<UpdateOrderDTO, Order>().ReverseMap();
            CreateMap<OrderDTO, Order>().ReverseMap();
        }

    }
}
