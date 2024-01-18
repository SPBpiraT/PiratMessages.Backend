using AutoMapper;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Application.Messages.Commands.UpdateMessage;

namespace PiratMessages.WebApi.Models.Messages
{
    public class UpdateMessageDto : IMapWith<UpdateMessageCommand>
    {
        public Guid Id { get; set; }
        public string Details { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateMessageDto, UpdateMessageCommand>()
                .ForMember(messageCommand => messageCommand.Id,
                    opt => opt.MapFrom(messageDto => messageDto.Id))
                .ForMember(messageCommand => messageCommand.Details,
                    opt => opt.MapFrom(messageDto => messageDto.Details));
        }
    }
}
