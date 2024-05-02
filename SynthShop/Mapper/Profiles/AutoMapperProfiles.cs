using AutoMapper;
using SynthShop.Domain.Entities;
using SynthShop.DTO;


namespace SynthShop.Mapper.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddCategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<UpdateCategoryDTO, Category>().ReverseMap();

            CreateMap<AddCustomerDTO, User>().ReverseMap();
            CreateMap<CustomerDTO, User>().ReverseMap();
            CreateMap<UpdateCustomerDTO, User>().ReverseMap();

            CreateMap<AddProductDTO, Product>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();

            CreateMap<AddOrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<OrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<UpdateOrderItemDTO, OrderItem>().ReverseMap();

            CreateMap<AddOrderDTO, Order>().ReverseMap();
            CreateMap<UpdateOrderDTO, Order>().ReverseMap();
            CreateMap<OrderDTO, Order>().ReverseMap();

            CreateMap<BasketDTO, Basket>().ReverseMap();
            CreateMap<AddBasketItemDTO, BasketItem>().ReverseMap();

            CreateMap<RegistrationRequest, User>()
                .ForMember(dest=> dest.UserName, src => src.MapFrom(x => x.Email));
        }

    }
}
