using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PillPiece : MonoBehaviour
{

    //The grid position and the sprite when it becomes a piece
    public Vector2 gridPos;
    public Sprite pieceSprite;

    //Timer between falling down a grid spot
    public float fallTimer;
    private float fallTimerReset;

    //Time the piece doesn't move to be placed
    public float stillTimer;
    private float stillTimerReset;

    //The field the piece is in
    public FieldGrid field;

    //Whether the piece can fall, or if it's apart of a pill still
    private bool allowFall = false;

    // Use this for initialization
    void Start()
    {

        //Set the timers
        fallTimerReset = fallTimer / 2;
        fallTimer = fallTimerReset;
        stillTimerReset = stillTimer / 2;
        stillTimer /= 2;
    }

    // Update is called once per frame
    void Update()
    {

        //Test if the piece isn't apart of a pill
        TurnToPiece();

        //Determin if the piece is still apart of a pill
        if (transform.parent.childCount != 1 &&
            !transform.parent.GetComponent<Pill>() &&
            transform.parent.name.Contains("Pill"))
        {
            //Test if the pill can fall
            MovePill(new Vector2(0, 1));
        }

        //Allow the peice to fall by itself
        if (allowFall)
        {
            PieceFall();
            SetPosition();
            PlacePiece();
        }

        //Wait for everything to stop moving before testing matches
        if (field.movingPieces.Count == 0)
        {
            field.GetComponent<FieldGrid>().RemoveMatches(gridPos);
        }

        if (field.gameOver)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Move the piece in the direction given
    /// </summary>
    /// <param name="dir">The direction the piece will move</param>
    /// <returns></returns>
    private bool MovePiece(Vector2 dir)
    {
        //Get the new position the piece will be
        Vector2 newPos = gridPos + dir;

        //Test if the new position is in the play area
        if (field.InBounds(newPos))
        {
            //Make sure there isn't anything in the new position
            if (field.gameObjects[newPos] == null)
            {
                //Set the current position to have nothing there
                field.gameObjects[gridPos] = null;

                //Set the new position and occupy the new space
                gridPos = newPos;
                field.gameObjects[gridPos] = gameObject;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Move the pill if it isn't a piece yet
    /// </summary>
    /// <param name="dir">The direction the pill will move</param>
    /// <returns></returns>
    private bool MovePill(Vector2 dir)
    {
        //Make sure to only test half of the pill
        if (name == "Right") { return false; }

        //Get the half that isn't being tested
        Vector2 rightPos = transform.parent.Find("Right").GetComponent<PillPiece>().gridPos;

        //Set the new positions for both halves of the pill
        Vector2 newPos1 = gridPos + dir;
        Vector2 newPos2 = rightPos + dir;

        //Reduce the time between falls
        fallTimer -= Time.deltaTime;

        //Test if the movement will be in bounds
        if (field.InBounds(newPos1) &&
            field.InBounds(newPos2))
        {

            //If the timer isn't down exit else reset the timer
            if (fallTimer > 0) { return false; }
            fallTimer = fallTimerReset;

            //Make sure there isn't an  object in the new position
            if ((field.gameObjects[newPos1] == null ||
                field.gameObjects[newPos1].transform.parent == transform.parent) &&
                (field.gameObjects[newPos2] == null ||
                field.gameObjects[newPos2].transform.parent == transform.parent))
            {
                //Remove the pill from the current position
                field.gameObjects[gridPos] = null;
                field.gameObjects[rightPos] = null;

                //Change the position of the pill
                gridPos = newPos1;
                rightPos = newPos2;
                transform.parent.Find("Right").GetComponent<PillPiece>().gridPos = newPos2;

                //Set the new occupation of the pill
                field.gameObjects[gridPos] = gameObject;
                field.gameObjects[rightPos] = transform.parent.Find("Right").gameObject;

                //Set the position of the pill to the average of the new positions
                transform.parent.localPosition = (field.gridPos[gridPos] + field.gridPos[rightPos]) / 2;

                if (!field.movingPieces.Contains(gameObject))
                {
                    field.movingPieces.Add(gameObject);
                }
                return true;
            }
        }

        field.movingPieces.Remove(gameObject);
        return false;
    }

    /// <summary>
    /// Determine if the piece is moving or if it has been placed
    /// </summary>
    private void PlacePiece()
    {
        //If the timer is done then the peice isn't moving, else it is
        if (stillTimer <= 0)
        {
            field.movingPieces.Remove(gameObject);
        }
        else if (!field.movingPieces.Contains(gameObject))
        {
            field.movingPieces.Add(gameObject);
        }
    }

    /// <summary>
    /// Make the piece fall until it can't anymore
    /// </summary>
    private void PieceFall()
    {
        //Reduce the time between the falls
        fallTimer -= Time.deltaTime;

        //Test if the timer is completed
        if (fallTimer < 0)
        {
            //Reset the timer
            fallTimer = fallTimerReset;

            if (!MovePiece(new Vector2(0, 1)))
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
    /// Set the actual position of the peice
    /// </summary>
    private void SetPosition()
    {
        transform.localPosition = field.gridPos[gridPos];
    }

    private void TurnToPiece()
    {
        //Test if there is only one half of the pill left
        if (transform.parent.childCount == 1 &&
            !allowFall)
        {
            //Set the new image to be a piece of a pill
            GetComponent<Image>().sprite = pieceSprite;

            //Get the pill parent
            GameObject pill = transform.parent.gameObject;

            //Set the parent to the play area and delete the pill parent
            transform.SetParent(transform.parent.parent);

            Destroy(pill);

            //Allow the piece to fall
            allowFall = true;
        }
    }
}
