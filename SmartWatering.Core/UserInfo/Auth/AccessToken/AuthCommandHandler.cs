using MediatR;
using Microsoft.Extensions.Configuration;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.Core.Helpers.PasswordHasher;
using SmartWatering.DAL.DbContext;

namespace SmartWatering.Core.UserInfo.Auth.AccessToken;

public class AuthCommandHandler : IRequestHandler<AuthCommand, IResult<Tokens>>
{
    private readonly SwDbContext _context;
    private readonly string _jwtApiKey;
    private readonly IPasswordHash _hasher;
    private readonly IExecutionResult<Tokens> _executionResult;

    public AuthCommandHandler(SwDbContext context, IPasswordHash hasher, IConfiguration configuration, IExecutionResult<Tokens> executionResult)
    {
        _context = context;
        _hasher = hasher;
        _executionResult = executionResult;
        _jwtApiKey = configuration.GetSection("JWTSettings")["ApiKey"];
    }

    public async Task<IResult<Tokens>> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var user = _context.Users.FirstOrDefault(a => a.Email == request.Email);
        if (user == null)
        {
            return await _executionResult.Fail(new InvalidDataException("Wrong credentials!").ToString());
        }

        var passwordHash = _hasher.EncryptPassword(request.Password, Convert.FromBase64String(user.Salt));

        if (user.Password == passwordHash)
        {
            var accessToken = TokenUtilities.CreateToken(user, _jwtApiKey);
            var refreshToken = TokenUtilities.GenerateRefreshToken();

            RefreshTokenStorage.AddOrUpdateRefreshToken(user.Id, refreshToken);

            var tokens = new Tokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return await _executionResult.Successful(tokens);
        }
        else
        {
            return await _executionResult.Fail(new InvalidDataException("Wrong credentials!").ToString());
        }
    }
}
