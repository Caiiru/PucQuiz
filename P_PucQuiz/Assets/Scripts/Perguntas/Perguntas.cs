using System;
using UnityEngine;

[Serializable]
public abstract class Perguntas : MonoBehaviour
{

    public string type;

    public abstract void Pre_Load(GameObject mod);
    public abstract void Start_Layout(GameObject mod);
    public abstract void Update_Layout(GameObject mod);
    public abstract void End_Layout(GameObject mod);
}