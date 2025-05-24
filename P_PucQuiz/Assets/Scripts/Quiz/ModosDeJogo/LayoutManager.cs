
using System;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [Header("Event Variables")]

    [SerializeField] private string scene_actualy;
    [SerializeField] private string layout_actualy;
    [SerializeField] private string question_result;

    [Header("Manager Variables")]
    public Login menu;
    public Modos quiz;
    [SerializeField] private bool quiz_start, menu_start = true;

    public void Awake()
    {
        Event_PucQuiz.scene_actualy = "Menu";

        quiz.transform = transform;
    }

    public void Update()
    {
        scene_actualy = Event_PucQuiz.scene_actualy;
        layout_actualy = Event_PucQuiz.layout_actualy;
        question_result = Event_PucQuiz.question_result;

        switch (Event_PucQuiz.scene_actualy)
        {
            case "Quiz":
                Quiz_Run();
                break;
            case "Menu":
                Menu_Run();
                break;
        }
        
    }

    private void Quiz_Run()
    {
        if (quiz_start)
        {
          //  Debug.Log("Call to Start Quiz");
            quiz.Awake(gameObject);
            quiz.Start(gameObject);
            quiz_start = false;
        }
        //Debug.Log("Call to Update Quiz");
        quiz.Update(gameObject);
    }

    private void Menu_Run()
    {
        if (menu_start)
        {
            menu.Awake();
            menu.Start();
            menu_start = false;
        }
        menu.Update();
    }

    //Mandar para o host as informa��es.

    //Solicitar para o host o dicionario de pontos para atualizar.
}
