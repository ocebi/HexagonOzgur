using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public int rows, columns;
    private GameObject[,] hexagons;

    //public List<Sprite> hexagonSprites = new List<Sprite>();

    public GameObject hexagon;

    public Color32[] colorArray;

    void Start()
    {
        instance = GetComponent<GridManager>();
        Vector2 hexDimensions = hexagon.GetComponent<SpriteRenderer>().bounds.size;
        GenerateGrid(hexDimensions.x, hexDimensions.y);
    }

    

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }
        */
    }

    private void GenerateGrid(float width, float height)
    {
        hexagons = new GameObject[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float xPosition = transform.position.x + (width * x);
                float yPosition = transform.position.y + (height * y);
                if (x % 2 != 1)
                {
                    yPosition = transform.position.y + (width * y + 0.3f);
                }

                GameObject newGem = Instantiate(
                  hexagon,
                  new Vector3(xPosition, yPosition, 0),
                  hexagon.transform.rotation
                );

                hexagons[x, y] = newGem;

                newGem.transform.parent = transform;
                //newGem.GetComponent<SpriteRenderer>().sprite = hexagonSprites[Random.Range(0, hexagonSprites.Count)];
                Color32 randomColor = colorArray[Random.Range(0, colorArray.Length)];
                newGem.GetComponent<SpriteRenderer>().color = new Color32(randomColor.r, randomColor.g, randomColor.b, 255);//colorArray[Random.Range(0, colorArray.Length)];
            }
        }
    }
    /*
    private void Clicked()
    {
        print("here");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point.ToString());
        }
    }
    */
}
