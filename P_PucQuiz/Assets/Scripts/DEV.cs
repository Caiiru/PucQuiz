using UnityEngine;
using UnityEngine.UIElements;

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

        DeveloperConsole.Console.AddCommand("testAnim", TestAnimCommand);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TestAnimCommand(string[] args)
    {
        if (GameManager.Instance.CurrentGameState.Value != GameState.DisplayingQuestion) return;
        var document = FindAnyObjectByType<UIDocument>();
        var textContainer = document.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = document.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = document.rootVisualElement.Q<VisualElement>("Container-Resposta1");
        textContainer.RemoveFromClassList("QuestionTextStart");
        answersContainer.RemoveFromClassList("ScaleUpStart"); 
        timerContainer.RemoveFromClassList("TimerStart"); 
    }

    public void DevPrint(string text)
    {
        if (!isDebug) return;
        Debug.Log(text);
        DeveloperConsole.Console.Print(text);
    }
}
