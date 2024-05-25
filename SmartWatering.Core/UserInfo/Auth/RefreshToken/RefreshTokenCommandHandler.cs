using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartWatering.Core.ExecutionResults;
using SmartWatering.DAL.DbContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartWatering.Core.UserInfo.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResult<Tokens>>
{
    private readonly SwDbContext _context;
    private readonly string _jwtApiKey;
    private readonly IExecutionResult<Tokens> _executionResult;

    public RefreshTokenCommandHandler(SwDbContext context, IConfiguration configuration, IExecutionResult<Tokens> executionResult)
    {
        _context = context;
        _jwtApiKey = configuration.GetSection("JWTSettings")["ApiKey"];
        _executionResult = executionResult;
    }

    public async Task<IResult<Tokens>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await _executionResult.Fail(new ArgumentNullException(nameof(request)).ToString());
        }

        var user = _context.Users.FirstOrDefault(a => a.Id == request.UserId);

        if (user == null)
        {
            return await _executionResult.Fail(new ArgumentNullException("User not found!").ToString());
        }

        var storedRefreshToken = RefreshTokenStorage.GetRefreshToken(user.Id);

        if (storedRefreshToken != request.RefreshToken)
        {
            return await _executionResult.Fail(new ArgumentNullException("Refresh token not valid or expired.").ToString());
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtApiKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(request.AccessToken, tokenValidationParameters, out _);

        if (!(claimsPrincipal.Identity is ClaimsIdentity claimsIdentity) ||
            !claimsIdentity.IsAuthenticated ||
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value != user.Id.ToString())
        {
            return await _executionResult.Fail(new ArgumentNullException("Refresh token not valid or expired.").ToString());
        }

        var refreshToken = TokenUtilities.GenerateRefreshToken();
        RefreshTokenStorage.AddOrUpdateRefreshToken(user.Id, refreshToken);

        var tokens = new Tokens
        {
            AccessToken = TokenUtilities.CreateToken(user, _jwtApiKey),
            RefreshToken = refreshToken,
        };

        return await _executionResult.Successful(tokens);
    }
}