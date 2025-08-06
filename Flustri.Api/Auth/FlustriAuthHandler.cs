using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Flustri.Core;
using Flustri.Core.Models;
using Flustri.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NSec.Cryptography;

namespace Flustri.Api.Auth;

public class FlustriAuthHandler : AuthenticationHandler<FlustriAuthSchemeOptions>
{
    private FlustriDbContext _db;
    private ISigningService _signingService;

    public FlustriAuthHandler(
        IOptionsMonitor<FlustriAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        FlustriDbContext db,
        ISigningService signingService) : base(options, logger, encoder)
    {
        _db = db;
        _signingService = signingService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            return await Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));

        var authToken = ExtractToken();
        if (authToken is null)
            return await Task.FromResult(AuthenticateResult.Fail("Auth token is malformed."));

        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == authToken.UserId);

        if (user is null)
            return await Task.FromResult(AuthenticateResult.Fail("Authentication signature is invalid."));

        var verified = VerifyToken(authToken, user);
        if (!verified)
            return await Task.FromResult(AuthenticateResult.Fail("Authentication signature is invalid."));

        var ticket = GenerateAuthenticationTicket(user);
        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private AuthenticationTicket GenerateAuthenticationTicket(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, nameof(FlustriAuthHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
        return ticket;
    }

    private bool VerifyToken(FlustriAuthToken authToken, User user)
    {        
        var body = Span<byte>.Empty;
        Request.Body.ReadExactly(body);

        if (!PublicKey.TryImport(SignatureAlgorithm.Ed25519, user.PublicKey, KeyBlobFormat.NSecPublicKey, out var publicKey))
            return false;

        if (publicKey is null)
            return false;

        var verified = _signingService.Verify(publicKey, body.ToArray(), authToken.Signature);

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
