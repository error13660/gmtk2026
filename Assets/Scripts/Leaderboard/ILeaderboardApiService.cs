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
        int depth,
        Action<LeaderboardEntryResponse> onSuccess,
        Action<string> onError
    );
}