using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Game : MonoBehaviour
{
    public static bool firstStart = false;
    public static int points;
    public static int segmentCount;
    public static int maxLength;
    public static int lives;
    public static ButtonsStates pressedButton;    

    private int _lastPoints = -1;
    private int _lastSegmentCount = -1;
    private Text pointsLabel;
    private Text lengthLabel;
    private GameObject pauseCanvas;
    private GameObject gameOverCanvas;
    private byte cameraMode;
    private byte newCameraMode;
    private Vector3 defaultCameraPosition;
    private Quaternion defaultCameraRotation;
    private bool paused = false;
    private string playerName;
    private float defaultFov;
    private GameObject arrow;

    public enum ButtonsStates { Left, Right, Null };
    public Sprite noLiveSprite;
    public AudioClip[] backgroundSound; 
    public AudioClip buttonClickSound;
    public float cameraChangeSpeed = 1.0f;
    public AudioClip gameOverSound;
    public float fov3D;

    public void Awake()
    {
        SqliteDatabase sqlDB = new SqliteDatabase("scores.db");
        string historyTrigger = "CREATE TRIGGER IF NOT EXISTS history_trigger INSERT ON scores WHEN (select count(*) from scores)>19 "
            +"BEGIN "
            + "DELETE FROM scores WHERE scores.id IN (SELECT scores.id FROM scores ORDER BY scores.id limit (select count(*) -19 from scores));"
            +"END;";
        string createTable = "CREATE TABLE IF NOT EXISTS scores (id UNSIGNED MEDIUMINT(8),username CHAR(50) NOT NULL, points int NOT NULL, length INT NOT NULL, PRIMARY KEY (id))";
        sqlDB.ExecuteNonQuery(createTable);
        sqlDB.ExecuteNonQuery(historyTrigger);
        GameObject c = GameObject.Find("Main Camera");
        arrow = GameObject.Find("Arrow");
        defaultCameraPosition = c.transform.position;
        defaultCameraRotation = c.transform.rotation;
        defaultFov = c.GetComponent<Camera>().fieldOfView;
        Time.timeScale = 1;
        playerName = PlayerPrefs.GetString("PlayerName", "Player");
        switch (PlayerPrefs.GetInt("CameraMode",1))
        {
            case 1:
                cameraMode = 1;
                GameObject.Find("CameraButton").GetComponentInChildren<Text>().text = "Camera: 2D";
                break;
            case 2:
                cameraMode = 2;                
                Transform cam = GameObject.Find("Main Camera").transform;
                Camera2D temp = cam.GetComponent<Camera2D>();
                if (temp != null)
                    temp.enabled = false;
                if (arrow != null)
                    arrow.SetActive(false);
                cam.SetParent(GameObject.Find("head").transform);
                cam.position = GameObject.Find("SecondCameraPosition").transform.position;
                cam.rotation = GameObject.Find("SecondCameraPosition").transform.rotation;
                cam.GetComponent<Camera>().fieldOfView = fov3D;
                GameObject.Find("CameraButton").GetComponentInChildren<Text>().text = "Camera: 3D";
                break;
        }
        newCameraMode = cameraMode;
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
        gameOverCanvas = GameObject.Find("GameOverCanvas");
        gameOverCanvas.SetActive(false);
        segmentCount = 0;
        pressedButton = ButtonsStates.Null;
        if (!firstStart)
        {
            points = 0;
            maxLength = 0;
            lives = 3;
            MusicManager.instance.PlayBackground(backgroundSound);
            firstStart = true;
        }
        pointsLabel = GameObject.Find("Score Text").GetComponent<Text>();
        lengthLabel = GameObject.Find("Length Text").GetComponent<Text>();
        switch (lives)
        {
            case 1:
                GameObject.Find("heart1").GetComponent<Image>().sprite = noLiveSprite;
                GameObject.Find("heart2").GetComponent<Image>().sprite = noLiveSprite;
                break;
            case 2:
                GameObject.Find("heart1").GetComponent<Image>().sprite = noLiveSprite;
                break;
        }
    }

    public void Update()
    {
        if (_lastPoints != points)
        {
            _lastPoints = points;
            pointsLabel.text = "Score: " + points.ToString("0000");
        }
        if (_lastSegmentCount != segmentCount)
        {
            _lastSegmentCount = segmentCount;
            lengthLabel.text = "Length: " + segmentCount;
        }
        if (segmentCount > maxLength) maxLength = segmentCount;
        if (Input.GetKey(KeyCode.Escape)&&!paused)
            pauseButtonListener();
    }

    public void leftButtonListener()
    {
        pressedButton = ButtonsStates.Left;
    }

    public void rightButtonListener()
    {
        pressedButton = ButtonsStates.Right;
    }

    public void deselectButtonListener()
    {
        pressedButton = ButtonsStates.Null;
    }

    public void restartButtonListener()
    {
        Time.timeScale = 1;
        firstStart = false;
        LoadLevelManager.instance.openLevel(Application.loadedLevelName);
    }

    public void exitButtonListener()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        Application.Quit();
    }

    public void goToMenuButtonListener()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        firstStart = false;        
        LoadLevelManager.instance.openLevel("Menu");
    }

    public void cameraButtonListener()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        if (newCameraMode==1)
        {    
            newCameraMode = 2;
            GameObject.Find("CameraButton").GetComponentInChildren<Text>().text = "Camera: 3D";
        }
        else
        {
            newCameraMode = 1;
            GameObject.Find("CameraButton").GetComponentInChildren<Text>().text = "Camera: 2D";           
        }
        
    }

    private IEnumerator cameraChange()
    {     
        float temp = Time.deltaTime;
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        GameObject cam3D = GameObject.Find("SecondCameraPosition");        
        if (cameraMode == 1)
        {
            cam.transform.parent = null;
            GameObject player = GameObject.Find("head");
            Camera2D  c = cam.GetComponent<Camera2D>();
            if (c != null)
                c.enabled = true;
            if (arrow != null)
                arrow.SetActive(true);
            while (true)
            {
                float x, y, z;
                Vector3 v;
                if (c == null)
                {
                    x = Mathf.Lerp(cam3D.transform.position.x, defaultCameraPosition.x, temp * cameraChangeSpeed);
                    y = Mathf.Lerp(cam3D.transform.position.y, defaultCameraPosition.y, temp * cameraChangeSpeed);
                    z = Mathf.Lerp(cam3D.transform.position.z, defaultCameraPosition.z, temp * cameraChangeSpeed);
                    v = new Vector3(x,y,z);
                }
                else
                {
                    x = Mathf.Lerp(cam3D.transform.position.x, player.transform.position.x, temp * cameraChangeSpeed);
                    y = Mathf.Lerp(cam3D.transform.position.y, defaultCameraPosition.y, temp * cameraChangeSpeed);
                    z = Mathf.Lerp(cam3D.transform.position.z, player.transform.position.z, temp * cameraChangeSpeed);
                    v = new Vector3(x, y, z) + player.transform.forward*c.cameraDistance;
                }
                cam.transform.position = v;                
                Quaternion q = Quaternion.Slerp(cam3D.transform.rotation, defaultCameraRotation, temp * cameraChangeSpeed);
                cam.transform.rotation = q;
                float fov = Mathf.Lerp(fov3D, defaultFov, temp * cameraChangeSpeed);
                cam.fieldOfView = fov;
                if (temp >= 1) break;
                yield return 0;
                temp += Time.deltaTime;
            }
        }
        if (cameraMode == 2)
        {
            cam.transform.parent = GameObject.Find("head").transform;
            Camera2D c = cam.GetComponent<Camera2D>();
            if (c != null)
                c.enabled = false;
            if (arrow != null)
                arrow.SetActive(false);
            while (true)
            {
                float x = Mathf.Lerp(defaultCameraPosition.x, cam3D.transform.position.x, temp * cameraChangeSpeed);
                float y = Mathf.Lerp(defaultCameraPosition.y, cam3D.transform.position.y, temp * cameraChangeSpeed);
                float z = Mathf.Lerp(defaultCameraPosition.z, cam3D.transform.position.z, temp * cameraChangeSpeed);
                Vector3 v = new Vector3(x, y, z);
                cam.transform.position = v;
                Quaternion q = Quaternion.Slerp(defaultCameraRotation, cam3D.transform.rotation, temp * cameraChangeSpeed);
                cam.transform.rotation = q;
                float fov = Mathf.Lerp(defaultFov, fov3D, temp * cameraChangeSpeed);
                cam.fieldOfView = fov;
                if (temp >= 1) break;
                yield return 0;
                temp += Time.deltaTime;
            }
        }
    }

    public void pauseButtonListener()
    {
        paused = true;
        MusicManager.instance.PlayOneShot(buttonClickSound);
        Time.timeScale = 0;
        Button temp = GameObject.Find("PauseButton").GetComponent<Button>();
        temp.onClick.RemoveAllListeners();
        temp.GetComponent<Button>().onClick.AddListener(resumeButtonListener);
        pauseCanvas.SetActive(true);
    }

    public void resumeButtonListener()
    {
        paused = false;
        MusicManager.instance.PlayOneShot(buttonClickSound);
        if (newCameraMode!=cameraMode)
        {
            cameraMode = newCameraMode;
            PlayerPrefs.SetInt("CameraMode", cameraMode);
            PlayerPrefs.Save();
            StartCoroutine(cameraChange());
        }
        Time.timeScale = 1;
        Button temp = GameObject.Find("PauseButton").GetComponent<Button>();
        temp.GetComponent<Button>().onClick.RemoveAllListeners();
        temp.GetComponent<Button>().onClick.AddListener(pauseButtonListener);
        pauseCanvas.SetActive(false);
    }

    public void showGameOverScreen() 
    {
        MusicManager.instance.StopBackground();
        MusicManager.instance.PlayOneShot(gameOverSound);
        Time.timeScale = 0;
        gameOverCanvas.SetActive(true);
        GameObject.Find("ScoreText").GetComponent<Text>().text = "Score: " + Game.points.ToString("0000");
        GameObject.Find("MaxLengthText").GetComponent<Text>().text = "Max Length: " + Game.maxLength;
        SqliteDatabase sqlDB = new SqliteDatabase("scores.db");
        string query = "INSERT INTO scores (username,points,length) VALUES ('"+playerName+"',"+points+","+maxLength+")";
        sqlDB.ExecuteNonQuery(query);
    }
}