
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core;

public record UserConfig(
    string KeyPath,
    string Description,
    string Nickname
);

public class User
{
    public required string UserId { get; set; }
    public required char[] KeyPem { get; set; }

    public string? Nickname { get; set; }
    public string? Description { get; set; }    
}