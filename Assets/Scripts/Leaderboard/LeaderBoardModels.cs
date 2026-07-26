using System;
using System.Collections.Generic;

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

[Serializable]
public class LeaderboardEntry
{
    public int id;
    public long player_id;
    public string player_name;
    public int depth;
    public string updated_at;
}

[Serializable]
public class LeaderboardEntryRequest
{
    public long player_id;
    public string player_name;
    public int depth;

    public LeaderboardEntryRequest(
        long playerId,
        string playerName,
        int depth
    )
    {
        player_id = playerId;
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