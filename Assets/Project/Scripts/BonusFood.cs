using UnityEngine;
using System.Collections;

public class BonusFood : MonoBehaviour, Food {
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
    public float animationHeight = 1.5f;
    public float defaultYPos = 0.86f;
    public int decreaseStep = 2;
    public int bonusSeconds = 3;

    private Player player;
    private bool disposed = true;
    private GameObject current;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public void Eat()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        MusicManager.instance.PlayOneShot(eatSound);
        Game.points += points;
        player.speed -= player.increaseSpeed*decreaseStep;
        player.rotationSpeed -= player.increaseRotation * decreaseStep;
        StartCoroutine(disposeFood());
    }

    public void GenerateNewFood()
    {
        disposed = false;
        gameObject.SetActive(true);
        StartCoroutine(foodGeneration());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(bonusSeconds);
        if (!disposed)
            StartCoroutine(disposeFood());
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
                additionalCollider.transform.Translate(0, -5, 0);
                break;
            }
            yield return 0;
        }
    }

    private IEnumerator disposeFood()
    {
        disposed = true;
        transform.GetComponent<BoxCollider>().enabled = false;
        float temp = 0;
        while (true)
        {
            temp += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, -animationHeight, animationSpeed * temp), transform.position.z);
            if (animationSpeed * temp >= 1) break;
            yield return 0;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator foodGeneration()
    {       
        yield return StartCoroutine(getNewPosition());
        transform.position = new Vector3(additionalCollider.transform.position.x, transform.position.y, additionalCollider.transform.position.z);
        transform.GetComponent<BoxCollider>().enabled = true;
        float temp = 0;
        while (true)
        {
            temp += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, defaultYPos, animationSpeed * temp), transform.position.z);
            if (animationSpeed * temp >= 1) break;
            yield return 0;
        }
        StartCoroutine(Timer());
    }

}
