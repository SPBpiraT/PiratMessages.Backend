using AutoMapper;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Queries.GetMessageDetails
{
    public class MessageDetailsVm : IMapWith<Message>
    {
        public Guid Id { get; set; }
        public Guid DestinationUserId { get; set; }
        public string Details { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Message, MessageDetailsVm>()
                .ForMember(messageVm => messageVm.DestinationUserId,
                opt => opt.MapFrom(message => message.DestinationUserId))
                .ForMember(messageVm => messageVm.Details,
                opt => opt.MapFrom(message => message.Details))
                .ForMember(messageVm => messageVm.Id,
                opt => opt.MapFrom(message => message.Id))
                .ForMember(messageVm => messageVm.CreationDate,
                opt => opt.MapFrom(message => message.CreationDate))
                .ForMember(messageVm => messageVm.EditDate,
                opt => opt.MapFrom(message => message.EditDate));
        }
    }
}
