using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PiratMessages.Application.Messages.Commands.CreateMessage;
using PiratMessages.Application.Messages.Commands.DeleteMessage;
using PiratMessages.Application.Messages.Commands.UpdateMessage;
using PiratMessages.Application.Messages.Queries.GetMessageDetails;
using PiratMessages.Application.Messages.Queries.GetMessageList;
using PiratMessages.WebApi.Models;

namespace PiratMessages.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMapper _mapper;

        public MessageController(IMapper mapper) => _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<MessageListVm>> GetAll()
        {
            var query = new GetMessageListQuery
            {
                UserId = UserId
            };

            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDetailsVm>> Get(Guid id)
        {
            var query = new GetMessageDetailsQuery
            {
                UserId = UserId,
                Id = id
            };

            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMessageDto createMessageDto)
        {
            var command = _mapper.Map<CreateMessageCommand>(createMessageDto);
            command.UserId = UserId;
            var messageId = await Mediator.Send(command);
            return Ok(messageId);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateMessageDto updateMessageDto)
        {
            var command = _mapper.Map<UpdateMessageCommand>(updateMessageDto);
            command.UserId = UserId;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteMessageCommand
            {
                Id = id,
                UserId = UserId
            };

            await Mediator.Send(command);
            return NoContent();
        }

    }
}
