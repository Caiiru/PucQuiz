using System;
using UnityEngine;

[Serializable]
public class Attributes
{
    [SerializeField] public Attributes.Type question_type; //O tipo da questão.
    [SerializeField] public string question; //A questão que busca-se a resposta.

    public enum Type
    {
        none,
        quiz,
        verdadeiroOUfalso
    }
}

[Serializable]
public class Quiz_Attributes : Attributes
{
    [SerializeField] public int[] choice_correct; //Quais respostas estão corretas.
    [SerializeField] public bool change; //Pode mudar a resposta?
    [SerializeField] public string[] options; //Texto de cada opção
    [SerializeField] public bool[] choices; //Bool que define quais opções foram escolhidas.

    [Header("Time")]
    [SerializeField] public Timer timer = new Timer(30f); //Tempo max até o fim da pergunta.
}