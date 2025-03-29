using System;
using UnityEngine;

[Serializable]
public abstract class Perguntas : MonoBehaviour
{
    [Header("Variaveis Gerais")]
    [Space]
    public string type;
    public string question;
    public int points = 10;

    public abstract void Pre_Load(GameObject mod);
    public abstract void Start_Layout(GameObject mod);
    public abstract void Update_Layout(GameObject mod);
    public abstract void End_Layout(GameObject mod);
}