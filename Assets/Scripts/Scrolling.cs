using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour {

    public Vector2 direction;
    public float speed;

    private List<GameObject> tiles = new List<GameObject>();

    private Vector2 screenSize;

    // Use this for initialization
    void Start () {
        direction = direction.normalized;

        screenSize = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height);
        Vector2 textureSize = new Vector2(transform.GetChild(0).GetComponent<RectTransform>().rect.width, transform.GetChild(0).GetComponent<RectTransform>().rect.height);

        for (int x=-1; x<=1; x++)
        {
            for (int y=-1; y<=1; y++)
            {
                GameObject tile = Instantiate(transform.GetChild(0).gameObject);
                tile.transform.SetParent(transform);
                tile.transform.localPosition = new Vector3(x * textureSize.x, y * textureSize.y);

                tiles.Add(tile);
            }
        }

        Destroy(transform.GetChild(0).gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        foreach(Transform child in transform)
        {
            child.localPosition += (Vector3)direction * speed;

            child.localPosition = WrapScreen(child);
        }
	}

    private Vector2 WrapScreen(Transform obj)
    {
        Vector2 localPos = obj.localPosition;

        Rect textureSize = obj.GetComponent<RectTransform>().rect;

        if (localPos.x - textureSize.width * .5f > screenSize.x * .5f)
        {
            localPos.x += (Mathf.Sqrt(tiles.Count) * 2) * textureSize.x;
        }
        else if (localPos.x + textureSize.width * .5f < -screenSize.x * .5f)
        {
            localPos.x -= (Mathf.Sqrt(tiles.Count) * 2) * textureSize.x;
        }

        if (localPos.y - textureSize.height * .5f > screenSize.y * .5f)
        {
            localPos.y += (Mathf.Sqrt(tiles.Count) * 2) * textureSize.y;
        }
        else if (localPos.y + textureSize.height * .5f < -screenSize.y * .5f)
        {
            localPos.y -= (Mathf.Sqrt(tiles.Count) * 2) * textureSize.y;
        }

        return localPos;
    }
}
