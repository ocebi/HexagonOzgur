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

    private List<Vector2> buttonPositions = new List<Vector2>();
    

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
        //RectTransform UI_Element;
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

        //float tempIncrement = 0;
        for (int x = 0; x < columns; ++x)
        {
            
            for (int y = 0; y < rows; ++y)
            {
                float xPosition = transform.position.x + (width * x);
                xPosition = xPosition * 0.8f;
                float yPosition = transform.position.y + (height * y);
                
                if (x % 2 == 1)
                {
                    //xPosition -= 0.15f;
                    yPosition -= 0.3f;
                    
                }
                /*
                if(x % 2 == 0)
                {
                    //xPosition -= tempIncrement;
                }
                */
                
                
                GameObject newGem = Instantiate(
                  hexagon,
                  new Vector3(xPosition, yPosition, 0),
                  hexagon.transform.rotation
                );
                newGem.name = x + "," + y;
                
                //spawn buttons
                if (x != 0 && y != 0)
                {
                    if(y != columns)
                    { 
                        GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject2.transform.SetParent(canvas.transform, true);

                        //buttonPositions.Add(new Vector2(xPosition - 0.38f, yPosition));

                        GameObject testgameObject = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.2f, yPosition - 0.34f, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject.transform.SetParent(canvas.transform, true);
                        //buttonPositions.Add(new Vector2(xPosition - 0.35f, yPosition - 0.34f));
                    }
                    else
                    {
                        GameObject testgameObject = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.2f, yPosition - 0.34f, 0),
                        buttonPrefab.transform.rotation);
                        testgameObject.transform.SetParent(canvas.transform, true);
                        //buttonPositions.Add(new Vector2(xPosition - 0.35f, yPosition - 0.34f));

                        //CheckCollision(new Vector2(xPosition - 0.35f, yPosition - 0.34f), 0.5f);

                        if(x % 2 == 1)
                        {
                            GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                            testgameObject2.transform.SetParent(canvas.transform, true);
                            //buttonPositions.Add(new Vector2(xPosition - 0.38f, yPosition));
                        }
                    }
                    
                    
                }
                
                else if(x != 0 && y == 0 && (x % 2 == 0))
                {
                    GameObject testgameObject2 = Instantiate(buttonPrefab,
                        new Vector3(xPosition - 0.38f, yPosition, 0),
                        buttonPrefab.transform.rotation);
                    testgameObject2.transform.SetParent(canvas.transform, true);
                    //buttonPositions.Add(new Vector2(xPosition - 0.38f, yPosition));
                }
                
                hexagons[x, y] = newGem;

                newGem.transform.parent = transform;
                //newGem.GetComponent<SpriteRenderer>().sprite = hexagonSprites[Random.Range(0, hexagonSprites.Count)];
                Color32 randomColor = colorArray[Random.Range(0, colorArray.Length)];
                newGem.GetComponent<SpriteRenderer>().color = new Color32(randomColor.r, randomColor.g, randomColor.b, 255);//colorArray[Random.Range(0, colorArray.Length)];
            }
            /*
            if (x % 4 == 0)
            {
                tempIncrement += 0.3f;
            }
            */
        }

        //CheckCollision(new Vector3(0, 0, 0), 100);
        /*
        for(int x = 0; x < columns; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {

            }
        }
        */
        /*
        foreach(Vector2 vec2 in buttonPositions)
        {
            CheckCollision(vec2, 0.2f);
        }
        print("Total: " + buttonPositions.Count);
        */
    }
    /*
    void CheckCollision(Vector2 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        int i = 0;
        foreach (var hitCollider in hitColliders)
        {
            print("Collider" + i + ": " + hitCollider.gameObject.name);
            ++i;
            hitCollider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    */
    /*
    public void OnButtonClicked(Vector2 buttonPosition)
    {

    }
    */

}
