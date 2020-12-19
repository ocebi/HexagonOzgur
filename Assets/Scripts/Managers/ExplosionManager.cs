using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionManager : Singleton<ExplosionManager>
{
    //public static Action<List<GridObject>> OnExplosionDetected;
    public static Action OnFallCompleted;
    public static Action<List<Vector2>> OnExplosionDetected;
    public static Action OnExplosionCompleted;

    List<GridObject> checkExplosionAfterFall = new List<GridObject>();

    private int blocksFallingDown = 0;

    #region Unity Methods
    private void OnEnable()
    {
        OnFallCompleted += ClearExplosionBuffer;
    }

    private void OnDisable()
    {
        OnFallCompleted -= ClearExplosionBuffer;
    }
    #endregion


    #region Public Methods
    /// <summary>
    /// Checks if the selected group triggers any explosion.
    /// Explodes them if they do with respect to the parameter provided.
    /// </summary>
    /// <param name="group"> Selected group. </param>
    /// <param name="explode"> Explode the objects or not. </param>
    /// <returns> True if explosion occurs. False if not. </returns>
    public bool CheckExplosion(List<GridObject> group, bool explode=true)
    {
        List<Vector2> checkListVector;
        List<GridObject> explosionList;
        checkListVector = GetSelectablePositions(group);
        explosionList = FillExplosionList(checkListVector);

        if (explosionList.Count > 0)
        {
            Debug.Log("Explosion detected");

            if (explode)
            {
                Explode(explosionList);
            }
            return true;

        }
        else
        {
            //Debug.Log("Explosion not detected");
            return false;
        }
    }

    /// <summary>
    /// Sets up the blocks by attaching the upper grid objects on top of the bottom grid object.
    /// Sends the blocks to DropBlock coroutine to start movement.
    /// </summary>
    /// <param name="moveList"> Heads of the blocks. </param>
    public void SetupBlocks(List<GridObject> moveList)
    {
        Debug.Log("Inside move list");
        foreach (var go in moveList)
        {
            go.neighbors = go.GetNeighbors();
            for (GridObject i = GridObject.GetObjectAt(go.neighbors.up); i != null; i = GridObject.GetObjectAt(i.neighbors.up))
            {
                i.transform.parent = go.transform;
                i.neighbors = i.GetNeighbors();
            }
            StartCoroutine(DropBlock(go));
        }
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Calculates the selectable positions that needs to be checked for explosion.
    /// </summary>
    /// <param name="group"> Selected group. </param>
    /// <returns> Selectable positions near the selected group. </returns>
    private List<Vector2> GetSelectablePositions(List<GridObject> group)
    {
        List<Vector2> checkListVector = new List<Vector2>();
        foreach (GridObject go in group)
        {
            go.neighbors = go.GetNeighbors();
            checkListVector.Add(go.neighbors.upLeft + new Vector2(GridObject.HALF_VERTICAL, 0));
            checkListVector.Add(go.neighbors.upRight + new Vector2(-GridObject.HALF_VERTICAL, 0));
            checkListVector.Add(new Vector2(go.transform.position.x - GridObject.HALF_VERTICAL, go.transform.position.y)); //left
            checkListVector.Add(new Vector2(go.transform.position.x + GridObject.HALF_VERTICAL, go.transform.position.y)); //right
            checkListVector.Add(go.neighbors.downLeft + new Vector2(GridObject.HALF_VERTICAL, 0));
            checkListVector.Add(go.neighbors.downRight + new Vector2(-GridObject.HALF_VERTICAL, 0));
        }
        checkListVector = checkListVector.Distinct().ToList();
        return checkListVector;
    }

    /// <summary>
    /// Finds the objects that explode and returns them.
    /// </summary>
    /// <param name="checkListVector"> Vector2 positions to check for explosion. </param>
    /// <returns> Objects that explode. </returns>
    private List<GridObject> FillExplosionList(List<Vector2> checkListVector)
    {
        List<GridObject> explosionList = new List<GridObject>();
        foreach (Vector2 pos in checkListVector)
        {
            var checkGroup = GridManager.Instance.GetGroupAt(pos);
            if (checkGroup == null) continue;

            if (GridColorer.Instance.CheckColorMatch(checkGroup))
            {
                foreach (var item in checkGroup)
                    explosionList.Add(item);
            }

        }
        explosionList = explosionList.Distinct().ToList();
        return explosionList;

    }

    /// <summary>
    /// Triggers the explosion for the explosion list.
    /// </summary>
    /// <param name="explosionList"> Grid objects to explode. </param>
    private void Explode(List<GridObject> explosionList)
    {
        foreach (var go in explosionList)
        {
            go.ExplodeObject();
        }

        SelectBlockHeads(explosionList);

    }

    /// <summary>
    /// Selects the bottom grid objects of the exploded objects. Sends them to SetupBlocks.
    /// </summary>
    /// <param name="explosionList"> Exploded grid objects. </param>
    private void SelectBlockHeads(List<GridObject> explosionList)
    {
        List<GridObject> moveList = new List<GridObject>();

        foreach(var go in explosionList)
        {
            go.neighbors = go.GetNeighbors();
            GridObject up = GridObject.GetObjectAt(go.neighbors.up);
            if (up != null && !explosionList.Contains(up))
            {
                moveList.Add(up);
            }
        }
        List<Vector2> explosionPositions = new List<Vector2>();
        foreach(GridObject go in explosionList)
        {
            explosionPositions.Add(go.transform.position);
        }
        OnExplosionDetected.Invoke(explosionPositions);

        SetupBlocks(moveList);
    }

    

    /// <summary>
    /// Starts moving the block down along with the attached grid objects on top.
    /// </summary>
    /// <param name="go"> Head of the block. </param>
    private IEnumerator DropBlock(GridObject go)
    {
        ++blocksFallingDown;
        go.neighbors = go.GetNeighbors();

        while ((GridObject.GetObjectAt(go.neighbors.down) == null) &&
            !Mathf.Approximately(go.transform.localPosition.y, 0f) &&
            !Mathf.Approximately(go.transform.localPosition.y, -0.35f))
        {
            var isTweenFinished = Tween.Position(go.transform, new Vector3(go.transform.position.x, go.neighbors.down.y - 0.05f, go.transform.position.z), 0.1f, 0f);
            while (isTweenFinished.Status != Tween.TweenStatus.Finished)
            {
                yield return null;
            }
            GridManager.RoundPosition(go);
            GridManager.RoundScale(go);

            go.neighbors = go.GetNeighbors();
        }

        List<Transform> childList = new List<Transform>();

        foreach (Transform child in go.transform)
        {
            if(child.GetComponent<GridObject>() != null)
            {
                childList.Add(child);
            }
        }

        foreach (var child in childList)
        {
            GridObject childGridObject = child.GetComponent<GridObject>();
            if (childGridObject == null) continue;
            child.parent = GridGenerator.Instance.transform;
            child.position = new Vector3(go.transform.position.x,
                    child.position.y,
                    child.position.z);
            checkExplosionAfterFall.Add(childGridObject);
            GridManager.RoundPosition(childGridObject);
            GridManager.RoundScale(childGridObject);
        }

        checkExplosionAfterFall.Add(go);

        --blocksFallingDown;
        if(blocksFallingDown == 0)
        {
            OnFallCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Checks if the relocated objects trigger an explosion.
    /// </summary>
    private void ClearExplosionBuffer()
    {
        List<GridObject> explosionBuffer = new List<GridObject>(checkExplosionAfterFall);
        explosionBuffer = explosionBuffer.Distinct().ToList();
        checkExplosionAfterFall.Clear();
        var explosion = CheckExplosion(explosionBuffer);
        if(!explosion)
            OnExplosionCompleted.Invoke();

    }

    #endregion
}
