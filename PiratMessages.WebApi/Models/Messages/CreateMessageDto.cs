using AutoMapper;
using PiratMessages.Application.Common.Mappings;
using PiratMessages.Application.Messages.Commands.CreateMessage;
using System.ComponentModel.DataAnnotations;

namespace PiratMessages.WebApi.Models.Messages
{
    public class CreateMessageDto : IMapWith<CreateMessageCommand>
    {
        [Required]
        public Guid DestinationUserId { get; set; }
        [Required]
        public string Details { get; set; } = "test";


        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateMessageDto, CreateMessageCommand>()
                .ForMember(messageCommand => messageCommand.Details,
                opt => opt.MapFrom(messageDto => messageDto.Details));
        }
    }
}
