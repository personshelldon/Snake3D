using UnityEngine;
using System.Collections;

public class BaseFood : MonoBehaviour, Food {
    public int points = 10;
    public Bounds positionBounds;
    public float rotationSpeed = 60;
    public float animationSpeed = 1;
    public float rightBound = 1.0f;
    public float leftBound = 1.0f;
    public float topBound = 1.0f;
    public float bottomBound = 1.0f;
    public AudioClip eatSound;
    public Collider additionalCollider;

    private float defaultYPos;
    private bool pause = false;

	void Start () {
        defaultYPos = 0.86f;
        StartCoroutine(animateNewFood());
	}
	
	void Update () {
        transform.Rotate(Vector3.forward,rotationSpeed*Time.deltaTime);
	}

    public void Eat()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        Game.points += points;
        MusicManager.instance.PlayOneShot(eatSound);
        StartCoroutine(animateNewFood());
    }

    public void PauseFoodGeneration()
    {
        pause = true;
    }

    public void ResumeFoodGeneration()
    {
        pause = false;
    }

    private IEnumerator getNewPosition()
    {
        while (true)
        {
            additionalCollider.transform.position = new Vector3(Random.Range(leftBound, rightBound), 1.2f, Random.Range(topBound, bottomBound));
            Bounds foodBounds = additionalCollider.bounds;
            bool intersects = false;
            foreach (Collider objectColiider in FindObjectsOfType(typeof(Collider)))
            {
                if (objectColiider != additionalCollider)
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
                additionalCollider.transform.Translate(0,-5,0);
                break;
            }
            yield return 0;
        }
    }

    IEnumerator animateNewFood()
    {
        float temp = 0;
        while (true)
        {
            temp += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, -1.5f, animationSpeed * temp), transform.position.z);
            if (animationSpeed * temp >= 1) break;
            yield return 0;
        }
        while (true)
            if (pause)
                yield return 0;
            else
                break;
        yield return StartCoroutine(getNewPosition());
        transform.position = new Vector3(additionalCollider.transform.position.x,transform.position.y,additionalCollider.transform.position.z);
        transform.GetComponent<BoxCollider>().enabled = true;
        temp = 0;        
        while (true)
        {
            temp += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, defaultYPos, animationSpeed * temp), transform.position.z);
            if (animationSpeed * temp >= 1) break;
            yield return 0;
        }
    }
}
