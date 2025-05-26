
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

        if (player != null) //Player to Events
        {
            Event_PucQuiz.points = Set(Event_PucQuiz.points, player.Points);
            //Event_PucQuiz.streak = player.Streak; @ Caso va colocar a streak no player.
            
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

    /// <summary>
    /// Tenta colocar um valor Y em um valor X de forma mais segura.
    /// </summary>
    /// <param name="x">Esta é a variavel que voce deseja alterar.</param>
    /// <param name="y">Esta é a variavel que representa o novo valor desejado.</param>
    /// <returns>
    /// Retorna o valor X se Y não puder ser atribuido ou retorna Y se ele puder ser atribuido.
    /// </returns>
    private T Set<T>(T x, object y)
    {
        if (x is T)
        {
            return (T)y;
        }
        //Ideia do Gepeto abaixo ksksks
        try
        {
            // Tenta converter se for possível (ex.: string -> int, int -> double, etc.)
            return (T)Convert.ChangeType(y, typeof(T));
        }
        catch
        {
            // Se não conseguir converter, mantém o valor anterior
            return x;
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
    public void SendToHost(float time)
    {
        bool win = true; if (Event_PucQuiz.question_result == "lose") { win = false; }

        if (multiplayer_on)
        {
            //Enviar os valores para o Host calcular com o uso do -> Config_PucQuiz.Get_Points() <-.
        }
        else
        {
            Event_PucQuiz.points = Config_PucQuiz.Get_Points(win,Event_PucQuiz.streak,time);
            if (Event_PucQuiz.player != null) { player.SetPlayerPoints((int)Event_PucQuiz.points); Debug.Log("Player not exist"); }
        }
    }

    //Solicitar para o host o dicionario de pontos para atualizar.
}
