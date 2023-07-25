using AutoMapper;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Helpers.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            // Transactions
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionDto, Transaction>();
            CreateMap<CreateTransactionCommand, Transaction>();

            CreateMap<TransactionPagedList<Transaction>, TransactionPagedList<TransactionDto>>();
            CreateMap<ReturnDTO<TransactionDto>, ReturnDTO<Transaction>>();
            CreateMap<ReturnDTO<Transaction>, ReturnDTO<TransactionDto>>();

            // Categorries
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CreateCategoryCommand, Category>();


        }
    }
}
