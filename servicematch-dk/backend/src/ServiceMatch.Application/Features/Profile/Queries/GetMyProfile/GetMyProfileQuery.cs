using MediatR;

namespace ServiceMatch.Application.Features.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery(Guid UserId, string Role) : IRequest<object>;
