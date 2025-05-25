
using System;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [Header("Event Variables")]
    [SerializeField] private QuizPlayer player;
    [SerializeField] private string scene_actualy;
    [SerializeField] private string layout_actualy;
    [SerializeField] private string question_result;

    [Header("Manager Variables")]
    public static LayoutManager instance;
    public Login menu;
    public Modos quiz;
    [SerializeField] private bool quiz_start, menu_start = true;

    [Header("Multiplayer Variables")]
    [SerializeField] private bool multiplayer_on;


    public LayoutManager()
    {
        Debug.Log("Instance LayoutManager = true.");
        instance = this;
    }

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
        player = Event_PucQuiz.player;

        if (player != null)
        {
            Event_PucQuiz.points = player.Points;
        }

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
            Debug.Log("Call to Start Quiz");
            quiz.Awake(gameObject);
            quiz.Start(gameObject);
            quiz_start = false;
        }
        Debug.Log("Call to Update Quiz");
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

    //[Rpc(SendTo.NoServer)]
    private void SendToLocal(Dictionary<int,QuizPlayer> players)
    {
        Event_PucQuiz.players = players;
        
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].name == Event_PucQuiz.player_name)
            {
                Event_PucQuiz.player = players[i];
            }
        }
    }

    //[Rpc(SendTo.Server)]
    public void SendToHost(QuizPlayer player, Config_PucQuiz config, int streak, float time)
    {
        bool win = true; if (Event_PucQuiz.question_result == "lose") { win = false; }

        if (multiplayer_on)
        {
            //Enviar os valores para o Host calcular com o uso do -> Config_PucQuiz.Get_Points() <-.
        }
        else
        {
            Event_PucQuiz.points = Config_PucQuiz.Get_Points(win,streak,time);
            if (Event_PucQuiz.player != null) { player.SetPlayerPoints((int)Event_PucQuiz.points); }
        }
    }

    //Solicitar para o host o dicionario de pontos para atualizar.
}
