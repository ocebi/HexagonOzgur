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

    public Canvas canvas;

    public GameObject buttonPrefab; //debug
    

    void Start()
    {
        instance = GetComponent<GridManager>();
        Vector2 hexDimensions = hexagon.GetComponent<SpriteRenderer>().bounds.size;
        GenerateGrid(hexDimensions.x, hexDimensions.y);
    }

    

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            print(ray.ToString());
        }
        
    }

    private void GenerateGrid(float width, float height)
    {
        hexagons = new GameObject[columns, rows];

        //debug
        RectTransform UI_Element;
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

        for (int x = 0; x < columns; ++x)
        {
            for (int y = 0; y < rows; ++y)
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
                newGem.name = x + "," + y;

                //if(x % 2 != 1 && x != 0) //debug - this spawn logic might be unnecessary. try old logic with the world space canvas
                if (x != 0 && y != 0)
                {
                    /*
                    if(y == columns)
                    {
                        GameObject testgameObject = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.35f, yPosition - 0.34f, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject.transform.SetParent(canvas.transform, true);
                    }
                    else
                    {
                    */
                    if(y != columns)
                    { 
                        GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject2.transform.SetParent(canvas.transform, true);

                        GameObject testgameObject = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.35f, yPosition - 0.34f, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject.transform.SetParent(canvas.transform, true);
                    }
                    else
                    {
                        GameObject testgameObject = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.35f, yPosition - 0.34f, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject.transform.SetParent(canvas.transform, true);
                        if(x % 2 == 1)
                        {
                            GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                            testgameObject2.transform.SetParent(canvas.transform, true);
                        }
                    }
                    
                    
                }
                
                else if(x != 0 && y == 0 && (x % 2 == 0))
                {
                    GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                    testgameObject2.transform.SetParent(canvas.transform, true);
                }
                
                hexagons[x, y] = newGem;

                newGem.transform.parent = transform;
                //newGem.GetComponent<SpriteRenderer>().sprite = hexagonSprites[Random.Range(0, hexagonSprites.Count)];
                Color32 randomColor = colorArray[Random.Range(0, colorArray.Length)];
                newGem.GetComponent<SpriteRenderer>().color = new Color32(randomColor.r, randomColor.g, randomColor.b, 255);//colorArray[Random.Range(0, colorArray.Length)];
            }
        }
        /*
        for(int x = 0; x < columns; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {

            }
        }
        */
    }

    

}
