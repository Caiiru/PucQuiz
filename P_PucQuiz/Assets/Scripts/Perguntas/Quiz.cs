using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class Quiz : Perguntas
{
    #region @Variaveis Locais

    [Header("Variaveis Locais")]
    public int choice_max; //Numero max de escolhas.
    public int choice_actualy; //Numero atual escolhidos.
    public bool chose; //J� fez a escolha?

    #endregion

    public Quiz_Attributes attributes;

    [Header("Event Variables")]
    private string question_event = Event_PucQuiz.question_event; //Puxa o evento de respota e qual pergunta foi respondida.
    private bool question_lock = Event_PucQuiz.question_lock; //Puxa o evento que bloqueia as escolhas.
    public bool pause = Event_PucQuiz.pause; //
    public override void Pre_Load(GameObject mod)
    {
        if(attributes.timer == 0) { attributes.timer = 30; }
        attributes.choices = new bool[attributes.options.Length];
    }

    public override void Start_Layout(GameObject mod)
    {
        choice_max = attributes.choice_correct.Length;
        //throw new System.NotImplementedException();
    }

    public override void Update_Layout(GameObject mod)
    {
        if (Event_PucQuiz.start_layoult) { Start_Layout(mod); }

        pause = Event_PucQuiz.pause;
        if (pause) { return; }

        /* ---- Lembrete ---- *\
         * Caso o jogo esteja
         * pausado, o codigo ira
         * quebrar aqui e n�o
         * rodara o nada abaixo
        \*                    */

        switch (attributes.timer)
        {
            case -1:
                break;
            case 0:
                End_Layout(mod);
                break;
            default:
                attributes.timer = attributes.timer - Time.deltaTime;
                if (attributes.timer < 0) { attributes.timer = 0; } //Quebra caso o numero seja negativo, mas n�o igual a 1.
                break;
        }

        //Puxando Eventos relacionados a tentativa de resposta das questoes.
        question_event = Event_PucQuiz.question_event;
        question_lock = Event_PucQuiz.question_lock;

        if (question_event != "" && !question_lock)
        {
            if (chose && !attributes.change) { Debug.Log("Escolha feita."); return; } //Quebra a execu��o do codigo.

            if (choice_max == 1) { Choices_Reset(); }

            switch (question_event)
            {
                case "chose_01":
                    if (attributes.choices[0]) { choice_actualy--; attributes.choices[0] = false; }
                    else { choice_actualy++; attributes.choices[0] = true; }
                    Make_Chose();
                    break;
                case "chose_02":
                    if (attributes.choices[1]) { choice_actualy--; attributes.choices[1] = false; }
                    else { choice_actualy++; attributes.choices[1] = true; }
                    Make_Chose();
                    break;
                case "chose_03":
                    if (attributes.choices[2]) { choice_actualy--; attributes.choices[2] = false; }
                    else { choice_actualy++; attributes.choices[2] = true; }
                    Make_Chose();
                    break;
                case "chose_04":
                    if (attributes.choices[3]) { choice_actualy--; attributes.choices[3] = false; }
                    else { choice_actualy++; attributes.choices[3] = true; }
                    Make_Chose();
                    break;
                default:
                    throw new System.Exception("Evento n�o reconhecido.");
            }
        }

        /* ---- Lembrete ---- *\
         * Caso o usuario decida
         * n�o permitir mudar a
         * escolha, o programa
         * ira deixar de ser
         * executado desta parte
         * para baixo.
        \*                    */
    }

    public override void End_Layout(GameObject mod)
    {

        if (Event_PucQuiz.question_next) { return; }

        Event_PucQuiz.question_event = "";
        Event_PucQuiz.question_lock = false;

        bool uncorrect = false;

        for (int i = 0; i < attributes.choices.Length; i++)
        {
            if (attributes.choices[i] == true)
            {
                bool search = true;

                for(int o = 0; o < attributes.choice_correct.Length; o++)
                {
                    if (attributes.choice_correct[o] == i+1) { search = false; break; }
                }

                if(search) { uncorrect = true; }
                Debug.Log("i = " + search);
            }
        }

        if(!uncorrect)
        { 
            Debug.Log("You win");
            Event_PucQuiz.question_result = "win";
        }
        else
        {
            Debug.Log("You lose");
            Event_PucQuiz.question_result = "lose";
        }
        Debug.Log(uncorrect);

        Event_PucQuiz.question_next = true;

        Debug.Log("End Layoult");

        //throw new System.NotImplementedException();
    }

    #region || Fun��es Rapidas ||

    public void Choice_Event(string chose){ Debug.Log(chose); Event_PucQuiz.question_event = chose; }
    private void Make_Chose() { if (choice_actualy == choice_max) { chose = true; }; Event_PucQuiz.question_event = ""; }
    private void Choices_Reset()
    { 
        attributes.choices[0] = false; 
        attributes.choices[1] = false;
        attributes.choices[2] = false;
        attributes.choices[3] = false;
    }

    #endregion
}

[Serializable]
public class Quiz_Attributes : Attribute
{
    public int[] choice_correct; //Quais respostas est�o corretas.
    public bool change; //Pode mudar a resposta?
    public string[] options; //Texto de cada op��o
    public bool[] choices; //Bool que define quais op��es foram escolhidas.

    [Header("Time")]
    public float timer = 30; //Tempo max at� o fim da pergunta.
}

