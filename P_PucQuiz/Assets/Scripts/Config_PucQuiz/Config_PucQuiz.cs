using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Objects/PucKahoot/NewConfig")]
public class Config_PucQuiz : ScriptableObject
{
    public string[] types_modes;
    public StringToGameObject[] layout_list;
    //public string[] types_question;
    //public GameObject[] layout_question;

    public GameObject Get_Layout(string type)
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
    public string name;
    public GameObject gameObject;

    public GameObject GetObject(string test)
    {
        if(test == name) { return  gameObject; }

        return null;
    }
}