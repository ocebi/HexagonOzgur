using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : Singleton<GridGenerator>
{
    public int columns;
    public int rows;
    public GridObject gridOrigin { 
        get { return GridObject.GetObjectAt(new Vector3(transform.position.x, transform.position.y, 0)); }
    }

    private void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// Generates the grid with respect to the rows and columns.
    /// </summary>
    private void GenerateGrid()
    {
        for (int x = 0; x < columns; ++x)
        {

            for (int y = 0; y < rows; ++y)
            {
                float xPosition = transform.position.x + (GridObject.HALF_HORIZONTAL * x);
                float yPosition = transform.position.y + (GridObject.VERTICAL_GRID_DISTANCE * y);

                if (x % 2 == 1)
                {
                    yPosition -= GridObject.HALF_VERTICAL; //Lower the y-value for the layout
                }

                SpawnManager.Instance.SpawnHexagon(xPosition, yPosition);
            }
        }

        CenterGrid();
    }

    /// <summary>
    /// Sums up x-values of the bottom line of hexagons.
    /// Division by columns gives the deflection from the center.
    /// Centers the grid with respect to the deflection.
    /// </summary>
    private void CenterGrid()
    {
        float totalX = 0;

        for (Hexagon i = (Hexagon)gridOrigin; i != null; i = (Hexagon)GridObject.GetObjectAt(i.neighbors.upRight))
        {
            var down = (Hexagon)GridObject.GetObjectAt(i.neighbors.down);
            if (down)
            {
                i = down;
            }

            totalX += i.gameObject.transform.position.x;
        }
        transform.position -= new Vector3(totalX / columns, 0, 0);

        GridManager.OnGridGenerated.Invoke((Hexagon)gridOrigin);
    }
}
