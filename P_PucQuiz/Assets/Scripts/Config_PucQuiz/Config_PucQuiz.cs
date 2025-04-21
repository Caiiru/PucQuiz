using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Objects/PucKahoot/NewConfig")]
public class Config_PucQuiz : ScriptableObject
{
    private static Config_PucQuiz instance;

    public string[] types_modes;
    public StringToGameObject[] layout_list;

    [Header("Scenes")]
    public string Layout_Start;
    public string Layout_Contagem;
    public string Layout_Game;

    public static Config_PucQuiz Get_Config()
    {
        if(instance == null) { instance = Resources.Load<Config_PucQuiz>("Config/PucQuiz"); }
        return instance;
    }
    public GameObject Get_Layout(Attributes.Type type)
    {
        for(int i = 0; i < layout_list.Length; i++)
        {
            GameObject layout = layout_list[i].GetObject(type);
            if (layout != null) { return layout; }
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