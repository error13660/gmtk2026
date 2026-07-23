using System;
using System.Collections;

public interface ILeaderboardApiService
{
    IEnumerator GetLeaderboard(
        Action<LeaderboardListResponse> onSuccess,
        Action<string> onError
    );

    IEnumerator GetPlayer(
        string playerName,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    );

    IEnumerator SavePlayer(
        string playerName,
        int score,
        int depth,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    );
}