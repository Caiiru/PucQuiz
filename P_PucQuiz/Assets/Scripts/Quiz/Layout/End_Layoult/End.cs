using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class End
{
    Config_PucQuiz config;
    public Dictionary<int,QuizPlayer> players => Event_PucQuiz.players;
    public int length => players.Count;

    public Timer time;

    void Start()
    {
        
    }

    void Update()
    {

    }
}
