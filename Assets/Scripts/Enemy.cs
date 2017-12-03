using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    //The frames of the virus
    public List<Sprite> frames = new List<Sprite>();

    //The time between frames and the timer reset
    public float frameTimer;
    private float timerReset;

    //The next frame that will be shown, and the current frame
    private int nextFrame;
    private int frame;

    public FieldGrid field;

	// Use this for initialization
	void Start () {
        //Set the current and next frame
        frame = 0;
        nextFrame = 1;

        //Set the reset timer to the current timer
        timerReset = frameTimer;
	}
	
	// Update is called once per frame
	void Update () {
        Animate();

        if (field.gameOver)
        {
            Destroy(gameObject);
        }
	}
    
    /// <summary>
    /// Animate the cirus to make it look better
    /// </summary>
    private void Animate()
    {
        //If there is only 1 frame in the animation return
        if (frames.Count <= 1) { return; }

        //Reduce the amount of time til the next animation
        frameTimer -= Time.deltaTime;

        //Change the animation and reset the timer, once the timer is done
        if (frameTimer < 0)
        {
            frame += nextFrame;
            frameTimer = timerReset;
        }

        //If the animation would go out of index, go the other direction
        if (frame < 0 || frame >= frames.Count) { nextFrame *= -1; frame += nextFrame; }

        //Set the new animation frame of the virus
        gameObject.GetComponent<Image>().sprite = frames[frame];
    }
}
