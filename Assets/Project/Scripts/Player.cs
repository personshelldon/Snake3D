using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
 
// скрипту игрока необходим на объекте компонент CharacterController
// с помощью этого компонента будет выполняться движение
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speed = 6;
    public float rotationSpeed = 60;
    public float segmentDistance = 2;
    public GameObject segmentPrefab;
    public int startSegmentCount = 3;
    public int bonusSegmentCount = 2;
    public float increaseSpeed = 0.5f;
    public float increaseRotation = 0.5f;
    public AudioClip deadSound;
    public AudioClip bitSound;

    private CharacterController controller;
    private bool collideFlag = false;
    private Transform lastSegment;
    private BonusFood bonusFood;

    void Awake()
    {
        bonusFood = GameObject.FindObjectOfType<BonusFood>();
    }

    public void Start()
    {
        //bonusFood = GameObject.FindObjectOfType<BonusFood>();
        // получаем компонент CharacterController и 
        // записываем его в локальную переменную
        controller = GetComponent<CharacterController>();
        // создаем хвост
        // current - текущая цель элемента хвоста, начинаем с головы
        // создаем примитив куб и добавляем ему компонент Tail
        lastSegment = transform;
        for (int i = 0; i < startSegmentCount; i++)
        {
            GameObject temp = Instantiate(segmentPrefab) as GameObject;
            Tail tail  = temp.AddComponent<Tail>();         
            // помещаем "хвост" за "хозяина"
            tail.transform.position = lastSegment.transform.position - transform.forward * segmentDistance;
            // ориентация хвоста как ориентация хозяина            
            //tail.transform.rotation = current.transform.rotation;
            // элемент хвоста должен следовать за хозяином, поэтому передаем ему ссылку на его
            tail.target = lastSegment.transform;
            // дистанция между элементами хвоста - 2 единицы
            tail.targetDistance = segmentDistance;
            lastSegment = tail.transform;
            tail.name = "Tail" + Game.segmentCount;
            Game.segmentCount++;
            if (i == 0) Destroy(tail.collider);
        }
    }

    private void addSegment() 
    {
        speed += increaseSpeed;
        rotationSpeed += increaseRotation;
        GameObject temp = Instantiate(segmentPrefab) as GameObject;
        Tail tail = temp.AddComponent<Tail>();
        // помещаем "хвост" за "хозяина"
        tail.transform.position = lastSegment.transform.position;
        // ориентация хвоста как ориентация хозяина            
        //tail.transform.rotation = current.transform.rotation;
        // элемент хвоста должен следовать за хозяином, поэтому передаем ему ссылку на его
        tail.target = lastSegment.transform;
        // дистанция между элементами хвоста - 2 единицы
        tail.targetDistance = segmentDistance;
        // следующим хозяином будет новосозданный элемент хвоста
        //tail.transform.localScale = current.transform.localScale;
        lastSegment = tail.transform;
        tail.name = "Tail" + Game.segmentCount;
        Game.segmentCount++;
    }

    public void Update()
    {
        // по аналогии с блитцем - управление 
        // кодами кнопок, но такой вариант не очень гибкий
        float horizontal = 0;
        //foreach (Touch touch in Input.touches) {
           // if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
           // {
           //     pressed=ButtonsPressed.Null;    
           // }
       // }        
        if (Input.GetKey(KeyCode.RightArrow) || Game.pressedButton == Game.ButtonsStates.Right || Input.GetKey(KeyCode.D)) horizontal = 1;
        if (Input.GetKey(KeyCode.LeftArrow) || Game.pressedButton == Game.ButtonsStates.Left || Input.GetKey(KeyCode.A)) horizontal = -1;
        // вращаем трансформ вокруг оси Y 
        transform.Rotate(0, rotationSpeed * Time.deltaTime * horizontal, 0);
        // двигаем змею постоянно
        collideFlag = true;
        controller.Move(transform.forward * speed * Time.deltaTime);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (collideFlag)
        {
            Food food = (Food) hit.gameObject.GetComponent(typeof(Food));
            if (food != null)
            {
                if (((Game.segmentCount - startSegmentCount) % bonusSegmentCount == 0) && (Game.segmentCount - startSegmentCount - bonusSegmentCount >= 0))
                    bonusFood.GenerateNewFood();
                food.Eat();
                if (food.GetType()!=typeof(BonusFood))
                    addSegment();
            }
            else
            {
                if (!hit.gameObject.name.Equals("Terrain") 
                    && !hit.gameObject.name.Equals("Tail0")
                    && !hit.gameObject.name.Equals("Tail1")
                    && !hit.gameObject.name.Equals("Collider"))
                {
                    Game.lives--;
                    if (Game.lives == 0)
                    {
                        GameObject.Find("GameManager").GetComponent<Game>().showGameOverScreen();
                    }
                    else
                    {
                        if (hit.gameObject.name.Contains("Tail"))
                            MusicManager.instance.PlayOneShot(bitSound);
                        else
                            MusicManager.instance.PlayOneShot(deadSound);
                        Application.LoadLevel(Application.loadedLevelName); 
                    }
                }                    
            }
            collideFlag = false;
        }
    }
}
