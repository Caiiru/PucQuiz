using System;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;

[Serializable]
public class Translator
{
    private static Translator instance;
    public static Translator Get()
    {
        if (instance == null) { instance = new Translator(); }
        return instance;
    }

    public void Save_Attributes(Attributes[] attributes, string path)
    {
        Attributes_Save attributes_save = new Attributes_Save();
        var save = JsonUtility.ToJson(attributes_save);
        System.IO.File.WriteAllText(path,save);
    }
    public Attributes Load_Attributes(Attributes[] attributes, string path)
    { 
        var load = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<Attributes>(load);
    }
}

[Serializable]
public class Attributes_Save
{
    #region @Variaveis Gerais
    [SerializeField] public int question_type; //O tipo da questão em int.
    [SerializeField] public string question; //A questão que busca-se a resposta.

    [SerializeField] public float timer = 30; //Tempo max até o fim da pergunta.
    #endregion

    #region @Variaveis Especificas
    [SerializeField] public String[] choice_correct; //Quais respostas estão corretas.
    [SerializeField] public bool change; //Pode mudar a resposta?
    [SerializeField] public string[] options; //Texto de cada opção
    [SerializeField] public bool[] choices; //Bool que define quais opções foram escolhidas.
    #endregion

    public void Save(Quiz_Attributes quiz)
    {

    }

    public Quiz_Attributes Load_Quiz()
    {

        Quiz_Attributes quiz = new Quiz_Attributes();

        quiz.question_type = (Attributes.Type)question_type;
        quiz.question = question;
        quiz.timer.start = timer;
        quiz.timer.time = timer;

        for (int i = 0; i < choice_correct.Length; i++)
        {

        }


        return quiz;
    }
}