using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Modos : MonoBehaviour
{
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] private Quiz_Attributes[] attributes;
    [SerializeField] private GameObject question_actualy;
    [SerializeField] private int timer_next_max = 0;
    [SerializeField] private Timer timer_next;
    [SerializeField] private int question_actualy_index;
    [SerializeField] private string path;

    [SerializeField] private int points = 0;

    public static Modos get = null;
    
    public void Awake()
    {
        if(Modos.get == null)
        {
            Modos.get = this;
            if (config == null) { config = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }
            timer_next.start = 0;

            question_actualy = GameObject.Instantiate(config.Get_Layout(attributes[0].question_type), transform);
            question_actualy.GetComponent<Quiz>().attributes = attributes[0];
            question_actualy_index = 0;
        }
        else
        {
            Config_PucQuiz config = Modos.get.config;
            Quiz_Attributes[] attributes = Modos.get.attributes;
            GameObject question_actualy = Modos.get.question_actualy;
            timer_next_max = 0;
            Timer timer_next = Modos.get.timer_next;
            question_actualy_index = Modos.get.question_actualy_index;
            path = Modos.get.path;

            question_actualy = Instantiate(config.Get_Layout(attributes[question_actualy_index].question_type), transform);
            Quiz pergunta_new = question_actualy.GetComponent<Quiz>();
            pergunta_new.attributes = attributes[question_actualy_index];

            timer_next.start = 0;
        }

    }
    public void Start()
    {
        //Translator.Get().Save_Attributes(attributes, path);
        Event_PucQuiz.start_layout = true;
    }
    public void Update()
    {
        Modos.get = this;

        if (Input.GetKeyDown(KeyCode.W))
        {
            
        }

        if (question_actualy != null)
        { 
            Perguntas pergunta = question_actualy.GetComponent<Perguntas>();

            Event_PucQuiz.points = points;
            pergunta.Update_Layout(transform.gameObject);
        }

        if (Event_PucQuiz.question_next)
        {
            if(timer_next.start == 0)
            { 
                timer_next.start = timer_next_max; 
                timer_next.Reset();

                if (Event_PucQuiz.question_result == "win")
                {
                    points += question_actualy.GetComponent<Perguntas>().points;
                    Event_PucQuiz.points = points;
                }
            }
            timer_next.Run();
            if (timer_next.End()) { timer_next = null; Change_Question(); }
        }
    }

    private void Change_Question()
    {
        Event_PucQuiz.start_layout = true;
        Event_PucQuiz.question_next = false;

        Perguntas pergunta_old = question_actualy.GetComponent<Perguntas>();

        Event_PucQuiz.question_result = "";

        Destroy(question_actualy);

        /* ---- Lembrete ---- *\
         * A partir deste ponto
         * a pergunta e o layoult
         * antigos não poderam
         * ser acessados até que
         * o programa crie uma
         * nova instancia.
        \*                    */

        if (question_actualy_index != attributes.Length)
        {
            //TODO -> FIX - An object reference is required for the non-static field, method, or property 'Modos.question_actualy_index'

            //Modos.question_actualy_index++;
        }
        Debug.Log("Question = "+question_actualy_index);
        Modos.get = this;
        Event_PucQuiz.Change_Scene(config.Layout_Contagem);
    }

    public bool Final()
    {
        if(question_actualy_index == attributes.Length) { return true; }
        return false;
    }
}
