using System;
using System.Collections.Generic;
using UnityEngine;

// Leaderboard Function models

[Serializable]
public class LeaderboardPlayerData
{
    public string playerName;
    public int score;
    public int depth;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardPlayerData> players;
}


// API MODELS 

[Serializable]
public class LeaderboardEntry
{
    public int id;
    public string player_name;
    public int score;
    public int depth;
    public string updated_at;
}

[Serializable]
public class LeaderboardEntryRequest
{
    public string player_name;
    public int score;
    public int depth;

    public LeaderboardEntryRequest(string playerName, int score, int depth)
    {
        player_name = playerName;
        this.score = score;
        this.depth = depth;
    }
}

[Serializable]
public class LeaderboardEntryResponse
{
    public bool success;
    public string message;
    public LeaderboardEntry data;
}

[Serializable]
public class LeaderboardListResponse
{
    public bool success;
    public string message;
    public LeaderboardEntry[] data;
}

