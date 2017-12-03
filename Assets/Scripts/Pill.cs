using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pill : MonoBehaviour
{

    public List<Sprite> pillHalves = new List<Sprite>();
    public List<Sprite> pillPieces = new List<Sprite>();

    private Vector2 gridPos1;
    private Vector2 gridPos2;
    public FieldGrid field;

    public bool playing = false;

    public float fallTimer;
    private float fallTimerReset;

    public float stillTimer;
    private float stillTimerReset;

    public float moveTimer;
    private float moveDownReset;

    private int rotation;

    // Use this for initialization
    void Start()
    {
        gridPos1 = new Vector2(3, 0);
        gridPos2 = new Vector2(4, 0);

        if (field.gameObjects[gridPos1] != null ||
            field.gameObjects[gridPos2] != null)
        {
            field.lost = true;
        } 

        //if (seed != 0) { Random.InitState(seed); }

        //Set a random sprite for both sides of the pill
        foreach (Transform child in transform)
        {
            //Set a random sprite for half od the pill
            int sprite = Random.Range(0, pillHalves.Count);
            child.GetComponent<Image>().sprite = pillHalves[sprite];

            //Set the tag to the color of the pill
            if (child.GetComponent<Image>().sprite.name.Contains("Blue")) { child.tag = "Blue"; }
            if (child.GetComponent<Image>().sprite.name.Contains("Red")) { child.tag = "Red"; }
            if (child.GetComponent<Image>().sprite.name.Contains("Yellow")) { child.tag = "Yellow"; }

            //Set the pill piece sprite, and the timers for the pill piece
            child.GetComponent<PillPiece>().pieceSprite = pillPieces[sprite];
            child.GetComponent<PillPiece>().fallTimer = fallTimer;
            child.GetComponent<PillPiece>().stillTimer = stillTimer;
            child.GetComponent<PillPiece>().field = field;
        }

        //Set the reset timer values

        fallTimerReset = fallTimer - (.01f * field.currentLevel);
        stillTimerReset = stillTimer - (.01f * field.currentLevel);
        moveDownReset = moveTimer;

        fallTimer = fallTimerReset;
        stillTimer = stillTimerReset;

        Debug.Log(fallTimerReset);
    }

    // Update is called once per frame
    void Update()
    {
        //If the pill is not in play don't update it
        if (!playing) { return; }

        PillFall();
        ControlPill();
        SetPosition();

        PlacePill();

        if (field.gameOver)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Place the pill at its final resting place
    /// </summary>
    private void PlacePill()
    {
        //Test if the pill has stayed still for a long time
        if (stillTimer <= 0)
        {
            //Get the left and right half of the pill
            GameObject left = transform.Find("Left").gameObject;
            GameObject right = transform.Find("Right").gameObject;

            //Set the position the halfs occupy on the grid
            field.GetComponent<FieldGrid>().gameObjects[gridPos1] = left;
            field.GetComponent<FieldGrid>().gameObjects[gridPos2] = right;

            //Check if there are any matches
            field.GetComponent<FieldGrid>().RemoveMatches(gridPos1);
            field.GetComponent<FieldGrid>().RemoveMatches(gridPos2);

            //Set the position of the individual pill pieces
            left.GetComponent<PillPiece>().gridPos = gridPos1;
            right.GetComponent<PillPiece>().gridPos = gridPos2;

            //Destryo this script
            Destroy(this);
        }
    }

    /// <summary>
    /// Control the pill before it is locked into place
    /// </summary>
    private void ControlPill()
    {
        //Set the timer to 0 on the first frame a movement key is pressed
        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.D)) { moveTimer = 0; }

        //Reset the timer when the key is held up
        if (Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D)) { moveTimer = moveDownReset; }

        //Reduce the time it takes to move when a movement key is held
        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D))
        {
            moveTimer -= Time.deltaTime;
        }

        //Make the key fall faster
        if (Input.GetKey(KeyCode.S))
        {
            //Make the timer run faster
            moveTimer -= Time.deltaTime;

            //Test if the timer is finished
            if (moveTimer <= 0)
            {
                //Move the pill if it returns true, set the pill
                if (!MovePill(new Vector2(0, 1))) { stillTimer = 0; }

                //Reset the fall timer and movement timer
                fallTimer = fallTimerReset;
                moveTimer = moveDownReset;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            //Test if the timer is finished
            if (moveTimer <= 0)
            {
                //Move the pill to the right and reset the movement timer
                if (MovePill(new Vector2(-1, 0))) { fallTimer += Time.deltaTime * 5; }
                moveTimer = moveDownReset;
            }

        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Test if the timer is finished
            if (moveTimer <= 0)
            {
                //Move the pill to the left and reset the fall timer
                if (MovePill(new Vector2(1, 0))) { fallTimer += Time.deltaTime * 5; }
                moveTimer = moveDownReset;
            }
        }

        //Rotate the pill Clockwise
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (RotatePill(1)) { fallTimer += Time.deltaTime * 5; }
            else if (RotatePill(2)) { fallTimer += Time.deltaTime * 5; }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Rotate the pill
            if (RotatePill(-1)) { fallTimer += Time.deltaTime * 5; }
            else if (RotatePill(-2)) { fallTimer += Time.deltaTime * 5; }
        }
    }

    /// <summary>
    /// Rotate the pill in a 2*2 square
    /// </summary>
    /// <param name="direction">The direction the pill will rotate</param>
    /// <returns></returns>
    private bool RotatePill(int direction)
    {
        //Get the next rotation the pill will be at
        int localRot = ((rotation + 360) + 90 * direction) % 360;

        //The new position for the pill
        Vector2 newPos1 = gridPos1;
        Vector2 newPos2 = gridPos2;

        if (direction > 0)
        {
            //Set the new position based on the rotation
            switch (localRot)
            {
                case 0:
                    newPos2 += new Vector2(1, 1);
                    break;

                case 90:
                    newPos1 -= new Vector2(0, 1);
                    newPos2 -= new Vector2(1, 0);
                    break;

                case 180:
                    newPos1 += new Vector2(1, 1);
                    break;

                case 270:
                    newPos1 -= new Vector2(1, 0);
                    newPos2 -= new Vector2(0, 1);
                    break;
            }
        } else
        {
            switch (localRot)
            {
                case 0:
                    newPos1 += new Vector2(0, 1);
                    newPos2 += new Vector2(1, 0);
                    break;

                case 90:
                    newPos1 -= new Vector2(1, 1);
                    break;

                case 180:
                    newPos1 += new Vector2(1, 0);
                    newPos2 += new Vector2(0, 1);
                    break;

                case 270:
                    newPos2 -= new Vector2(1, 1);
                    break;
            }
        }

        if (Mathf.Abs(direction) == 2)
        {
            Vector2 oldPos = gridPos1;
            newPos1 = gridPos2;
            newPos2 = oldPos;
            localRot = rotation + 180;
        }

        //Test if the movement will be in bounds
        if (field.InBounds(newPos1) &&
            field.InBounds(newPos2))
        {

            //Make sure there isn't an  object in the new position
            if ((field.gameObjects[newPos1] == null ||
                field.gameObjects[newPos1] == gameObject) &&
                (field.gameObjects[newPos2] == null ||
                field.gameObjects[newPos2] == gameObject))
            {
                //Rotate the pill
                rotation += (90 * direction);
                rotation %= 360;
                transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -rotation));

                //Remove the pill from the current position
                field.gameObjects[gridPos1] = null;
                field.gameObjects[gridPos2] = null;

                //Change the position of the pill
                gridPos1 = newPos1;
                gridPos2 = newPos2;

                //Set the new occupation of the pill
                field.gameObjects[gridPos1] = gameObject;
                field.gameObjects[gridPos2] = gameObject;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Move the pill in the direction passed as a parameter
    /// </summary>
    /// <param name="dir">The direction the pill will move towards</param>
    /// <returns></returns>
    private bool MovePill(Vector2 dir)
    {
        //Get the new positions of the pill
        Vector2 newPos1 = gridPos1 + dir;
        Vector2 newPos2 = gridPos2 + dir;

        //Test if the movement will be in bounds
        if (field.InBounds(newPos1) &&
            field.InBounds(newPos2))
        {

            //Make sure there isn't an  object in the new position
            if ((field.gameObjects[newPos1] == null ||
                field.gameObjects[newPos1] == gameObject) &&
                (field.gameObjects[newPos2] == null ||
                field.gameObjects[newPos2] == gameObject))
            {
                //Remove the pill from the current position
                field.gameObjects[gridPos1] = null;
                field.gameObjects[gridPos2] = null;

                //Change the position of the pill
                gridPos1 = newPos1;
                gridPos2 = newPos2;

                //Set the new occupation of the pill
                field.gameObjects[gridPos1] = gameObject;
                field.gameObjects[gridPos2] = gameObject;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Make the pill drop on a timer and keep track of the time that the pill stays still
    /// </summary>
    private void PillFall()
    {
        //Reduce the time before the next move
        fallTimer -= Time.deltaTime;

        //Test if the timer is completed
        if (fallTimer < 0)
        {
            //Reset the timer
            fallTimer = fallTimerReset;

            if (!MovePill(new Vector2(0, 1)))
            {
                //If the pill doesn't move start the still timer
                //Keep the movement timer at 0
                stillTimer -= Time.deltaTime;
                fallTimer = 0;
                return;
            }

            //Reset the timer for the pill staying still
            stillTimer = stillTimerReset;
        }
    }

    /// <summary>
    /// Calculate the actual position of the pill based off the spaces it occupies
    /// </summary>
    private void SetPosition()
    {
        //Get the two positions the pill occupies
        Vector2 pos1 = field.gridPos[gridPos1];
        Vector2 pos2 = field.gridPos[gridPos2];

        //Set the position to the average of the two positions
        transform.localPosition = (pos1 + pos2) / 2;
    }
}
