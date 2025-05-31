
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEditor.EditorTools;
using UnityEditor.Toolbars;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public static LayoutManager instance;

    [Header("Event Variables")]
    public QuizPlayer player;
    [SerializeField] private string scene_actualy;
    [SerializeField] private string layout_actualy;
    [SerializeField] private string question_result;

    [Header("Manager Variables")]
    public Login menu;
    public Modos quiz;
    public End end;
    [Header("Manager Variables - Start Bools",order = 1)]
    [SerializeField] public bool quiz_start = true;
    [SerializeField] public bool menu_start, end_start = true;

    [Header("Multiplayer Variables")]
    [SerializeField] private bool multiplayer_on;

    

    public LayoutManager()
    { 
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
            //Event_PucQuiz.points = player.Points;
            //Event_PucQuiz.points = Set(Event_PucQuiz.points, player.Points);
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
            case "End":
                End_Run();
                break;
        }
        
    }

    /// <summary>
    /// Tenta colocar um valor Y em um valor X de forma mais segura.
    /// </summary>
    /// <param name="x">Esta � a variavel que voce deseja alterar.</param>
    /// <param name="y">Esta � a variavel que representa o novo valor desejado.</param>
    /// <returns>
    /// Retorna o valor X se Y n�o puder ser atribuido ou retorna Y se ele puder ser atribuido.
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
            // Tenta converter se for poss�vel (ex.: string -> int, int -> double, etc.)
            return (T)Convert.ChangeType(y, typeof(T));
        }
        catch
        {
            // Se n�o conseguir converter, mant�m o valor anterior
            return x;
        }
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
    private void End_Run()
    {
        if(end_start)
        {
            end.Awake(gameObject);
            end.Start(gameObject);
            end_start = false;
        }
        end.Update(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeMenu(string scene, string layout)
    {
        if (!QuizLobby.Instance.GetIsHost()) { return; }

        switch(scene)
        {
            case "Start":
                menu.ChangeMenu(layout);
                break;
            case "Quiz":
                quiz.ChangeMenu(layout);
                break;
            case "End":
                end.ChangeMenu(layout);
                break;
        }
    }
    //[Rpc(SendTo.NoServer)]
    private void SendToLocal(Dictionary<int,QuizPlayer> players)
    {
        Event_PucQuiz.players = players;
        
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].playerName.Value == Event_PucQuiz.player_name)
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
            //Event_PucQuiz.points = Config_PucQuiz.Get_Points(win,streak,time);
            //if (Event_PucQuiz.player != null) { player.SetPlayerPoints((int)Event_PucQuiz.points); }
            Event_PucQuiz.points = Config_PucQuiz.Get_Points(win,Event_PucQuiz.streak,time);
            //if (Event_PucQuiz.player != null) { player.SetPlayerPoints((int)Event_PucQuiz.points); Debug.Log("Player not exist"); }
        }
    }
    //Mandar para o host as informa��es.

    //Solicitar para o host o dicionario de pontos para atualizar.
}
