using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : GridObject
{
    private int scoreAmount = 5;
    
    public override void ExplodeObject()
    {
        ScoreManager.OnScoreUpdated.Invoke(scoreAmount);
        SpawnManager.Instance.AddToHexagonPool(this);
        gameObject.SetActive(false);
    }
}
