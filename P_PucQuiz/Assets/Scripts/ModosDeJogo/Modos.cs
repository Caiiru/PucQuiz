using System;
using UnityEditor.Build.Content;
using UnityEngine;

public class Modos : MonoBehaviour
{
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] private Quiz_Attributes[] attributes;
    [SerializeField] private GameObject question_actualy;
    [SerializeField] private int question_actualy_index;

    [SerializeField] private int points = 0;
    public void Awake()
    {
        if (config == null) { config = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }

        question_actualy = GameObject.Instantiate(config.Get_Layout(attributes[0].question_type), transform);
        question_actualy.GetComponent<Quiz>().attributes = attributes[0];

    }
    public void Start()
    {
        Event_PucQuiz.start_layout = true;
        question_actualy_index = 0;
    }
    public void Update()
    {
        if (question_actualy != null)
        { 
            Perguntas pergunta = question_actualy.GetComponent<Perguntas>();

            Event_PucQuiz.points = points;
            pergunta.Update_Layout(transform.gameObject);
        }

        if (Event_PucQuiz.question_next)
        {
            Change_Question();
        }
    }

    private void Change_Question()
    {
        Event_PucQuiz.start_layout = true;
        Event_PucQuiz.question_next = false;

        Perguntas pergunta_old = question_actualy.GetComponent<Perguntas>();

        if (Event_PucQuiz.question_result == "win")
        {
            points += pergunta_old.points;
            Event_PucQuiz.points = points;
        }

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

        if (question_actualy_index+1 != attributes.Length)
        {
            question_actualy_index++;

            question_actualy = Instantiate(config.Get_Layout(attributes[question_actualy_index].question_type), transform);
            Quiz pergunta_new = question_actualy.GetComponent<Quiz>();
            pergunta_new.attributes = attributes[question_actualy_index];
        }
        else
        {
            Debug.Log("Points = "+points);
            Event_PucQuiz.Change_Scene("End");
        }
    }    
}
