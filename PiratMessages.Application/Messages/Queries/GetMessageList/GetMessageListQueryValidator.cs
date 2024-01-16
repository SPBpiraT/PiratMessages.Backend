using FluentValidation;

namespace PiratMessages.Application.Messages.Queries.GetMessageList
{
    public class GetMessageListQueryValidator : AbstractValidator<GetMessageListQuery>
    {
        public GetMessageListQueryValidator()
        {
            RuleFor(x =>
                x.UserId).NotEqual(Guid.Empty);
        }
    }
}
