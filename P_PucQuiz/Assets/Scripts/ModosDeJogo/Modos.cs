using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Modos
{
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] public Quiz_Attributes[] attributes;
    [SerializeField] public int question_actualy_index;
    [SerializeField] private Transform transform; public void Set_Transform(Transform transform) { this.transform = transform; }


    [SerializeField] private GameObject question_actualy;
    [SerializeField] private Timer timer_next;
    [SerializeField] private string path;

    
    [SerializeField] private int points = 0;

    public static Modos get = null;

    public Modos() { }
    
    public void Awake()
    {
        timer_next.Reset();

        if (Modos.get == null)
        {
            Modos.get = new Modos();

            if (config == null) { config = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }

            //question_actualy = GameObject.Instantiate(config.Get_Layout(attributes[0].question_type), transform);
            //question_actualy.GetComponent<Quiz>().attributes = attributes[0];
            question_actualy_index = 0;

            Modos.get.config = config;
            Modos.get.attributes = attributes;
            Modos.get.question_actualy_index = question_actualy_index;
            Modos.get.transform = transform;
            Modos.get.timer_next = timer_next;

            Debug.Log("Modos get = false");
        }

        question_actualy = GameObject.Instantiate(config.Get_Layout(attributes[question_actualy_index].question_type), transform);
        question_actualy.GetComponent<Quiz>().attributes = attributes[question_actualy_index];

    }
    public void Start()
    {
        //Translator.Get().Save_Attributes(attributes, path);
        Event_PucQuiz.start_layout = true;
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            
        }

        if (question_actualy != null)
        { 
            Perguntas pergunta = question_actualy.GetComponent<Perguntas>();
            pergunta.Update_Layout(transform.gameObject);
            points = (int)Event_PucQuiz.points;
        }

        if (Event_PucQuiz.question_next)
        {
            if (Event_PucQuiz.question_result == "win")
            {
                points = question_actualy.GetComponent<Perguntas>().points;
            }
            timer_next.Run();
            if (timer_next.End()) { Change_Question(); }
        }
    }

    private void Change_Question()
    {
        Event_PucQuiz.start_layout = true;
        Event_PucQuiz.question_next = false;
        //Event_PucQuiz.points = points;

        Perguntas pergunta_old = question_actualy.GetComponent<Perguntas>();

        Event_PucQuiz.question_result = "";

        GameObject.Destroy(question_actualy);

        /* ---- Lembrete ---- *\
         * A partir deste ponto
         * a pergunta e o layoult
         * antigos n�o poderam
         * ser acessados at� que
         * o programa crie uma
         * nova instancia.
        \*                    */

        Debug.Log("Question = "+question_actualy_index);
        Event_PucQuiz.Change_Scene(config.Layout_Contagem);
    }

    public bool Final()
    {
        if(question_actualy_index == attributes.Length) { return true; }
        return false;
    }
}
