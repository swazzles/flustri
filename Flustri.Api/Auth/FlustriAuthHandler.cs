using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Flustri.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Flustri.Api.Auth;

public class FlustriAuthHandler : AuthenticationHandler<FlustriAuthSchemeOptions>
{
    private IServer _server;

    public FlustriAuthHandler(IOptionsMonitor<FlustriAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IServer server) : base(options, logger, encoder)
    {
        _server = server;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));

        var authToken = ExtractToken();
        if (authToken is null)
            return Task.FromResult(AuthenticateResult.Fail("Auth token is malformed."));

        var user = _server.GetUserById(authToken.UserId);

        var verified = VerifyToken(authToken, user);
        if (!verified)
            return Task.FromResult(AuthenticateResult.Fail("Authentication signature is invalid."));

        var ticket = GenerateAuthenticationTicket(user);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private AuthenticationTicket GenerateAuthenticationTicket(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        };

        var claimsIdentity = new ClaimsIdentity(claims, nameof(FlustriAuthHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
        return ticket;
    }

    private bool VerifyToken(FlustriAuthToken authToken, User user)
    {
        var signingContextOptions = new SigningContextOptions(user.KeyPem, HashAlgorithmName.SHA512);
        var signingContext = new SigningContext(signingContextOptions);

        var body = Span<byte>.Empty;
        Request.Body.ReadExactly(body);

        var verified = signingContext.Verify(body.ToArray(), authToken.Signature);

        // If we don't reset the request body stream back to the start it will remain as is and can cause issues further along the request chain.
        Request.Body.Seek(0, SeekOrigin.Begin);

        return verified;
    }

    private FlustriAuthToken? ExtractToken()
    {
        var authHeader = Request.Headers[HeaderNames.Authorization].ToString();
        var authHeaderDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
        var authToken = JsonSerializer.Deserialize<FlustriAuthToken>(authHeaderDecoded);
        return authToken;
    }
}