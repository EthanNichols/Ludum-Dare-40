using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    //The amount of players, and their fields
    public int players = 1;
    public List<GameObject> playerFields = new List<GameObject>();

    //Field prefab and canvas object
    public GameObject PlayField;
    public GameObject canvas;

    //Whether the game has been started or not
    private bool gameStarted = false;

	// Use this for initialization
	void Start () {
        CreateFields();
	}
	
	// Update is called once per frame
	void Update () {
        SetStart();
	}

    /// <summary>
    /// Create all of the fields for the players to play on
    /// </summary>
    private void CreateFields()
    {
        //Get the size of the window
        Vector2 windowSize = new Vector2(
            canvas.GetComponent<RectTransform>().rect.width,
            canvas.GetComponent<RectTransform>().rect.height);

        //Loop through all of the players
        for (int i = 0; i < players; i++)
        {
            //Create a new field
            GameObject newField = Instantiate(PlayField);

            //Set the parent and the position of the field
            newField.transform.SetParent(canvas.transform);
            newField.transform.localPosition = new Vector2((-windowSize.x / 2) + (windowSize.x / players) * (i + .5f), 0);

            //Add the field to a list of fields
            playerFields.Add(newField);
        }
    }

    /// <summary>
    /// Start the game by spawning viruses on all playing fields
    /// </summary>
    private void SetStart()
    {
        //If the game has started don't start it again
        if (gameStarted) { return; }

        //Set a random seed
        int setSeed = Random.Range(0, 1000);

        //Start each map giving the same speed and difficulty
        foreach(GameObject field in playerFields)
        {
            field.GetComponent<FieldGrid>().SpawnEnemies(setSeed, 20);
        }
        
        //Set that the game has started
        gameStarted = true;
    }
}
