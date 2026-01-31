using Mapster;
using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.GetUserInformation;

public sealed class GetUserInformationHandler(
    AuthDbContext context,
    ILogger<GetUserInformationHandler> logger)
{
    public async Task<Result<GetUserInformationResponse>> Handle(
        GetUserInformationCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            GetUserInformationResponse? user = await context.Users
                .Where(u => u.Id == command.UserId)
                .ProjectToType<GetUserInformationResponse>()
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User not found with ID {UserId}.", command.UserId);
                return Result<GetUserInformationResponse>.Fail("User not found.", ResultCode.NotFound);
            }

            return Result<GetUserInformationResponse>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while retrieving user information for user {UserId}.",
                command.UserId);
            return Result<GetUserInformationResponse>.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}