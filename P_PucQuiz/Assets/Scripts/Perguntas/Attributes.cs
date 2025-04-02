using System;
using UnityEngine;

[Serializable]
public class Attributes
{
    
}

[Serializable]
public class Quiz_Attributes : Attributes
{
    public string question_type; //O tipo da quest�o.
    public string question; //A quest�o que busca-se a resposta.
    public int[] choice_correct; //Quais respostas est�o corretas.
    public bool change; //Pode mudar a resposta?
    public string[] options; //Texto de cada op��o
    public bool[] choices; //Bool que define quais op��es foram escolhidas.

    [Header("Time")]
    public float timer = 30; //Tempo max at� o fim da pergunta.
}

public class Attributes_Save
{
    public Attributes[] attributes;
}