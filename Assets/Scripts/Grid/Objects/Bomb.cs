using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bomb : GridObject
{
    private int scoreAmount = 5;
    private int bombTime = 10;
    [SerializeField]
    private TextMeshPro bombText;

    private void OnEnable()
    {
        GroupSelectorController.OnRotationCompleted += DecrementBombTime;
        ExplosionManager.OnExplosionCompleted += DecrementBombTime;
        bombTime = 10;
        SetBombTime(bombTime);
    }

    private void OnDisable()
    {
        GroupSelectorController.OnRotationCompleted -= DecrementBombTime;
        ExplosionManager.OnExplosionCompleted -= DecrementBombTime;
    }

    public override void ExplodeObject()
    {
        ScoreManager.OnScoreUpdated.Invoke(scoreAmount);
        SpawnManager.Instance.AddToBombPool(this);
        gameObject.SetActive(false);
    }

    private void DecrementBombTime()
    {
        --bombTime;
        SetBombTime(bombTime);

        if (bombTime == 0)
        {
            GameManager.OnGameOver?.Invoke();
        }
    }

    private void SetBombTime(int value)
    {
        bombText.text = value.ToString();
    }
}
