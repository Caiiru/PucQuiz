using UnityEngine;

public class DEV : MonoBehaviour
{

    public GameObject consoleCanvas;


    public bool isDebug = true;
    public static DEV Instance;

    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }
    void Start()
    {
        consoleCanvas.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DevPrint(string text)
    {
        Debug.Log(text);
        DeveloperConsole.Console.Print(text);
    }
}
