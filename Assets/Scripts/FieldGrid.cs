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

    //The pill prefab
    public GameObject pillPrefab;

    //The next playing pill, the current playing pill, and the spawn position 
    private GameObject nextPill = null;
    private GameObject playingPill = null;
    private Vector2 pillSpawnPos;

    //The list of peices that are moving
    public List<GameObject> movingPieces = new List<GameObject>();

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

        if (movingPieces.Count > 0)
        {
            //Don't throw any exeptions trying to remove null from the list
            try
            {
                if (movingPieces.Contains(null))
                {
                    movingPieces.RemoveAll(null);
                }
            }
            catch (System.Exception)
            {
            }
        }
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
        //Set the Random starting seed
        Random.InitState(seed);

        //Loop through the amount of enemies that should be created
        for (int i = 0; i < 10; i++)
        //for (int i = 0; i < Random.Range(2 * difficulty, 10 * difficulty); i++)
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

    /// <summary>
    /// Spawn a pill in the play area
    /// </summary>
    private void SpawnPill()
    {
        if (movingPieces.Count != 0) { return; }

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

        //Set that there is no playing pill if the pill component doesn't exist
        if (!playingPill.GetComponent<Pill>())
        {
            playingPill = null;
        }
    }

    /// <summary>
    /// Determine if an object is indside the play area or not
    /// </summary>
    /// <param name="pos">The position the object will be</param>
    /// <returns></returns>
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

    /// <summary>
    /// Remove rows of 4 or more of the same color
    /// </summary>
    /// <param name="startingPos">The last placed pill piece</param>
    public void RemoveMatches(Vector2 startingPos)
    {
        //If the piece doesn't extist retun, else set the peice color
        if (!gameObjects[startingPos]) { return; }
        string colorTag = gameObjects[startingPos].tag;

        //List of matches in both directions
        List<Vector2> horizontal = new List<Vector2>();
        List<Vector2> vertical = new List<Vector2>();
        //Add the starting position to both lists
        horizontal.Add(startingPos);
        vertical.Add(startingPos);

        //Current index, and whether the left/right can be checked
        int i = 1;
        bool checkLeft = true;
        bool checkRight = true;
        while (true)
        {
            //Whether a match is made or not
            bool match = false;

            //Make sure the index in the play area and left can be checked
            if (startingPos.x - i >= 0 && checkLeft)

                //Make sure an object is at the position and the tags match
                if (gameObjects[startingPos - new Vector2(i, 0)])
                {
                    if (gameObjects[startingPos - new Vector2(i, 0)].tag == colorTag)
                    {
                        //Add the position to the list and set that a match is made
                        horizontal.Add(startingPos - new Vector2(i, 0));
                        match = true;
                    }

                    //If something goes wrong never check the left again
                    else { checkLeft = false; }
                }
                else { checkLeft = false; }

            //Make sure the index in the play area and right can be checked
            if (startingPos.x + i < gridWidth && checkRight)

                //Make sure an object is at the position and the tags match
                if (gameObjects[startingPos + new Vector2(i, 0)])
                {
                    if (gameObjects[startingPos + new Vector2(i, 0)].tag == colorTag)
                    {
                        //Add the position to the list and set that a match is made
                        horizontal.Add(startingPos + new Vector2(i, 0));
                        match = true;
                    }

                    //If something goes wrong never check the right again
                    else { checkRight = false; }
                }
                else { checkRight = false; }

            //Increase the index, and break if a match isn't made
            i++;
            if (!match) { break; }
        }

        //Current index, and whether the up/down can be checked
        i = 1;
        bool checkup = true;
        bool checkDown = true;
        while (true)
        {
            //Whether a match is made or not
            bool match = false;

            //Make sure the index in the play area and up can be checked
            if (startingPos.y - i >= 0 && checkup)

                //Make sure an object is at the position and the tags match
                if (gameObjects[startingPos - new Vector2(0, i)])
                {
                    if (gameObjects[startingPos - new Vector2(0, i)].tag == colorTag)
                    {
                        //Add the position to the list and set that a match is made
                        vertical.Add(startingPos - new Vector2(0, i));
                        match = true;
                    }

                    //If something goes wrong never check the up again
                    else { checkup = false; }
                }
                else { checkup = false; }

            //Make sure the index in the play area and down can be checked
            if (startingPos.y + i < gridHeight && checkDown)

                //Make sure an object is at the position and the tags match
                if (gameObjects[startingPos + new Vector2(0, i)]) {
                    if (gameObjects[startingPos + new Vector2(0, i)].tag == colorTag)
                    {
                        //Add the position to the list and set that a match is made
                        vertical.Add(startingPos + new Vector2(0, i));
                        match = true;
                    }

                    //If something goes wrong never check the down again
                    else { checkDown = false; }
                }
                else { checkDown = false; }

            //Increase the index, and break if a match isn't made
            i++;
            if (!match) { break; }
        }

        //Test if the list has more than 4 elements
        if (horizontal.Count >= 4)
        {
            //Destroy all the objects and set the position to have nothing
            foreach (Vector2 pos in horizontal)
            {
                Destroy(gameObjects[pos]);
                gameObjects[pos] = null;
            }
        }

        //Test if the list has more than 4 elements
        if (vertical.Count >= 4)
        {
            //Destroy all the objects and set the position to have nothing
            foreach (Vector2 pos in vertical)
            {
                Destroy(gameObjects[pos]);
                gameObjects[pos] = null;
            }
        }
    }
}
