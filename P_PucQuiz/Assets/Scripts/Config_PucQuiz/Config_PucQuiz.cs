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
    public static float Get_Points(bool win, int streak, float speed)
    {
        Config_PucQuiz config = Config_PucQuiz.Get_Config();

        Debug.Log("--------------------------------");
        Debug.Log("Vitoria = " + win);
        Debug.Log("Streak Atual = " + streak);
        Debug.Log("Speed Atual = " + speed);
        Debug.Log("--------------------------------");

        float base_ = config.base_incorrect;

        if (win) { base_ = config.base_correct; }
        else if(streak > 0) { streak = 0; }

        float rec = 0;
        if (streak < 0) { rec = config.bonus_recuperacao; }

        float points = base_ + rec + (config.bonus_streak*streak) + (config.bonus_velocidade*speed);

        Debug.Log("--------------------------------");
        Debug.Log("Points = " + points);
        Debug.Log("Streak Bonus = " + config.bonus_streak * streak);
        Debug.Log("Speed Bonus = " + config.bonus_velocidade * speed);
        Debug.Log("--------------------------------");

        return points;
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