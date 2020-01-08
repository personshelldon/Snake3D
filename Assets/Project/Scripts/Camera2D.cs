using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {
    public CharacterController player;
    public GameObject arrow;
    public float positionX = 0.5f;
    public float positionY = 0.95f;
    public float cameraDistance = 5;

    private GameObject apple;

	// Use this for initialization
	void Start () {
        apple = GameObject.Find("apple");
	}
	
	// Update is called once per frame
	void Update () {
        arrow.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(positionX, positionY, camera.transform.position.y - 10));
        arrow.transform.LookAt(apple.transform);
        Vector3 v = player.transform.position + player.transform.forward * cameraDistance;
        v.y = transform.position.y;
        transform.position = v;
	}
}
