using System;
using System.Collections.Generic;
using UnityEngine;

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
