using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public static float HORIZONTAL_GRID_DISTANCE = 1.3f;
    public static float VERTICAL_GRID_DISTANCE = 0.7f;
    public static float HALF_HORIZONTAL = 0.65f;
    public static float HALF_VERTICAL = 0.35f;

    public abstract void ExplodeObject();

    public Neighbors neighbors;
    public Color color
    {
        get { return GetComponent<SpriteRenderer>().color; }
        set { GetComponent<SpriteRenderer>().color = value; }
    }

    private void Awake()
    {
        neighbors = GetNeighbors();
    }


    public struct Neighbors
    {
        public Vector2 upLeft;
        public Vector2 up;
        public Vector2 upRight;
        public Vector2 downLeft;
        public Vector2 down;
        public Vector2 downRight;
    }

    /// <summary>
    /// Returns the neighbors of the current grid object.
    /// </summary>
    public Neighbors GetNeighbors()
    {
        Neighbors neighbors;
        neighbors.upLeft = transform.position + new Vector3(-HALF_HORIZONTAL, HALF_VERTICAL);
        neighbors.up = transform.position + new Vector3(0, HALF_HORIZONTAL);
        neighbors.upRight = transform.position + new Vector3(HALF_HORIZONTAL, HALF_VERTICAL);
        neighbors.downLeft = transform.position + new Vector3(-HALF_HORIZONTAL, -HALF_VERTICAL);
        neighbors.down = transform.position + new Vector3(0, -HALF_HORIZONTAL);
        neighbors.downRight = transform.position + new Vector3(HALF_HORIZONTAL, -HALF_VERTICAL);

        return neighbors;
    }

    /// <summary>
    /// Returns the GridObject at the given coordinates if it exists.
    /// </summary>
    /// <param name="position"> Position of the object. </param>
    /// <returns> GridObject at the position. </returns>
    public static GridObject GetObjectAt(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<GridObject>();
        }
        else
            return null;
    }
}
