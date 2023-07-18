using AutoMapper;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {

            CreateMap<TransactionEntity, Transaction>();
            CreateMap<Transaction, TransactionEntity>();
            CreateMap<CreateTransactionCommand, TransactionEntity>();

            CreateMap<PagedSortedList<TransactionEntity>, PagedSortedList<Transaction>>();

        }
    }
}
