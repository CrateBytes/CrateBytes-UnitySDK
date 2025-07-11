# CrateBytes Unity SDK

A comprehensive Unity SDK for integrating CrateBytes backend services into your games. This SDK provides authentication, session management, leaderboards, and player metadata functionality.

## Features

- **Authentication**: Guest and Steam authentication support
- **Session Management**: Automatic session tracking with heartbeat
- **Leaderboards**: Submit scores and retrieve leaderboard data
- **Player Metadata**: Store and retrieve player-specific data
- **Unity Integration**: Built specifically for Unity with coroutine support
- **Error Handling**: Comprehensive error handling and logging

## Installation

1. Import the CrateBytes SDK package into your Unity project
2. Add the `CrateBytesSDK` component to a GameObject in your scene
3. Configure the SDK with your project's base URL and public key

## Configuration

### SDK Settings

The SDK can be configured through the Inspector or via code:

```csharp
// Configure via code
CrateBytesSDK.Instance.Initialize("https://api.cratebytes.com/api/game", "your-public-key");
```

### Logging

The SDK includes a built-in logging system that can be enabled/disabled:

**Via Inspector:**
- Select the GameObject with the `CrateBytesSDK` component
- Check/uncheck the "Enable Logging" field

**Via Code:**
```csharp
// Enable logging
CrateBytesSDK.Instance.enableLogging = true;

// Or use the logger directly
CrateBytesLogger.Enabled = true;

// Disable logging (recommended for production)
CrateBytesLogger.Enabled = false;
```

**Note:** Logging is disabled by default for production builds. Enable only when debugging.

## Quick Start

### 1. Setup

Add the SDK component to your scene:

```csharp
// The SDK will automatically create a singleton instance
var sdk = CrateBytesSDK.Instance;

// Configure with your project settings
sdk.Initialize("https://api.cratebytes.com/api/game", "your-public-key");
```

### 2. Authentication

```csharp
// Guest authentication
yield return CrateBytesSDK.Instance.Auth.GuestLogin(null, (response) =>
{
    if (response.Success)
    {
        Debug.Log($"Authenticated! Player ID: {response.Data.playerId}");
    }
});

// Steam authentication
yield return CrateBytesSDK.Instance.Auth.SteamLogin("steam-auth-ticket", (response) =>
{
    if (response.Success)
    {
        Debug.Log($"Steam authenticated! Player ID: {response.Data.playerId}");
    }
});
```

### 3. Session Management

```csharp
// Start a session
yield return CrateBytesSDK.Instance.Session.StartSession((response) =>
{
    if (response.Success)
    {
        Debug.Log("Session started!");
        // Start automatic heartbeat
        CrateBytesSDK.Instance.StartHeartbeat();
    }
});

// Stop session when done
yield return CrateBytesSDK.Instance.Session.StopSession();
```

### 4. Leaderboards

```csharp
// Submit a score
yield return CrateBytesSDK.Instance.Leaderboard.SubmitScore("leaderboard-id", "1000", (response) =>
{
    if (response.Success)
    {
        Debug.Log("Score submitted!");
    }
});

// Get leaderboard entries
yield return CrateBytesSDK.Instance.Leaderboard.GetLeaderboard("leaderboard-id", 1, (response) =>
{
    if (response.Success)
    {
        foreach (var entry in response.Data.entries)
        {
            Debug.Log($"Player {entry.player.playerId}: {entry.score}");
        }
    }
});
```

### 5. Player Metadata

```csharp
// Define your player data structure
[Serializable]
public class PlayerData
{
    public int Level { get; set; }
    public int Experience { get; set; }
    public string[] Achievements { get; set; }
}

// Save player data
var playerData = new PlayerData
{
    Level = 5,
    Experience = 1250,
    Achievements = new[] { "FirstWin", "SpeedRunner" }
};

yield return CrateBytesSDK.Instance.Metadata.SetPlayerDataObject(playerData, (response) =>
{
    if (response.Success)
    {
        Debug.Log("Player data saved!");
    }
});

// Retrieve player data
yield return CrateBytesSDK.Instance.Metadata.GetPlayerData<PlayerData>((response) =>
{
    if (response.Success && response.Data != null)
    {
        Debug.Log($"Level: {response.Data.Level}, XP: {response.Data.Experience}");
    }
});
```

## API Reference

### CrateBytesSDK

Main SDK class that manages all services.

#### Properties
- `Auth`: Authentication service
- `Session`: Session management service
- `Leaderboard`: Leaderboard service
- `Metadata`: Player metadata service

#### Methods
- `Initialize(string baseUrl, string publicKey)`: Configure the SDK
- `IsConfigured()`: Check if SDK is properly configured
- `StartHeartbeat()`: Start automatic session heartbeat
- `StopHeartbeat()`: Stop automatic session heartbeat

### Authentication Service

Handles user authentication.

#### Methods
- `GuestLogin(string playerId, Action<CrateBytesResponse<AuthResponse>> callback)`: Authenticate as guest
- `SteamLogin(string steamAuthTicket, Action<CrateBytesResponse<AuthResponse>> callback)`: Authenticate with Steam
- `IsAuthenticated()`: Check if user is authenticated
- `GetAuthToken()`: Get current auth token
- `Logout()`: Clear authentication

### Session Service

Manages player sessions.

#### Methods
- `StartSession(Action<CrateBytesResponse<SessionData>> callback)`: Start a new session
- `Heartbeat(Action<CrateBytesResponse<SessionData>> callback)`: Send heartbeat
- `StopSession(Action<CrateBytesResponse<SessionData>> callback)`: Stop current session
- `IsSessionActive()`: Check if session is active
- `GetCurrentSession()`: Get current session data

### Leaderboard Service

Handles leaderboard operations.

#### Methods
- `GetLeaderboard(string leaderboardId, int page, Action<CrateBytesResponse<LeaderboardResponse>> callback)`: Get leaderboard entries
- `SubmitScore(string leaderboardId, string score, Action<CrateBytesResponse<ScoreSubmissionResponse>> callback)`: Submit a score

### Metadata Service

Manages player data.

#### Methods
- `GetPlayerData(Action<CrateBytesResponse<PlayerDataResponse>> callback)`: Get current player data
- `GetPlayerDataBySequentialId(int sequentialId, Action<CrateBytesResponse<PlayerDataResponse>> callback)`: Get player data by ID
- `SetPlayerData(string data, Action<CrateBytesResponse<PlayerDataResponse>> callback)`: Set player data as string
- `SetPlayerDataObject(object data, Action<CrateBytesResponse<PlayerDataResponse>> callback)`: Set player data as object
- `DeletePlayerData(Action<CrateBytesResponse<string>> callback)`: Delete player data
- `GetPlayerData<T>(Action<CrateBytesResponse<T>> callback)`: Get player data as specific type

## Data Structures

### AuthResponse
```csharp
public class AuthResponse
{
    public string token { get; set; }
    public string playerId { get; set; }
    public int sequentialId { get; set; }
    public string steamId { get; set; }
}
```

### SessionData
```csharp
public class SessionData
{
    public string id { get; set; }
    public string playerId { get; set; }
    public DateTime startTime { get; set; }
    public DateTime lastHeartbeat { get; set; }
    public DateTime? endTime { get; set; }
}
```

### LeaderboardResponse
```csharp
public class LeaderboardResponse
{
    public LeaderboardInfo leaderboard { get; set; }
    public LeaderboardEntry[] entries { get; set; }
    public int totalEntries { get; set; }
    public int pages { get; set; }
}
```

## Error Handling

All API calls return a `CrateBytesResponse<T>` object with the following structure:

```csharp
public class CrateBytesResponse<T>
{
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public CrateBytesError Error { get; set; }
    public bool Success { get; set; }
}
```

Always check the `Success` property before using the `Data`:

```csharp
yield return CrateBytesSDK.Instance.Auth.GuestLogin(null, (response) =>
{
    if (response.Success)
    {
        // Use response.Data safely
        Debug.Log($"Player ID: {response.Data.playerId}");
    }
    else
    {
        // Handle error
        Debug.LogError($"Error: {response.Error?.Message}");
    }
});
```

## Best Practices

### 1. Session Management
- Always start a session when the player begins playing
- Use automatic heartbeat to keep sessions alive
- Stop sessions when the player stops playing or the app is paused

### 2. Error Handling
- Always check response success before using data
- Implement retry logic for network failures
- Log errors for debugging

### 3. Data Management
- Use strongly-typed data structures for player metadata
- Validate data before sending to the server
- Handle data serialization errors gracefully

### 4. Performance
- Use coroutines for all API calls
- Don't make too many requests simultaneously
- Cache frequently accessed data locally

## Configuration

Configure the SDK in the Unity Inspector or programmatically:

```csharp
// Inspector configuration
[SerializeField] private string baseUrl = "https://api.cratebytes.com/api/game";
[SerializeField] private string publicKey = "your-public-key";
[SerializeField] private float heartbeatInterval = 60f; // 1 minute
[SerializeField] private float sessionTimeout = 300f; // 5 minutes

// Programmatic configuration
CrateBytesSDK.Instance.Initialize(baseUrl, publicKey);
```

## Dependencies

- Unity 2020.3 or later
- Newtonsoft.Json (included in package)

## Support

For support and documentation, visit:
- Documentation: https://docs.cratebytes.com
- Website: https://cratebytes.com
- Email: support@cratebytes.com

## License

This SDK is provided by CrateBytes for integration with the CrateBytes backend service. 