using System;
using UnityEngine;

[Serializable]
public class Attributes
{
    
}

[Serializable]
public class Quiz_Attributes : Attributes
{
    public string question_type; //O tipo da questão.
    public string question; //A questão que busca-se a resposta.
    public int[] choice_correct; //Quais respostas estão corretas.
    public bool change; //Pode mudar a resposta?
    public string[] options; //Texto de cada opção
    public bool[] choices; //Bool que define quais opções foram escolhidas.

    [Header("Time")]
    public float timer = 30; //Tempo max até o fim da pergunta.
}

public class Attributes_Save
{
    public Attributes[] attributes;
}