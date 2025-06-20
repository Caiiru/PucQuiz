using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Objects/PucKahoot/NewConfig")]
public class Config_PucQuiz : ScriptableObject
{
    private static Config_PucQuiz instance;

    public string[] types_modes;

    [Header("Question Types")]
    public Quiz quiz;
    //public StringToGameObject[] layout_list;

    [Header("Quiz")]
    public float base_correct;
    public float base_incorrect;
    public float bonus_streak;
    public float bonus_recuperacao;
    public float bonus_velocidade;

    [Header("Scenes")]
    public string Layout_Start;
    public string Layout_Contagem;
    public string Layout_Game;

    public static Config_PucQuiz Get_Config()
    {
        if(instance == null) { instance = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }
        return instance;
    }
    public static int Get_Points(bool win, float speed)
    {
        Config_PucQuiz config = Config_PucQuiz.Get_Config();
        MyPlayer player = LayoutManager.instance.player;

        //Debug.Log("--------------------------------");
        //Debug.Log("Vitoria = " + win);
        //Debug.Log("Streak Atual = " + Event_PucQuiz.streak);
        //Debug.Log("Speed Atual = " + speed);
        //Debug.Log("--------------------------------");

        float base_ = config.base_incorrect;
        float rec = 0;
        float real_bonus_streak = 0;

        if (win) 
        { 
            base_ = config.base_correct;

            if (Event_PucQuiz.streak < 0) { Event_PucQuiz.streak = 0; }
            else { Event_PucQuiz.streak++; }
        }
        else if(Event_PucQuiz.streak > 0) 
        { 
            if (!player.protetor) { Event_PucQuiz.streak = 0; }
        }
        else { Event_PucQuiz.streak --; }

        if (Event_PucQuiz.streak < 0) { rec = config.bonus_recuperacao; }

        if (Event_PucQuiz.streak > 0) { real_bonus_streak = config.bonus_streak * Event_PucQuiz.streak; }

        if (player.velocidade)
        {
            speed = 1;
        }
        float points = base_ + rec + real_bonus_streak + (config.bonus_velocidade*speed);

        //Debug.Log("--------------------------------");
        //Debug.Log("Points = " + points);
        //Debug.Log("Streak Bonus = " + config.bonus_streak * Event_PucQuiz.streak);
        //Debug.Log("Speed Bonus = " + config.bonus_velocidade * speed);
        //Debug.Log("--------------------------------");
        
        if (player.protetor) { player.protetor = false; }

        if (player.dobrar) { player.dobrar = false; points = points * 2; }
        
        Event_PucQuiz.new_points = (int)points;
        return (int)points;
    }
    public Perguntas Get_Layout(Attributes.Type type)
    {
        switch(type)
        {
            case Attributes.Type.Quiz:
                return quiz;
            case Attributes.Type.VerdadeiroOuFalso:
                return null;
        }

        return null;
    }
}

[Serializable]
public class StringToGameObject
{
    public Attributes.Type type;
    public GameObject gameObject;

    public GameObject GetObject(Attributes.Type test)
    {
        if(type == test) { return  gameObject; }

        return null;
    }
}