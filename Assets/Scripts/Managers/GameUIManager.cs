using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : Singleton<GameUIManager>
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI moveCountText;
    [SerializeField] GameObject gameOverPanel;

    private void OnEnable()
    {
        GroupSelectorController.OnRotationCompleted += SetMoveNumber;
        ExplosionManager.OnExplosionCompleted += SetMoveNumber;
        GameManager.OnGameOver += EnableGameOverScreen;
    }

    private void OnDisable()
    {
        GroupSelectorController.OnRotationCompleted -= SetMoveNumber;
        ExplosionManager.OnExplosionCompleted -= SetMoveNumber;
        GameManager.OnGameOver -= EnableGameOverScreen;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void SetMoveNumber()
    {
        moveCountText.text = (int.Parse(moveCountText.text) + 1).ToString();
    }

    private void EnableGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }
}
