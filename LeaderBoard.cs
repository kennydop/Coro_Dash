using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce.Internal;
using CloudOnce;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard Instance;
    private void Awake()
    {
        TestSingleton();
    }

    private void TestSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SubmitScoreToLeaderBoard(int score)
    {
        Leaderboards.Leaderboard.SubmitScore(score);
    }
}
