using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pill : MonoBehaviour {

    public List<Sprite> pillPieces = new List<Sprite>();

    private Vector2 gridPos1;
    private Vector2 gridPos2;
    public FieldGrid field;

    public bool playing = false;

    public float fallTimer;
    private float fallTimerReset;

    public float stillTimer;
    private float stillTimerReset;

    private int rotation;

    // Use this for initialization
    void Start () {
        gridPos1 = new Vector2(3, 0);
        gridPos2 = new Vector2(4, 0);

        //if (seed != 0) { Random.InitState(seed); }

        //Set a random sprite for both sides of the pill
		foreach(Transform child in transform)
        {
            child.GetComponent<Image>().sprite = pillPieces[Random.Range(0, pillPieces.Count)];
        }

        //Set the reset timer values
        fallTimerReset = fallTimer;
        stillTimerReset = stillTimer;
	}
	
	// Update is called once per frame
	void Update () {

        //If the pill is not in play don't update it
        if (!playing) { return;}

        PillFall();
        SetPosition();

        ControlPill();
        SetPosition();

        //If the pill has stayed still for to long destory this script
        if (stillTimer <= 0)
        {
            transform.SetParent(field.gameObject.transform);
            Destroy(this);
        }
	}

    private void ControlPill()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!MovePill(new Vector2(0, 1))) { stillTimer = 0; }
            fallTimer = fallTimerReset;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (MovePill(new Vector2(-1, 0))) { fallTimer += Time.deltaTime * 5; }

        } else if (Input.GetKeyDown(KeyCode.D))
        {
            if (MovePill(new Vector2(1, 0))) { fallTimer += Time.deltaTime * 5; }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (RotatePill(1)) { fallTimer += Time.deltaTime * 5; }
        }
    }

    private bool RotatePill(int direction)
    {
        int localRot = (rotation + 90 * direction) % 360;

        Vector2 newPos1 = gridPos1;
        Vector2 newPos2 = gridPos2;

        switch (localRot)
        {
            case 0:
                newPos2 += new Vector2(1, 1) * direction;
                break;

            case 90:
                newPos1 -= new Vector2(0, 1) * direction;
                newPos2 -= new Vector2(1, 0) * direction;
                break;

            case 180:
                newPos1 += new Vector2(1, 1) * direction;
                break;

            case 270:
                newPos1 -= new Vector2(1, 0) * direction;
                newPos2 -= new Vector2(0, 1) * direction;
                break;
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

                rotation += (90 * direction) % 360;
                transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

                Debug.Log(newPos1 + " " + newPos2);

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

    private bool MovePill(Vector2 dir)
    {
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

            if (!MovePill(new Vector2(0, 1))) {
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
