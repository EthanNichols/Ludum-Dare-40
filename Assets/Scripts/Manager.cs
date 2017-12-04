using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    //The amount of players, and their fields
    public int players = 1;
    public List<GameObject> playerFields = new List<GameObject>();

    public int GameState = 0;
    private int lastState = 0;

    public List<Sprite> numbers = new List<Sprite>();

    public GameObject mainMenu;
    private int currentButton = 0;

    //Field prefab and canvas object
    public GameObject PlayField;
    public GameObject canvas;

    public GameObject scorePanel;
    public GameObject infoPanel;
    public GameObject breakPanel;

    private int highScore = 1;
    public int score;

    public int level = 1;
    public float speed = 1;
    public int viruses = 1;

    //Whether the game has been started or not
    private bool gameStarted = false;

    // Use this for initialization
    void Start()
    {
        if (level > 20) { level = 20; }

        CreateFields();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState == 1)
        {
            SetStart();
            UpdateUI();
        }

        if (GameState == 0)
        {
            MainMenu();
        }
    }

    private void MainMenu()
    {
        GameObject buttons = mainMenu.transform.Find("Buttons").gameObject;

        string sLevel = level.ToString();
        GameObject levelUI = mainMenu.transform.Find("Level").gameObject;

        for (int i = 0; i < sLevel.Length; i++)
        {
            if (sLevel.Length == 1) { levelUI.transform.Find((i + 1).ToString()).GetComponent<Image>().sprite = numbers[0]; }
            levelUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[int.Parse(sLevel.Substring(sLevel.Length - (i + 1), 1))];
        }

        foreach (Transform button in buttons.transform)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentButton--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentButton++;
        }

        if (currentButton >= 3) { currentButton = 0; }
        if (currentButton < 0) { currentButton = 2; }

        buttons.transform.GetChild(currentButton).GetComponent<Image>().color = Color.green;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentButton == 0) { level--; }
            if (currentButton == 1) { level++; }
            if (currentButton == 2) { GameState = 1; mainMenu.SetActive(false); }

            if (level < 0) { level = 0; }
            if (level > 20) { level = 20; }
        }
    }

    private void UpdateUI()
    {
        if (score > highScore)
        {
            highScore = score;
        }

        GameObject highScoreUI = scorePanel.transform.Find("High").gameObject;
        GameObject scoreUI = scorePanel.transform.Find("Score").gameObject;

        GameObject levelUI = infoPanel.transform.Find("Level").gameObject;
        GameObject speedUI = infoPanel.transform.Find("Speed").gameObject;
        GameObject virusesUI = infoPanel.transform.Find("Viruses").gameObject;

        string sHighScore = highScore.ToString();
        string sScore = score.ToString();
        string slevel = level.ToString();
        string sViruses = viruses.ToString();

        for (int i = 0; i < sHighScore.Length; i++)
        {
            highScoreUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[int.Parse(sHighScore.Substring(sHighScore.Length - (i + 1), 1))];
        }

        if (score == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                scoreUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[0];
            }
        }

        for (int i = 0; i < sScore.Length; i++)
        {
            scoreUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[int.Parse(sScore.Substring(sScore.Length - (i + 1), 1))];
        }

        for (int i = 0; i < slevel.Length; i++)
        {
            if (slevel.Length == 1) { levelUI.transform.Find((i + 1).ToString()).GetComponent<Image>().sprite = numbers[0]; }
            levelUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[int.Parse(slevel.Substring(slevel.Length - (i + 1), 1))];
        }

        speed = 1 - (.05f * level);

        string sSpeed = "HI";
        if (speed > .7f) { sSpeed = "LOW"; }
        else if (speed > .4f) { sSpeed = "MED"; }
        speedUI.transform.Find("Speed").GetComponent<Text>().text = sSpeed;

        for (int i = 0; i < sViruses.Length; i++)
        {
            if (sViruses.Length == 1) { virusesUI.transform.Find((i + 1).ToString()).GetComponent<Image>().sprite = numbers[0]; }
            virusesUI.transform.Find(i.ToString()).GetComponent<Image>().sprite = numbers[int.Parse(sViruses.Substring(sViruses.Length - (i + 1), 1))];
        }
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

            newField.GetComponent<FieldGrid>().manager = this;

            if (players == 1)
            {
                scorePanel = Instantiate(scorePanel);
                scorePanel.transform.SetParent(newField.transform);
                scorePanel.transform.localScale = new Vector3(.25f, .25f);

                scorePanel.transform.localPosition = new Vector3(-74, 45);

                infoPanel = Instantiate(infoPanel);
                infoPanel.transform.SetParent(newField.transform);
                infoPanel.transform.localScale = new Vector3(.25f, .25f);

                infoPanel.transform.localPosition = new Vector3(74, -37.5f);

                breakPanel = Instantiate(breakPanel);
                breakPanel.transform.SetParent(newField.transform);
                breakPanel.transform.localScale = new Vector3(.25f, .25f);

                breakPanel.transform.localPosition = Vector3.zero;
                breakPanel.name = "Break";
            }

            //Add the field to a list of fields
            playerFields.Add(newField);

            newField.SetActive(false);
        }
    }

    /// <summary>
    /// Start the game by spawning viruses on all playing fields
    /// </summary>
    private void SetStart()
    {
        if (lastState == 1) { return; }

        //If the game has started don't start it again
        if (gameStarted) { return; }

        //Set a random seed
        int setSeed = Random.Range(0, 1000);

        //Start each map giving the same speed and difficulty
        foreach (GameObject field in playerFields)
        {
            field.SetActive(true);
            field.GetComponent<FieldGrid>().currentLevel = level;
        }

        //Set that the game has started
        gameStarted = true;
    }
}
