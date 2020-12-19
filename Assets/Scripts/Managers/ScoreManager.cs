using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int score;

    public static Action<int> OnScoreUpdated;

    private void OnEnable()
    {
        OnScoreUpdated += IncrementScore;
    }

    private void OnDisable()
    {
        OnScoreUpdated -= IncrementScore;
    }

    private void IncrementScore(int value)
    {
        score += value;
        GameUIManager.Instance.SetScore(score);
    }
}
