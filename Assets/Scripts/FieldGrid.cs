using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{

    //The gridPos and the actual position
    public Dictionary<Vector2, Vector2> gridPos = new Dictionary<Vector2, Vector2>();

    //The gridPos and the object at that position
    public Dictionary<Vector2, GameObject> gameObjects = new Dictionary<Vector2, GameObject>();

    //The different enemies that can spawn
    public List<GameObject> enemies = new List<GameObject>();

    public GameObject pillPrefab;
    private GameObject nextPill = null;
    private GameObject playingPill = null;
    private Vector2 pillSpawnPos;

    //The size of the field
    private const int gridSize = 8;
    private const int gridWidth = 8;
    private const int gridHeight = 16;

    // Use this for initialization
    void Start()
    {
        SetupGrid();

        SpawnPill();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnPill();
    }

    /// <summary>
    /// Get the gridPos and the exact position of the tile
    /// </summary>
    private void SetupGrid()
    {
        //Set the starting position for the grid
        Vector2 startingPos = new Vector2(
            transform.localPosition.x - (((gridWidth - 1) / 2f) * gridSize * transform.localScale.x),
            transform.localPosition.y + (((gridHeight - 5) / 2f) * gridSize * transform.localScale.y));

        //Loop through the width and height of the field
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {

                //Calculate the offset of the tile
                Vector2 offset = new Vector2(x * gridSize, -1 * y * gridSize) * transform.localScale.x;

                //Set the gridPos' actual position and set a null object at that position
                gridPos.Add(new Vector2(x, y), startingPos + offset);
                gameObjects.Add(new Vector2(x, y), null);
            }
        }

        //Calculate the position for the pill to start at
        pillSpawnPos = new Vector2(transform.localPosition.x, transform.localPosition.y + ((gridHeight + 2.5f) / 2f) * (gridSize * transform.localScale.y));
    }

    /// <summary>
    /// Spawn in the enemies for the player to remove
    /// </summary>
    /// <param name="seed">The seed that enemeies will be spawn with</param>
    /// <param name="difficulty">The difficulty the player is playing at</param>
    public void SpawnEnemies(int seed, int difficulty = 1)
    {
        return;
        //Set the Random starting seed
        Random.InitState(seed);

        //Loop through the amount of enemies that should be created
        for (int i = 0; i < Random.Range(2 * difficulty, 10 * difficulty); i++)
        {
            //Create a new enemy
            GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count)]);

            //Set the parent to the canvas, and the scale of the enemy
            enemy.transform.SetParent(transform.parent);
            enemy.transform.localScale = transform.localScale;

            //Get a random grid position until there isn't an enemy in that position
            Vector2 setGridPos = new Vector2((int)Random.Range(0, gridWidth), (int)Random.Range(3, gridHeight));
            while (gameObjects[setGridPos] != null)
            {
                setGridPos = new Vector2((int)Random.Range(0, gridWidth), (int)Random.Range(3, gridHeight));
            }

            //Set that an enemy is at that grid position
            gameObjects[setGridPos] = enemy;

            //Set the position of the enemy and make it a child of the field
            enemy.transform.localPosition = gridPos[setGridPos];
            enemy.transform.SetParent(transform);
        }
    }

    private void SpawnPill()
    {
        //Determing if the playing pill is still being controlled
        if (nextPill != null &&
            playingPill == null)
        {
            playingPill = nextPill;
            playingPill.GetComponent<Pill>().playing = true;
            nextPill = null;
        }

        //Return if there is still a pill being controlled
        if (nextPill == null)
        {

            //Create a new pill
            nextPill = Instantiate(pillPrefab);

            //Set the position and scale of the pill
            nextPill.transform.SetParent(transform.parent);
            nextPill.transform.localScale = transform.localScale;
            nextPill.transform.localPosition = pillSpawnPos;

            //Set the play field for the pill
            nextPill.GetComponent<Pill>().field = this;
            nextPill.GetComponent<Pill>().playing = false;

            return;
        }

        if (!playingPill.GetComponent<Pill>())
        {
            playingPill = null;
        }
    }

    public bool InBounds(Vector2 pos)
    {
        if (pos.x < 0 ||
            pos.x >= gridWidth ||
            pos.y < 0 ||
            pos.y >= gridHeight)
        {
            return false;
        }

        return true;
    }
}
