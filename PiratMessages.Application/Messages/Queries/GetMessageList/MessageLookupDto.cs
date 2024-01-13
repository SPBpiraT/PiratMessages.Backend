using AutoMapper;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Queries.GetMessageList
{
    public class MessageLookupDto : IMapWith<Message>
    {
        public Guid Id { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Message, MessageLookupDto>()
                .ForMember(messageDto => messageDto.Id,
                opt => opt.MapFrom(message => message.Id));
        }
    }
}
