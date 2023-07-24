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
            CreateMap<TransactionEntity, Transaction>();
            CreateMap<Transaction, TransactionEntity>();
            CreateMap<CreateTransactionCommand, TransactionEntity>();

            CreateMap<TransactionPagedList<TransactionEntity>, TransactionPagedList<Transaction>>();
            CreateMap<ReturnDTO<Transaction>, ReturnDTO<TransactionEntity>>();
            CreateMap<ReturnDTO<TransactionEntity>, ReturnDTO<Transaction>>();

            // Categorries
            CreateMap<CategoryEntity, Category>();
            CreateMap<Category, CategoryEntity>();
            CreateMap<CreateCategoryCommand, CategoryEntity>();


        }
    }
}
