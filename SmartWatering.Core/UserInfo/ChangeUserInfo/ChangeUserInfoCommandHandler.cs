using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserInfo.ChangeUserInfo;

public class ChangeUserInfoCommandHandler : IRequestHandler<ChangeUserInfoCommand, IResult<Unit>>
{
    private readonly IExecutionResult<Unit> _executionResult;
    private readonly SwDbContext _context;
    private readonly IPasswordHash _passwordHash;

    public ChangeUserInfoCommandHandler(IExecutionResult<Unit> executionResult, SwDbContext context, IPasswordHash passwordHash)
    {
        _executionResult = executionResult;
        _context = context;
        _passwordHash = passwordHash;
    }

    public async Task<IResult<Unit>> Handle(ChangeUserInfoCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

        if (user == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(user)).ToString());
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                user.Name = request.Name ?? user.Name;
                user.SurName = request.SurName ?? user.SurName;
                user.Email = request.Email ?? user.Email;

                if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrEmpty(request.OldPassword))
                {
                    var salt = GenerateSalt.GenerateRandomSalt(16);
                    var hashNewPassword = _passwordHash.EncryptPassword(request.Password, salt);
                    var hashOldPassword = _passwordHash.EncryptPassword(request.OldPassword, Convert.FromBase64String(user.Salt));

                    if (user.Password == hashOldPassword)
                    {
                        user.Password = hashNewPassword;
                        user.Salt = Convert.ToBase64String(salt);
                    }
                    else
                    {
                        return await _executionResult.Fail("Error: The old password does not match the user's actual password.");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _executionResult.Successful(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await _executionResult.Fail($"Failed to change user info. Error: {ex.Message}");
            }
        }
    }
}
