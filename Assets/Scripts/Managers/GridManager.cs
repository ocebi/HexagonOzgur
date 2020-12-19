using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static Action<Hexagon> OnGridGenerated;

    [SerializeField] GroupSelectorController groupSelectorController;
    [SerializeField] float radius;

    #region Unity Methods
    private void OnEnable()
    {
        GameInputManager.OnClickDetected += SelectGroup;
        ExplosionManager.OnExplosionCompleted += EnableGroupSelector;
        GameManager.OnGameOver += DisableGroupSelector;
    }

    private void OnDisable()
    {
        GameInputManager.OnClickDetected -= SelectGroup;
        ExplosionManager.OnExplosionCompleted -= EnableGroupSelector;
        GameManager.OnGameOver -= DisableGroupSelector;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Tries to get the GridObject group at the provided position.
    /// </summary>
    /// <returns> GridObject group at the provided position if it exists. </returns>
    public List<GridObject> GetGroupAt(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        List<GridObject> group = new List<GridObject>();
        if (colliders.Length == 3)
        {
            foreach(Collider2D c in colliders)
            {
                group.Add(c.GetComponent<GridObject>());
            }
            return group;
        }
        else return null;
    }

    public List<GridObject> GetGroupAtSelector()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groupSelectorController.transform.position, radius);
        List<GridObject> group = new List<GridObject>();
        if (colliders.Length == 3)
        {
            foreach (Collider2D c in colliders)
            {
                group.Add(c.GetComponent<GridObject>());
            }
            return group;
        }
        else return null;
    }

    /// <summary>
    /// Places the group selected indicator around the provided group.
    /// </summary>
    /// <param name="group"></param>
    public void SelectGroup(List<GridObject> group)
    {
        if (group.Count != 3)
        {
            Debug.LogWarning("Only a group of 3 can be selected.");
            return;
        }

        Vector3 position = Vector3.zero;
        foreach (GridObject go in group)
        {
            position += go.transform.position;
        }
        position.z = groupSelectorController.transform.position.z;

        groupSelectorController.gameObject.SetActive(true);
        groupSelectorController.SetLayout(GetGroupLayout(group));
        groupSelectorController.SetPosition(position / 3);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// GridObjects might have 2 possible layouts: 2 on the left or 2 on the right.
    /// Determines whether the group selected indicator should rotate or not with respect to the group layout.
    /// </summary>
    /// <param name="group"> Selected group </param>
    /// <returns> 1 if the indicator should rotate. 0 if not. </returns>
    private int GetGroupLayout(List<GridObject> group)
    {
        if (!Mathf.Approximately(group[0].transform.position.x, group[1].transform.position.x) && 
            !Mathf.Approximately(group[0].transform.position.x, group[2].transform.position.x))
        {
            if (group[0].transform.position.x < group[1].transform.position.x)
            {
                return 0;
            }
            else
                return 1;
        }
        else if (!Mathf.Approximately(group[1].transform.position.x, group[0].transform.position.x) &&
            !Mathf.Approximately(group[1].transform.position.x, group[2].transform.position.x))
        {
            if (group[1].transform.position.x < group[0].transform.position.x)
            {
                return 0;
            }
            else
                return 1;
        }
        else if (!Mathf.Approximately(group[2].transform.position.x, group[0].transform.position.x) &&
            !Mathf.Approximately(group[2].transform.position.x, group[1].transform.position.x))
        {
            if (group[2].transform.position.x < group[0].transform.position.x)
            {
                return 0;
            }
            else
                return 1;
        }
        else
        {
            Debug.LogWarning("Cannot find the group layout.");
            return 0;
        }
    }

    /// <summary>
    /// Checks for a possible move. If it exists, enables the group selector.
    /// </summary>
    private void EnableGroupSelector()
    {
        if(PossibleMoveExists())
        {
            Debug.Log("Possible move exists.");
            groupSelectorController.gameObject.SetActive(true);
            groupSelectorController.enabled = true;
        }
    }

    private void DisableGroupSelector()
    {
        groupSelectorController.enabled = false;
        groupSelectorController.gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks if a move exists that causes an explosion. Iterates through the grid until a move is found.
    /// If no move exists, the game is over.
    /// </summary>
    /// <returns> True if possible move exists. False if not. </returns>
    private bool PossibleMoveExists()
    {
        GridGenerator.Instance.gridOrigin.neighbors = GridGenerator.Instance.gridOrigin.GetNeighbors();

        for (GridObject i = GridGenerator.Instance.gridOrigin; GridObject.GetObjectAt(i.neighbors.upRight) != null; i.neighbors = i.GetNeighbors() ,i = GridObject.GetObjectAt(i.neighbors.upRight))
        {
            //i.neighbors = i.GetNeighbors();
            if (GridObject.GetObjectAt(i.neighbors.down) != null)
                i = GridObject.GetObjectAt(i.neighbors.down);

            for (GridObject j = i; j != null; j.neighbors = j.GetNeighbors(), j = GridObject.GetObjectAt(j.neighbors.up))
            {
                if (GridObject.GetObjectAt(j.neighbors.downRight) != null && GridObject.GetObjectAt(j.neighbors.upRight) != null)
                {
                    Dictionary<GridObject, Color> initialColors = new Dictionary<GridObject, Color>();
                    
                    GridObject downRight = GridObject.GetObjectAt(j.neighbors.downRight);
                    GridObject upRight = GridObject.GetObjectAt(j.neighbors.upRight);
                    initialColors.Add(j, j.color);
                    initialColors.Add(downRight, downRight.color);
                    initialColors.Add(upRight, upRight.color);

                    if (downRight == null || upRight == null) continue;

                    List<GridObject> hexagonGroup1 = new List<GridObject>
                    {
                        j,
                        downRight,
                        upRight
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup1, false))
                        return true;

                    downRight.color = initialColors[j];
                    upRight.color = initialColors[downRight];
                    j.color = initialColors[upRight];

                    List<GridObject> hexagonGroup2 = new List<GridObject>
                    {
                        downRight,
                        upRight,
                        j
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup2, false))
                    {
                        SetInitialColors(hexagonGroup2, initialColors);
                        return true;
                    }

                    upRight.color = initialColors[j];
                    j.color = initialColors[downRight];
                    downRight.color = initialColors[upRight];
                    List<GridObject> hexagonGroup3 = new List<GridObject>
                    {
                        upRight,
                        j,
                        downRight
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup3, false))
                    {
                        SetInitialColors(hexagonGroup3, initialColors);
                        return true;
                    }
                    SetInitialColors(hexagonGroup3, initialColors);
                }

                j.neighbors = j.GetNeighbors();

                if (GridObject.GetObjectAt(j.neighbors.up) != null)
                {
                    Dictionary<GridObject, Color> initialColors = new Dictionary<GridObject, Color>();
                    
                    GridObject up = GridObject.GetObjectAt(j.neighbors.up);
                    GridObject upRight = GridObject.GetObjectAt(j.neighbors.upRight);
                    if (up == null || upRight == null) continue;
                    initialColors.Add(j, j.color);
                    initialColors.Add(up, up.color);
                    initialColors.Add(upRight, upRight.color);

                    List<GridObject> hexagonGroup1 = new List<GridObject>
                    {
                        j,
                        up,
                        upRight
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup1, false))
                        return true;

                    upRight.color = initialColors[j];
                    j.color = initialColors[up];
                    up.color = initialColors[upRight];
                    List<GridObject> hexagonGroup2 = new List<GridObject>
                    {
                        upRight,
                        j,
                        up
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup2, false))
                    {
                        SetInitialColors(hexagonGroup2, initialColors);
                        return true;
                    }
                    up.color = initialColors[j];
                    upRight.color = initialColors[up];
                    j.color = initialColors[upRight];
                    List<GridObject> hexagonGroup3 = new List<GridObject>
                    {
                        up,
                        upRight,
                        j
                    };
                    if (ExplosionManager.Instance.CheckExplosion(hexagonGroup3, false))
                    {
                        SetInitialColors(hexagonGroup3, initialColors);
                        return true;
                    }
                    SetInitialColors(hexagonGroup3, initialColors);
                }
            }
        }
        //If no possible move is found, the game is over.
        Debug.Log("No more moves left.");
        GameManager.OnGameOver.Invoke();
        return false;
    }

    /// <summary>
    /// Sets the color of the GridObjects in the group to their initial colors.
    /// </summary>
    private void SetInitialColors(List<GridObject> group, Dictionary<GridObject, Color> initialColors)
    {
        foreach (var go in group)
            go.color = initialColors[go];
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Rounds the position of the GridObject provided.
    /// </summary>
    /// <param name="go"></param>
    public static void RoundPosition(GridObject go)
    {
        Transform t = go.transform;
        t.localPosition = new Vector3(
         (float)System.Math.Round(t.localPosition.x, 2),
        (float)System.Math.Round(t.localPosition.y, 2),
         (float)System.Math.Round(t.localPosition.z, 2));
    }

    /// <summary>
    /// Rounds the scale of the GridObject provided.
    /// </summary>
    /// <param name="go"></param>
    public static void RoundScale(GridObject go)
    {
        Transform t = go.transform;
        t.localScale = new Vector3(
         (float)System.Math.Round(t.localScale.x, 2),
        (float)System.Math.Round(t.localScale.y, 2),
         (float)System.Math.Round(t.localScale.z, 2));
    }
    #endregion

}
