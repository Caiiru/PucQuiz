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
    public float bonus_recuperação;
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
    public static float Get_Points(bool win, int streak, int speed)
    {
        Config_PucQuiz config = Config_PucQuiz.Get_Config();

        float base_ = config.base_incorrect;
        if (win) { base_ = config.base_correct; }

        float rec = 0;
        if (true) { rec = config.bonus_recuperação; }

        float points = base_ + rec + (config.bonus_streak*streak) + (config.bonus_velocidade*speed);
        
        return points;
    }
    public Perguntas Get_Layout(Attributes.Type type)
    {
        switch(type)
        {
            case Attributes.Type.quiz:
                return quiz;
            case Attributes.Type.verdadeiroOUfalso:
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