using System;
using UnityEngine;

[Serializable]
public class Attributes
{
    [SerializeField] public Attributes.Type question_type; //O tipo da quest�o.
    [SerializeField] public string question; //A quest�o que busca-se a resposta.

    public enum Type
    {
        none,
        Quiz,
        VerdadeiroOuFalso
    }
}

[Serializable]
public class Quiz_Attributes : Attributes
{
    [SerializeField] public bool[] choice_correct; //Quais respostas est�o corretas.
    [SerializeField] public bool change; //Pode mudar a resposta?
    [SerializeField] public string[] options; //Texto de cada op��o
    [SerializeField] public bool[] choices; //Bool que define quais op��es foram escolhidas.

    [Header("Time")]
    [SerializeField] public Timer timer = new Timer(30f); //Tempo max at� o fim da pergunta.
}