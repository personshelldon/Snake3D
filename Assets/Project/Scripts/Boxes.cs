using UnityEngine;
using System.Collections;

public class Boxes : MonoBehaviour {
    public int boxesCount;
    public float rightBound = 1.0f;
    public float leftBound = 1.0f;
    public float topBound = 1.0f;
    public float bottomBound = 1.0f;
    public GameObject boxPrefab;
    public Player player;
    public float minDistanceToPlayer;

	// Use this for initialization
	void Start () {
        StartCoroutine(createNewBoxes());
	}
	
	// Update is called once per frame
	void Update () {	    
	}

    private IEnumerator createNewBoxes()
    {
        for (int i = 0; i < boxesCount; i++)
        {
            GameObject box = Instantiate(boxPrefab) as GameObject;
            while (true)
            {
                Vector3 v;
                while (true)
                {
                    v = new Vector3(Random.Range(leftBound, rightBound), 1.5f, Random.Range(topBound, bottomBound));         
                    if (Vector3.Distance(v,player.transform.position)>=minDistanceToPlayer) break;
                }
                box.transform.position = v;
                Bounds foodBounds = box.collider.bounds;
                bool intersects = false;
                foreach (Collider objectColiider in FindObjectsOfType(typeof(Collider)))
                {
                    if (objectColiider != box.collider)
                    {
                        if (objectColiider.bounds.Intersects(foodBounds))
                        {
                            intersects = true;
                            break;
                        }
                    }
                }
                if (!intersects)
                {
                    break;
                }
                yield return 0;
            }
        }
    }
}
