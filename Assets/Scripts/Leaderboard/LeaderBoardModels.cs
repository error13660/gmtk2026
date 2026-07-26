using System;
using System.Collections.Generic;

// Leaderboard function models

[Serializable]
public class LeaderboardPlayerData
{
    public string playerName;
    public int depth;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardPlayerData> players;
}


// API models

[Serializable]
public class LeaderboardEntry
{
    public int id;
    public long user_id;
    public string player_name;
    public int depth;
    public string updated_at;
}

[Serializable]
public class LeaderboardEntryRequest
{
    public long user_id;
    public string player_name;
    public int depth;

    public LeaderboardEntryRequest(
        long userId,
        string playerName,
        int depth
    )
    {
        user_id = userId;
        player_name = playerName;
        this.depth = depth;
    }
}

[Serializable]
public class LeaderboardListResponse
{
    public bool success;
    public string message;
    public LeaderboardEntry[] data;
}

[Serializable]
public class LeaderboardEntryResponse
{
    public bool success;
    public string message;
    public LeaderboardEntry data;
}