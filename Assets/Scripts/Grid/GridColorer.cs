using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColorer : Singleton<GridColorer>
{
    public Color[] colorArray;

    #region Unity Methods

    private void Start()
    {
        //In case alpha values of the color array are not set to 255 by default.
        for (int i = 0; i < colorArray.Length; ++i)
        {
            colorArray[i].a = 255;
        }
    }

    private void OnEnable()
    {
        GridManager.OnGridGenerated += InitialColorer;
    }

    private void OnDisable()
    {
        GridManager.OnGridGenerated -= InitialColorer;
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Iterates over the grid. Checks for a triple match. If any match occurs, recolors the hexagons.
    /// </summary>
    /// <param name="origin"> Start position of the grid. </param>
    private void InitialColorer(Hexagon origin)
    {
        for (GridObject i = origin; GridObject.GetObjectAt(i.neighbors.upRight) != null; i = GridObject.GetObjectAt(i.neighbors.upRight))
        {
            if (GridObject.GetObjectAt(i.neighbors.down) != null)
                i = GridObject.GetObjectAt(i.neighbors.down);

            //for (GridObject j = i; GridObject.GetObjectAt(j.neighbors.up) != null; j = GridObject.GetObjectAt(j.neighbors.up))
            for (GridObject j = i; j != null; j = GridObject.GetObjectAt(j.neighbors.up))
            {
                if(GridObject.GetObjectAt(j.neighbors.downRight) != null && GridObject.GetObjectAt(j.neighbors.upRight) != null) //first
                {
                    List<GridObject> hexagonGroup = new List<GridObject>
                    {
                        j,
                        GridObject.GetObjectAt(j.neighbors.downRight),
                        GridObject.GetObjectAt(j.neighbors.upRight)
                    };
                    TryRecolor(hexagonGroup);
                }

                if (GridObject.GetObjectAt(j.neighbors.up) != null) //second
                {
                    List<GridObject> hexagonGroup = new List<GridObject>
                    {
                        j,
                        GridObject.GetObjectAt(j.neighbors.up),
                        GridObject.GetObjectAt(j.neighbors.upRight)
                    };
                    TryRecolor(hexagonGroup);
                }
            }
        }
    }

    /// <summary>
    /// Recolors the third hexagon if a match occurs.
    /// </summary>
    /// <param name="hexagonGroup"> Hexagon group to check. </param>
    private void TryRecolor(List<GridObject> hexagonGroup)
    {
        //Compare the colors. If all match, change the third.
        if(CheckColorMatch(hexagonGroup))
        {
            List<Color> randomColorList = new List<Color>(colorArray);
            randomColorList.Remove(hexagonGroup[2].color);
            hexagonGroup[2].color = GetRandomColor(randomColorList);
        }
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Checks if the colors of the provided hexagon group match.
    /// </summary>
    /// <returns> True if colors match. False if colors do not match.</returns>
    public bool CheckColorMatch(List<GridObject> hexagonGroup)
    {
        if (hexagonGroup[0].color == hexagonGroup[1].color &&
            hexagonGroup[1].color == hexagonGroup[2].color)
            return true; //All colors match.
        return false; //At least one color doesn't match.
    }

    /// <summary>
    /// Returns a random color from the initial color array.
    /// </summary>
    public Color GetRandomColor()
    {
        return colorArray[Random.Range(0, colorArray.Length)];
    }

    /// <summary>
    /// Returns a random color from the color list provided.
    /// </summary>
    public Color GetRandomColor(List<Color> colorList)
    {
        return colorList[Random.Range(0, colorList.Count)];
    }
    #endregion
}
