namespace Flustri.Api;

// Rooms
public record CreateRoomRequest(
    string Name
);


// Messages

public record CreateMessageRequest(
    string Message
);

public record UpdateMessageRequest (
    string Message
);