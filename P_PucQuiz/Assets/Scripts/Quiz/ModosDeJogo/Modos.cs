using System;
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
    [SerializeField] private Dictionary<String, Perguntas> question_manager = new Dictionary<string, Perguntas>();
    [SerializeField] public int question_actualy_index;
    [SerializeField] private Timer timer_next;


    public Modos() { }
    
    public void Awake(GameObject obj)
    {
        Debug.Log("Start to set Awake");

        //Variaveis De "Sistema"
        question_manager.Add("Quiz",new Quiz());
        doc = obj.GetComponent<UIDocument>();
        manager = obj.GetComponent<LayoutManager>();
        config = Resources.Load<Config_PucQuiz>("Config/PucQuiz");

        //Debug.Log("Variables Awake = Sistem Complet");

        //Variaveis do Quiz
        attributes = attributes;
        question_actualy_index = 0;
        timer_next = timer_next;

        //Debug.Log("Variables Awake = Quiz Complet");

        timer_next.Reset();

        //Debug.Log("Variables Awake = Reset Complet");

        ChangeMenu(attributes[question_actualy_index].question_type.ToString());

        //Debug.Log("Variables Awake = ChangeMenu Complet");
    }
    public void Start(GameObject obj)
    {
        Event_PucQuiz.start_layout = true;
    }
    public void Update(GameObject obj)
    {
        if(question_manager == null) { Debug.Log("Manager Null"); }

        if (question_manager != null && Event_PucQuiz.layout_actualy == "Quiz")
        {
            //Debug.Log("Manager exist");
            if(obj!=null)
            {
                doc.rootVisualElement.Q<TextElement>("Timer").text = "Points : " + ((int)Event_PucQuiz.points + " | " +
                                                                     "Tempo : " + ((int)attributes[question_actualy_index].timer.time));
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
        Event_PucQuiz.start_layout = true;
        Event_PucQuiz.question_next = false;
        Event_PucQuiz.question_result = "";

        question_actualy_index++;

        if(!Final())
        {
            Debug.Log("Question = " + question_actualy_index);

            Event_PucQuiz.layout_actualy = "FeedBack";
            //Colocar no "End"/"FeedBack layout" uma verificação o resultado do jogador e alterar o menu para o feedback correto.

            
            ChangeMenu(attributes[question_actualy_index].question_type.ToString());
        }
        else
        {
            Event_PucQuiz.layout_actualy = "Start";
            manager.menu.ChangeMenu("Start");
        }
        
        //Event_PucQuiz.Change_Scene(config.Layout_Contagem);
    }

    public bool Final()//Verifica se chegamos no fim das perguntas.
    {
        if(question_actualy_index == attributes.Length) { return true; }
        return false;
    }
    public void FeedBack()
    {
        Debug.Log("Start Feedback.");

        switch(Event_PucQuiz.question_result)
        {
            case "win":
                ChangeMenu("Correct");
                break;
            case "lose":
                ChangeMenu("Incorrect");
                break;
            case "":
                Debug.Log("No Result");
                break;
        }
        /*for (int i = 0; i < attributes[question_actualy_index].choice_correct.Length; i++)
        {
            if (attributes[question_actualy_index].choice_correct[i])
            {
                doc.rootVisualElement.Q<Button>("Pergunta" + (i + 1)).style.backgroundColor = Color.green;
            }
            else
            {
                doc.rootVisualElement.Q<Button>("Pergunta" + (i + 1)).style.backgroundColor = Color.gray;
            }
        }*/

        Debug.Log("End Feedback.");
    }
    public void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("Nao foi atribuido um valor ao novo menu buscado."); return; }

        Event_PucQuiz.layout_actualy = menu_new;

        GameObject background = null;

        try
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].getValue1() == menu_new)
                {
                    background = menu[i].getValue2();
                    doc.visualTreeAsset = menu[i].getValue3();
                }

            }

            if (background.active == false && background != null) { background.SetActive(true); }

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

                        Quiz quiz = question_manager["Quiz"] as Quiz;

                        quiz.attributes = attributes[question_actualy_index];
                        quiz.mod = this;

                        doc.rootVisualElement.Q<TextElement>("Timer").text = "Points : " + ((int)Event_PucQuiz.points + " | " +
                                                             "Tempo : " + ((int)attributes[question_actualy_index].timer.time));

                        doc.rootVisualElement.Q<TextElement>("Pergunta").text = attributes[question_actualy_index].question;

                        Debug.Log("Start set quiz buttons");

                        doc.rootVisualElement.Q<Button>("Resposta_1").text = attributes[question_actualy_index].options[0];
                        doc.rootVisualElement.Q<Button>("Resposta_1").RegisterCallback<ClickEvent>(quiz.ClickPergunta1);

                        doc.rootVisualElement.Q<Button>("Resposta_2").text = attributes[question_actualy_index].options[1];
                        doc.rootVisualElement.Q<Button>("Resposta_2").RegisterCallback<ClickEvent>(quiz.ClickPergunta2);

                        doc.rootVisualElement.Q<Button>("Resposta_3").text = attributes[question_actualy_index].options[2];
                        doc.rootVisualElement.Q<Button>("Resposta_3").RegisterCallback<ClickEvent>(quiz.ClickPergunta3);

                        doc.rootVisualElement.Q<Button>("Resposta_4").text = attributes[question_actualy_index].options[3];
                        doc.rootVisualElement.Q<Button>("Resposta_4").RegisterCallback<ClickEvent>(quiz.ClickPergunta4);

                        //Debug.Log("Reset Timers");

                        timer_next.Reset();

                        //Debug.Log("End set quiz");
                        break;
                    case "Correct":
                        doc.rootVisualElement.Q<TextElement>("Points").text = "+" + Event_PucQuiz.points;
                        break;
                    case "Incorrect":
                        doc.rootVisualElement.Q<TextElement>("Points").text = "+" + Event_PucQuiz.points;
                        break;
                }
            }
            try
            {
                
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }
}
