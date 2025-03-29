using System;
using UnityEditor.Build.Content;
using UnityEngine;

public class Modos : MonoBehaviour
{
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] private GameObject[] questions;
    [SerializeField] private GameObject question_actualy;
    [SerializeField] private int question_actualy_index;

    [SerializeField] private int points = 0;
    public void Awake()
    {
        question_actualy = Instantiate(questions[0],transform);
        
        if (config == null) { config = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }

        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i] == null) { return; }

            Perguntas pergunta = questions[i].GetComponent<Perguntas>();
            pergunta.Pre_Load(transform.gameObject);
        }
    }
    public void Start()
    {
        Event_PucQuiz.start_layoult = true;
        question_actualy_index = 0;
    }
    public void Update()
    {
        if (question_actualy != null)
        { 
            Perguntas pergunta = question_actualy.GetComponent<Perguntas>();
            
            pergunta.Update_Layout(transform.gameObject);
        }

        if (Event_PucQuiz.question_next)
        {
            Change_Question();
        }
    }

    private void Change_Question()
    {
        Perguntas pergunta_old = question_actualy.GetComponent<Perguntas>();

        if (Event_PucQuiz.question_result == "win")
        {
            points += pergunta_old.points;
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

        if (question_actualy_index + 1 != questions.Length)
        {
            question_actualy_index++;

            question_actualy = Instantiate(questions[question_actualy_index],transform);
        }

        Event_PucQuiz.question_next = false;
    }    
}
