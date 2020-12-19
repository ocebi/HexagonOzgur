using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public GameObject hexagonPrefab;
    public GameObject bombPrefab;

    [SerializeField]
    private int hexagonSpawnChance = 1000;
    [SerializeField]
    private int spawnBombAt = 1000;
    private int bombSpawnChance = 0;
    private int activeBombs { get { return GameObject.FindObjectsOfType<Bomb>().Length; } }
    

    private Stack<Hexagon> hexagonPool = new Stack<Hexagon>();
    private Stack<Bomb> bombPool = new Stack<Bomb>();

    #region Unity Methods
    private void OnEnable()
    {
        ExplosionManager.OnExplosionDetected += SpawnRandomObjectsAt;
    }

    private void OnDisable()
    {
        ExplosionManager.OnExplosionDetected -= SpawnRandomObjectsAt;
    }
    #endregion

    private void Start()
    {
        List<Bomb> tempBombList = new List<Bomb>();
        for (int i = 0; i < 10; ++i)
        {
            var bomb = SpawnBomb(-10, -10);
            tempBombList.Add(bomb.GetComponent<Bomb>());
        }
        foreach(var bomb in tempBombList)
        {
            AddToBombPool(bomb);
            bomb.gameObject.SetActive(false);
        }
    }

    #region Public Methods

    /// <summary>
    /// Either instantiates a hexagon or pops one from the pool.
    /// </summary>
    /// <param name="xPosition"> x-position to spawn. </param>
    /// <param name="yPosition"> y-position to spawn. </param>
    /// <returns> Spawned hexagon gameobject. </returns>
    public GameObject SpawnHexagon(float xPosition, float yPosition)
    {
        if (hexagonPool.Count > 0)
        {
            var hexagon = hexagonPool.Pop().gameObject;
            hexagon.transform.position = new Vector3(xPosition, yPosition, hexagon.transform.position.z);
            hexagon.SetActive(true);
            hexagon.GetComponent<SpriteRenderer>().color = GridColorer.Instance.GetRandomColor();
            Debug.Log("Spawned hexagon from pool");
            return hexagon;
        }
        else
        {
            Debug.Log("Creating new hexagon");
            GameObject newHexagon = Instantiate(
                      hexagonPrefab,
                      new Vector3(xPosition, yPosition, 0),
                      Quaternion.identity
                    );
            newHexagon.transform.SetParent(GridGenerator.Instance.transform);
            newHexagon.GetComponent<SpriteRenderer>().color = GridColorer.Instance.GetRandomColor();
            return newHexagon;
        }
    }

    /// <summary>
    /// Either instantiates a bomb or pops one from the pool.
    /// </summary>
    /// <param name="xPosition"> x-position to spawn. </param>
    /// <param name="yPosition"> y-position to spawn. </param>
    /// <returns> Spawned bomb gameobject. </returns>
    public GameObject SpawnBomb(float xPosition, float yPosition)
    {
        if (bombPool.Count > 0)
        {
            var bomb = bombPool.Pop().gameObject;
            bomb.transform.position = new Vector3(xPosition, yPosition, bomb.transform.position.z);
            bomb.SetActive(true);
            bomb.GetComponent<SpriteRenderer>().color = GridColorer.Instance.GetRandomColor();
            Debug.Log("Spawned bomb from pool");
            return bomb;
        }
        else
        {
            Debug.Log("Creating new bomb");
            GameObject newBomb = Instantiate(
                      bombPrefab,
                      new Vector3(xPosition, yPosition, 0),
                      Quaternion.identity
                    );
            newBomb.transform.SetParent(GridGenerator.Instance.transform);
            newBomb.GetComponent<SpriteRenderer>().color = GridColorer.Instance.GetRandomColor();
            return newBomb;
        }
    }
    /// <summary>
    /// Adds the object to the hexagon pool.
    /// </summary>
    public void AddToHexagonPool(Hexagon hexagon)
    {
        hexagonPool.Push(hexagon);
        Debug.Log("Added to hexagon pool");
    }
    /// <summary>
    /// Adds the object to the bomb pool.
    /// </summary>
    public void AddToBombPool(Bomb bomb)
    {
        bombPool.Push(bomb);
        Debug.Log("Added to bomb pool");
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Spawns random objects at the given explosion positions.
    /// </summary>
    private void SpawnRandomObjectsAt(List<Vector2> explosionPositions)
    {
        List<GridObject> objectsToFall = new List<GridObject>();
        foreach(var pos in explosionPositions)
        {
            if(DetermineObjectType() == 0)
            {
                var go = SpawnHexagon(pos.x, pos.y + 20 * GridObject.HALF_VERTICAL);
                objectsToFall.Add(go.GetComponent<GridObject>());
            }
            else
            {
                var go = SpawnBomb(pos.x, pos.y + 20 * GridObject.HALF_VERTICAL);
                objectsToFall.Add(go.GetComponent<GridObject>());
            }

            
        }
        List<GridObject> moveDownList = new List<GridObject>();
        moveDownList = FillMoveList(objectsToFall);
        ExplosionManager.Instance.SetupBlocks(moveDownList);
    }

    /// <summary>
    /// Determines which objects are the block heads to move down.
    /// </summary>
    private List<GridObject> FillMoveList(List<GridObject> spawnedObjects)
    {
        List<GridObject> moveList = new List<GridObject>();

        foreach (var go in spawnedObjects)
        {
            go.neighbors = go.GetNeighbors();
            GridObject down = GridObject.GetObjectAt(go.neighbors.down);
            if (down == null)
            {
                moveList.Add(go);
            }
        }
        return moveList;
    }

    /// <summary>
    /// Determines whether the spawned object will be a hexagon or a bomb.
    /// </summary>
    /// <returns> Object type. </returns>
    private int DetermineObjectType()
    {
        if (ScoreManager.Instance.score >= spawnBombAt)
        {
            if (bombSpawnChance != 0 && activeBombs > 1)
            {
                bombSpawnChance += 10 / activeBombs;
            }
            else if (activeBombs == 1)
            {
                bombSpawnChance += 10;
            }
            else
            {
                bombSpawnChance += 20;
            }

            int spawnChance = Random.Range(0, hexagonSpawnChance + bombSpawnChance);
            if (spawnChance < hexagonSpawnChance)
            {
                return 0;
            }
            else if (spawnChance < hexagonSpawnChance + bombSpawnChance)
            {
                bombSpawnChance = 0;
                return 1;
            }
            else return 0;
        }
        else return 0;
    }
    #endregion

}
