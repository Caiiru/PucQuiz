using System;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct Question : INetworkSerializable
{
    public FixedString512Bytes QuestionText;
    public FixedString128Bytes OptionA;
    public FixedString128Bytes OptionB;
    public FixedString128Bytes OptionC;
    public FixedString128Bytes OptionD;

    public int CorrectAnswerIndex; // 0 para A, 1 para B, 2 para C, 3 para D

    public Question(string text, string optionA, string optionB, string optionC, string optionD, int answerIndex)
    {
        QuestionText = new FixedString512Bytes(text);
        OptionA = new FixedString128Bytes(optionA);
        OptionB = new FixedString128Bytes(optionB);
        OptionC = new FixedString128Bytes(optionC);
        OptionD = new FixedString128Bytes(optionD);
        CorrectAnswerIndex = answerIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref QuestionText);
        serializer.SerializeValue(ref OptionA);
        serializer.SerializeValue(ref OptionB);
        serializer.SerializeValue(ref OptionC);
        serializer.SerializeValue(ref OptionD);
        serializer.SerializeValue(ref CorrectAnswerIndex);


    }
}