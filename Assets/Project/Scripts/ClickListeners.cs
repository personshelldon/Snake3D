using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClickListeners : MonoBehaviour {
    public AudioClip backgroundSound;
    public AudioClip buttonClickSound;
    public float animationSpeed = 1.0f;

	private bool aboutClicked = false;
	private Image background;
    private byte cameraMode;
    private Text cameraButtonText;
    private InputField playerNameText;
    private GameObject[] mainMenuItems;
    private GameObject[] playMenuItems;
    private bool playMenuActive = false;

	void Start() {
        mainMenuItems = GameObject.FindGameObjectsWithTag("FirstStepMenu");
        playMenuItems = GameObject.FindGameObjectsWithTag("SecondStepMenu");
        foreach (GameObject menuItem in playMenuItems)
        {
            menuItem.SetActive(false);
        }
        background = GameObject.Find("Background").GetComponent<Image>();
        MusicManager.instance.PlayBackground(backgroundSound);
        cameraMode = (byte)PlayerPrefs.GetInt("CameraMode",1);
        cameraButtonText = GameObject.Find("CameraButton").GetComponentInChildren<Text>();
        playerNameText = GameObject.Find("InputField").GetComponent<InputField>();
        playerNameText.text = PlayerPrefs.GetString("PlayerName", "Player");
        if (cameraMode == 1) cameraButtonText.text = "Camera: 2D";
        else cameraButtonText.text = "Camera: 3D";
	}

	void Update() {
		if (Input.GetKey (KeyCode.Escape)) {
			if (aboutClicked) {
                MusicManager.instance.PlayOneShot(buttonClickSound);
				background.animation.PlayQueued ("AboutClose");
				aboutClicked = false;
			}
            if (playMenuActive)
            {
                MusicManager.instance.PlayOneShot(buttonClickSound);
                playMenuActive = false;
                foreach (GameObject menuItem in playMenuItems)
                {
                    menuItem.SetActive(false);
                }
                foreach (GameObject menuItem in mainMenuItems)
                {
                    menuItem.SetActive(true);
                }
                
            }
		}
	}

	public void playButtonClick() {
        playMenuActive = true;
        MusicManager.instance.PlayOneShot(buttonClickSound);
        foreach (GameObject menuItem in mainMenuItems)
        {
            menuItem.SetActive(false);
        }
        foreach (GameObject menuItem in playMenuItems)
        {
            menuItem.SetActive(true);
        }
	}

	public void aboutButtonClick() {
        MusicManager.instance.PlayOneShot(buttonClickSound);
		background.animation.PlayQueued("AboutOpen");        
		aboutClicked = true;
	}

    public void simpleGameButtonClick()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        MusicManager.instance.doNotPlayWhileLoading();
        LoadLevelManager.instance.openLevel("Game");
    }

    public void advancedGameButtonClick()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        MusicManager.instance.doNotPlayWhileLoading();
        LoadLevelManager.instance.openLevel("Game_2");
    }

    public void playerNameChanged()
    {
        if (!playerNameText.text.Trim().Equals(""))
        {
            PlayerPrefs.SetString("PlayerName", playerNameText.text.Trim());
            PlayerPrefs.Save();
        }
        playerNameText.text = PlayerPrefs.GetString("PlayerName","Player");
    }

    public void scoresButtonListener()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        MusicManager.instance.playWhileLoading();
        LoadLevelManager.instance.openLevel("Scores");
    }

    public void cameraButtonClick()
    {
        MusicManager.instance.PlayOneShot(buttonClickSound);
        if (cameraMode ==1)
        {
            cameraMode = 2;
            PlayerPrefs.SetInt("CameraMode", 2);
            PlayerPrefs.Save();
            cameraButtonText.text = "Camera: 3D";
        }
        else
        {
            cameraMode = 1;
            PlayerPrefs.SetInt("CameraMode", 1);
            PlayerPrefs.Save();
            cameraButtonText.text = "Camera: 2D";
        }
    }
	
	public void exitButtonClick() {
        MusicManager.instance.PlayOneShot(buttonClickSound);
		Application.Quit();
	}
}
