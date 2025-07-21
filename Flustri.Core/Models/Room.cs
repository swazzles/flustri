using System.ComponentModel.DataAnnotations;

namespace Flustri.Core.Models;

public class Room
{
    public string? RoomId { get; set; }
    
    [Timestamp]
    public required Guid Version { get; set; }
}