using UnityEngine;
using UnityEngine.UIElements;

public class DEV : MonoBehaviour
{

    public GameObject consoleCanvas;


    public bool isDebug = true;
    public bool isTimerInfinity = true;
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
        DeveloperConsole.Console.AddCommand("loadAnim", LoadAnimCommand);
        DeveloperConsole.Console.AddCommand("changeToQuiz", ChangeToQuizCommand);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeToQuizCommand(string[] args)
    {
        var layoutManager = FindAnyObjectByType<LayoutManager>();
        if (layoutManager == null) return;

        //layoutManager.ChangeToQuiz();
        //TestAnimCommand(null);
    }
    public void LoadAnimCommand(string[] args)
    {
        var document = FindAnyObjectByType<UIDocument>();
        var textContainer = document.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = document.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = document.rootVisualElement.Q<VisualElement>("Container-Resposta1");
        textContainer.AddToClassList("QuestionTextStart");
        answersContainer.AddToClassList("ScaleUpStart");
        timerContainer.AddToClassList("TimerStart");
    }
    public void TestAnimCommand(string[] args)
    {
       
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
