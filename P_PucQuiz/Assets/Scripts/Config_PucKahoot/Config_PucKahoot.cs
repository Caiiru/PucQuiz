using UnityEngine;

[CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Objects/PucKahoot/NewConfig")]
public class Config_PucKahoot : ScriptableObject
{
    public string[] types_modes;
    public string[] types_question;
    public GameObject[] layout_question;
}
