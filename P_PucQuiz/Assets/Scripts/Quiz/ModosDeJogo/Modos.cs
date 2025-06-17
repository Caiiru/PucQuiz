using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Modos
{
    [Header("Basic Variables")]
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] public UIDocument doc;
    [SerializeField] public DictionaryThree<String, GameObject, VisualTreeAsset>[] menu;
    [SerializeField] public LayoutManager manager;
    [SerializeField] public Transform transform;

    [Header("Quiz Variables")]
    [SerializeField] public Quiz_Attributes[] attributes;
    [SerializeField] public Dictionary<String, Perguntas> question_manager = new Dictionary<string, Perguntas>();
    [SerializeField] public int question_actualy_index;
    [SerializeField] private Timer timer_awake;
    [SerializeField] private Timer timer_next;


    public Modos() { }

    public void Awake(GameObject obj)
    {

        //Variaveis De "Sistema"
        if (!question_manager.ContainsKey("Quiz")) { question_manager.Clear(); question_manager.Add("Quiz", new Quiz()); }
        Quiz quiz_manager = question_manager["Quiz"] as Quiz;

        doc = obj.GetComponent<UIDocument>();
        manager = obj.GetComponent<LayoutManager>();
        config = Resources.Load<Config_PucQuiz>("Config/PucQuiz");

        //Debug.Log("Variables Awake = Sistem Complet");

        //Variaveis do Quiz
       //attributes = attributes;
        question_actualy_index = 0;

        //Debug.Log("Variables Awake = Quiz Complet");

        timer_next.Reset();

        //Debug.Log("Variables Awake = Reset Complet");
        if (GameManager.Instance.IsServer)
            ChangeMenu("HostQuiz");
        else
            ChangeMenu(attributes[question_actualy_index].question_type.ToString());

        //Debug.Log("Variables Awake = ChangeMenu Complet");
    }
    public void Start(GameObject obj)
    {
        Event_PucQuiz.start_layout = true;
    }
    public void Update(GameObject obj)
    {
        if (question_manager == null) { Debug.Log("Manager Null"); }

        if (question_manager != null && Event_PucQuiz.layout_actualy == "Quiz")
        {
            if (obj != null)
            {
                doc.rootVisualElement.Q<TextElement>("Timer").text = "Points : " + ((int)Event_PucQuiz.points + " | " +
                                                                     "Tempo : " + ((int)attributes[question_actualy_index].timer.time));
                if (timer_awake.End() == false) { timer_awake.Run(); return; }
                question_manager["Quiz"].Update_Layout(obj);//TIMER AQUI
            }
            else
            {
                Debug.Log("Sem obj");
            }
        }
        else if (Event_PucQuiz.layout_actualy == "HostQuiz")
        {
            if (obj != null)
            {
                doc.rootVisualElement.Q<TextElement>("Timer").text = "Tempo : " + ((int)attributes[question_actualy_index].timer.time);
                if (timer_awake.End() == false) { timer_awake.Run(); return; }
                question_manager["Quiz"].Update_Layout(obj);//TIMER AQUI
            }
            else
            {
                Debug.Log("Sem obj");
            }
        }


        if (Event_PucQuiz.question_next)
        {
            if (Event_PucQuiz.question_result == "win")
            {
                //Mudar Streak.
                //Travar Time.
                //Calcular Pontos?
                //Config_PucQuiz.Get_Points(true,1,5);
            }
            timer_next.Run();
            if (timer_next.End()) { Change_Question(); }
        }
    }

    private void Change_Question()//Muda a pergunta.
    {
        GameManager gameManager = GameManager.Instance;

        if (!Final())
        {
            if (GameManager.Instance.IsServer) { gameManager.ChangeQuestionRpc(); gameManager.ChangeMenuRpc("End","Rank"); }

        }
        else
        {
            if (GameManager.Instance.IsServer) { gameManager.ChangeQuestionRpc(); gameManager.ChangeMenuRpc("End","End"); }
        }
    }

    public bool Final()//Verifica se chegamos no fim das perguntas.
    {
        if (question_actualy_index == attributes.Length-1) { return true; }
        return false;
    }
    public void FeedBack()
    {
        Debug.Log("Start Feedback.");

        if (GameManager.Instance.IsServer)
        {
            GameManager.Instance.ChangeCurrentGameStateRPC(GameState.ShowingResults, 5f);
            ChangeMenu("HostWaiting");
            return;
        }
        switch (Event_PucQuiz.question_result)
        {
            case "win":
                ChangeMenu("Correct");
                break;
            case "lose":
                ChangeMenu("Incorrect");
                break;
            case "":
                Debug.Log("No Result");
                ChangeMenu("Incorrect");
                break;
        }

        Debug.Log("End Feedback.");
    }
    public void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("Nao foi atribuido um valor ao novo menu buscado."); return; }

        Event_PucQuiz.scene_actualy = "Quiz";
        Event_PucQuiz.layout_actualy = menu_new;

        //if (!GameManager.Instance.IsServer) { ChangeMenu("HostQuiz"); ; return; }

        GameObject background = null;

        try
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].getValue1() == menu_new)
                {
                    //Debug.Log("Visual Three = " + menu_new);
                    background = menu[i].getValue2();
                    doc.visualTreeAsset = menu[i].getValue3();
                }

            }

            if (background.activeSelf == false && background != null) { background.SetActive(true); }

            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].getValue1() != menu_new && menu[i].getValue2() != background)
                {
                    menu[i].getValue2().SetActive(false);
                }
            }
        }
        catch (Exception error)
        {
            Debug.Log(error);
        }

        SetQuestion();
    }
    public void SetQuestion()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            if (menu[i].getValue1() == Event_PucQuiz.layout_actualy)
            {
                switch (menu[i].getValue1())
                {
                    case "Quiz":
                        if (question_actualy_index+1>attributes.Length/2)
                        {
                            if (!manager.sound_manager.InSound("Game2"))
                            {
                                manager.sound_manager.Play("Game Music", "Game2");
                            }
                        }
                        else
                        {
                            if (!manager.sound_manager.InSound("Game1"))
                            {
                                manager.sound_manager.Play("Game Music", "Game1");
                            }
                        }
                        if (!question_manager.ContainsKey("Quiz")) { question_manager.Clear(); question_manager.Add("Quiz", new Quiz()); }
                        SetQ(false);
                        break;
                    case "HostQuiz":
                        if (question_actualy_index+1 > attributes.Length/2)
                        {
                            if (!manager.sound_manager.InSound("Game2"))
                            {
                                manager.sound_manager.Play("Game Music", "Game2");
                            }
                        }
                        else
                        {
                            if (!manager.sound_manager.InSound("Game1"))
                            {
                                manager.sound_manager.Play("Game Music", "Game1");
                            }
                        }
                        if (!question_manager.ContainsKey("Quiz")) { question_manager.Clear(); question_manager.Add("Quiz", new Quiz()); }
                        SetQ(true);
                        break;
                    case "Correct":
                        if(Event_PucQuiz.streak <= 4 && Event_PucQuiz.streak >= 1)
                        {
                            manager.sound_manager.Play("Feedback - Correct", "Correct"+Event_PucQuiz.streak);
                        }
                        else if(Event_PucQuiz.streak > 4) { manager.sound_manager.Play("Feedback - Correct", "Correct4"); }
                        else { manager.sound_manager.Play("Feedback - Correct", "Correct1"); }
                        doc.rootVisualElement.Q<TextElement>("Points").text = "+" + Event_PucQuiz.points;
                        break;
                    case "Incorrect":
                        manager.sound_manager.Play("Feedback - Incorrect", "Error");
                        doc.rootVisualElement.Q<TextElement>("Points").text = "+" + Event_PucQuiz.points;
                        break;
                }
            }
        }


    }

    public void SetQ(bool isServer)
    {
        question_manager.Clear();
        question_manager.Add("Quiz",new Quiz());
        Quiz quiz = question_manager["Quiz"] as Quiz;

        quiz.attributes = attributes[question_actualy_index];
        quiz.mod = this;

        quiz.attributes.timer.Reset();
        if (isServer)
            doc.rootVisualElement.Q<TextElement>("Timer").text = "Tempo : " + ((int)attributes[question_actualy_index].timer.time);
        else
        {
            doc.rootVisualElement.Q<TextElement>("Timer").text = "Points : " + ((int)Event_PucQuiz.points + " | " +
                                             "Tempo : " + ((int)attributes[question_actualy_index].timer.time));
        }

        TextElement pergunta = doc.rootVisualElement.Q<TextElement>("Pergunta");
        pergunta.text = attributes[question_actualy_index].question;
        //21 - 96 - 100%
        //x - y - v%
        //x = (21 * y) / 96

        var newFontSize = ((21 * 96) / pergunta.text.Length);
        newFontSize = Mathf.Clamp(newFontSize, 36, 96);

        pergunta.style.fontSize = newFontSize;
        Debug.Log($"new font size: {newFontSize}");

        //pergunta.styleSheets.Remove(pergunta.styleSheets[1]);


        Button resposta_1 = doc.rootVisualElement.Q<Button>("Resposta_1");
        resposta_1.text = attributes[question_actualy_index].options[0];
        resposta_1.RegisterCallback<ClickEvent>(quiz.ClickPergunta1);

        newFontSize = ((6 * 76) / resposta_1.text.Length);
        newFontSize = Mathf.Clamp(newFontSize, 36, 76);


        Button resposta_2 = doc.rootVisualElement.Q<Button>("Resposta_2");
        resposta_2.text = attributes[question_actualy_index].options[1];
        resposta_2.RegisterCallback<ClickEvent>(quiz.ClickPergunta2);

        newFontSize = ((6 * 76) / resposta_2.text.Length);
        newFontSize = Mathf.Clamp(newFontSize, 36, 76);


        Button resposta_3 = doc.rootVisualElement.Q<Button>("Resposta_3");
        resposta_3.text = attributes[question_actualy_index].options[2];
        resposta_3.RegisterCallback<ClickEvent>(quiz.ClickPergunta3);

        newFontSize = ((6 * 76) / resposta_3.text.Length);
        newFontSize = Mathf.Clamp(newFontSize, 36, 76);

        Button resposta_4 = doc.rootVisualElement.Q<Button>("Resposta_4");
        resposta_4.text = attributes[question_actualy_index].options[3];
        resposta_4.RegisterCallback<ClickEvent>(quiz.ClickPergunta4);

        newFontSize = ((6 * 76) / resposta_4.text.Length);
        newFontSize = Mathf.Clamp(newFontSize, 36, 76);

        timer_awake.Reset();
        timer_next.Reset();

        manager.StartCoroutine(SetAnim(0.5f));
    }

    IEnumerator SetAnim(float time)
    {
        yield return new WaitForSeconds(time);

        VisualElement questions = doc.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        VisualElement timer = doc.rootVisualElement.Q<VisualElement>("Container_Timer");
        VisualElement buttons = doc.rootVisualElement.Q<VisualElement>("GridContainer");
        questions.RemoveFromClassList("QuestionText_Anim");
        timer.RemoveFromClassList("TimerText_Anim");
        buttons.RemoveFromClassList("Buttons_Anim");
    }
}
