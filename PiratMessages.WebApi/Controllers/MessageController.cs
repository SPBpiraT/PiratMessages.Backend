using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PiratMessages.Application.Messages.Commands.CreateMessage;
using PiratMessages.Application.Messages.Commands.DeleteMessage;
using PiratMessages.Application.Messages.Commands.UpdateMessage;
using PiratMessages.Application.Messages.Queries.GetMessageDetails;
using PiratMessages.Application.Messages.Queries.GetMessageList;
using PiratMessages.WebApi.Models.Messages;

namespace PiratMessages.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMapper _mapper;

        public MessageController(IMapper mapper) => _mapper = mapper;

        /// <summary>  
        /// Gets the list
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /message
        /// </remarks>
        /// <returns>Returns MessageListVm</returns>
        /// <responce code="200">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MessageListVm>> GetAll()
        {
            var query = new GetMessageListQuery
            {
                UserId = UserId
            };

            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Gets the message by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /message/9499F226-3A5E-4F74-A97E-DBBE9354125A
        /// </remarks>
        /// <param name="id">Message id (guid)</param>
        /// <returns>Returns MessageDetailsVm</returns>
        /// <responce code="200">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Creates the message
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /message
        /// {
        ///     details: "message details"
        /// }
        /// </remarks>
        /// <param name="createMessageDto">CreateMessageDto object</param>
        /// <returns>Returns id (guid)</returns>
        /// <response code="201">Success</response>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMessageDto createMessageDto)
        {
            var command = _mapper.Map<CreateMessageCommand>(createMessageDto);
            command.UserId = UserId;
            var messageId = await Mediator.Send(command);
            return Ok(messageId);
        }

        /// <summary>
        /// Updates the message
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// PUT /message
        /// </remarks>
        /// <param name="updateMessageDto">UpdateMessageDto object</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <responce code="401">If the user is unauthorized</responce>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateMessageDto updateMessageDto)
        {
            var command = _mapper.Map<UpdateMessageCommand>(updateMessageDto);
            command.UserId = UserId;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes the message by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// DELETE /message/56CF4FCC-D868-406D-9BF2-E4418ECB8255
        /// </remarks>
        /// <param name="id">Id of the message (guid)</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
