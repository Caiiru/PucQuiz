using System;
using Unity.Netcode;
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
public class Quiz_Attributes : Attributes, Unity.Netcode.INetworkSerializable
{
    [SerializeField] public bool[] choice_correct; //Quais respostas est�o corretas.
    [SerializeField] public bool change; //Pode mudar a resposta?
    [SerializeField] public string[] options; //Texto de cada op��o
    [SerializeField] public bool[] choices; //Bool que define quais op��es foram escolhidas.

    [Header("Time")]
    [SerializeField] public Timer timer = new Timer(30f); //Tempo max at� o fim da pergunta.
     

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    { 
        serializer.SerializeValue(ref question);
        serializer.SerializeValue(ref question_type);

        serializer.SerializeValue(ref change);

        if (serializer.IsWriter)
        {
            //choice correct
            int length = choice_correct != null ? choice_correct.Length : 0;
            serializer.SerializeValue(ref length); 
            for(int i = 0; i < length; i++)
            {
                serializer.SerializeValue(ref choice_correct[i]);
                serializer.SerializeValue(ref options[i]);
                serializer.SerializeValue(ref choices[i]);
            }

            
        }
        else
        {
            int length = 0; 
            serializer.SerializeValue(ref length);
            choice_correct = new bool[length];
            options = new string[length];
            choices = new bool[length];  
            for (int i = 0; i < length; i++)
            {
                serializer.SerializeValue(ref choice_correct[i]);
                serializer.SerializeValue(ref options[i]);
                serializer.SerializeValue(ref choices[i]);
            }
        }
        serializer.SerializeValue(ref timer.start);
        serializer.SerializeValue(ref timer.time);
        serializer.SerializeValue(ref timer.infinity);
    }
}