using AutoMapper;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {

            CreateMap<TransactionEntity, Models.Transaction>();
            CreateMap<Models.Transaction, TransactionEntity>();
            CreateMap<CreateTransactionCommand, TransactionEntity>();


        }
    }
}
