
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core.Models;

public class User
{
    public required string UserId { get; set; }

    [Timestamp]
    public required Guid Version { get; set; }

    public required byte[] PublicKey { get; set; }    

    public string? Nickname { get; set; }

    public string? Description { get; set; }    
}