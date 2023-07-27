using AutoMapper;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;
using PFMdotnet.Models.Split;
using System.Collections.Generic;

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
            CreateMap<ReturnDTO<List<TransactionDto>>, ReturnDTO<List<Transaction>>>();
            CreateMap<ReturnDTO<List<Transaction>>, ReturnDTO<List<TransactionDto>>>();
            CreateMap<TransactionSplit, TransactionSplitDto>();

            // Categorries
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CreateCategoryCommand, Category>();


        }
    }
}
