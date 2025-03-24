using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Objects/PucKahoot/NewConfig")]
public class Config_PucQuiz : ScriptableObject
{
    public string[] types_modes;
    public string[] types_question;
    public GameObject[] layout_question;
}
