using FluentValidation;

namespace PiratMessages.Application.Messages.Queries.GetMessageDetails
{
    public class GetMessageDetailsQueryValidator : AbstractValidator<GetMessageDetailsQuery>
    {
        public GetMessageDetailsQueryValidator()
        {
            RuleFor(message =>
                message.Id).NotEqual(Guid.Empty);
            RuleFor(message =>
                message.UserId).NotEqual(Guid.Empty);
        }
    }
}
