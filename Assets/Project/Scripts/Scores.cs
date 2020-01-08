using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scores : MonoBehaviour {
    public Text prefab;
    private bool exit = false;

	void Awake()
    {
        SqliteDatabase db = new SqliteDatabase("scores.db");
        try
        {
            string query = "SELECT * FROM scores ORDER BY points DESC";
            DataTable data = db.ExecuteQuery(query);
            GameObject parent = GameObject.Find("Panel");
            foreach (DataRow row in data.Rows)
            {
                Text t1 = Instantiate(prefab) as Text;
                Text t2 = Instantiate(prefab) as Text;
                Text t3 = Instantiate(prefab) as Text;
                t1.color = new Color(0.29f,0.48f,0.83f);
                t2.color = new Color(0.29f, 0.48f, 0.83f);
                t3.color = new Color(0.29f, 0.48f, 0.83f);
                t1.transform.SetParent(parent.transform, false);
                t2.transform.SetParent(parent.transform, false);
                t3.transform.SetParent(parent.transform, false);                
                t1.text = row["username"] as string;
                t2.text = "" + row["points"];
                t3.text = "" + row["length"];
            }
        }
        catch {}     
    }

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = GameObject.Find("ScrollRect").GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar.value *= 2;
        Canvas.ForceUpdateCanvases();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)&&!exit)
        {
            exit = true;
            LoadLevelManager.instance.openLevel("Menu");
        }
    }

    public void backButtonListener()
    {
        exit = true;
        LoadLevelManager.instance.openLevel("Menu");
    }
}
