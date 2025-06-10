using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Quiz : Perguntas
{
    [Header("Variaveis Locais")]
    public int choice_max; //Numero max de escolhas.
    public int choice_actualy; //Numero atual escolhidos.
    public bool chose; //Ja fez a escolha?

    [Header("Questions")]
    public Modos mod = null;
    public float speed_to_complet;
    public Quiz_Attributes attributes;

    [Header("Event Variables")]
    private string question_event = Event_PucQuiz.question_event; //Puxa o evento de respota e qual pergunta foi respondida.
    private bool question_lock = Event_PucQuiz.question_lock; //Puxa o evento que bloqueia as escolhas.
    public bool pause = Event_PucQuiz.pause;

    public override void Pre_Load(GameObject obj)
    {

        if (attributes.timer.start == 0) { attributes.timer.start = 30; }
        choice_actualy = 0;
        attributes.timer.Reset();
    }

    public override void Start_Layout(GameObject obj)
    {
        Event_PucQuiz.start_layout = false;

        Pre_Load(obj);

        /*for(int i = 0; i < attributes.choices.Length; i++)
        {
            if(attributes)
        }*/

        attributes.choices = new bool[attributes.options.Length];
    }

    public override void Update_Layout(GameObject obj)
    {

        if (Event_PucQuiz.start_layout) { Start_Layout(obj); }

        int choice_max_local = 0;

        foreach (bool i in attributes.choice_correct)
        {
            if (i)
            {
                choice_max_local++;
            }
        }

        choice_max = choice_max_local;

        pause = Event_PucQuiz.pause;
        if (pause) { Debug.Log("-- Game paused --"); return; }

        if (!chose)
        {
            speed_to_complet = (int)attributes.timer.time;
        }
        /* ---- Lembrete ---- *\
         * Caso o jogo esteja
         * pausado, o codigo ira
         * quebrar aqui e nao
         * rodara o nada abaixo
        \*                    */

        if (!attributes.timer.infinity)
            switch (attributes.timer.time)
            {
                case 0:
                    End_Layout(obj);
                    break;
                default:
                    attributes.timer.Run();
                    break;
            }

        //Puxando Eventos relacionados a tentativa de resposta das questoes.
        question_event = Event_PucQuiz.question_event;
        question_lock = Event_PucQuiz.question_lock;

        if (question_event != "" && !question_lock)
        {
            if (chose && !attributes.change) { Debug.Log("Escolha feita."); question_event = ""; return; } //Quebra a execucao do codigo.

            if (choice_max == 1) { Choices_Reset(); }

            switch (question_event)
            {
                case "chose_01":
                    if (attributes.choices[0]) { choice_actualy--; attributes.choices[0] = false; }
                    else { choice_actualy++; attributes.choices[0] = true; }
                    break;
                case "chose_02":
                    if (attributes.choices[1]) { choice_actualy--; attributes.choices[1] = false; }
                    else { choice_actualy++; attributes.choices[1] = true; }
                    break;
                case "chose_03":
                    if (attributes.choices[2]) { choice_actualy--; attributes.choices[2] = false; }
                    else { choice_actualy++; attributes.choices[2] = true; }
                    break;
                case "chose_04":
                    if (attributes.choices[3]) { choice_actualy--; attributes.choices[3] = false; }
                    else { choice_actualy++; attributes.choices[3] = true; }
                    break;
                default:
                    throw new System.Exception("Evento nao reconhecido.");
            }

            Make_Chose();
        }

        /* ---- Lembrete ---- *\
         * Caso o usuario decida
         * nao permitir mudar a
         * escolha, o programa
         * ira deixar de ser
         * executado desta parte
         * para baixo.
        \*                    */
    }

    public override void End_Layout(GameObject obj)
    {
        if (Event_PucQuiz.question_next) { return; }

        MyPlayer player = LayoutManager.instance.player;
        Event_PucQuiz.question_event = "";
        Event_PucQuiz.question_lock = false;


        bool correct = true;

        for (int i = 0; i < attributes.choice_correct.Length; i++)
        {
            if (!attributes.choices[i] == attributes.choice_correct[i])
            {
                correct = false;
                break;
            }
        }

        speed_to_complet = (1 * speed_to_complet) / attributes.timer.start;
        Debug.Log("% do Buff de velocidade = " + speed_to_complet);

        Event_PucQuiz.question_result = correct ? "win" : "lose";
        player.points += (int)Config_PucQuiz.Get_Points(correct, speed_to_complet);
        Event_PucQuiz.points = player.points;

        //ADD CARD HERE
        if (Event_PucQuiz.question_result == "lose")
        {
            var r = new System.Random().Next(CardsManager.Instance.AllCards.Count - 1);
            var nextCard = CardsManager.Instance.AllCards[r];
            GameManager.Instance.LocalPlayer.AddCardByID(nextCard.cardID);
        }
        GameManager.Instance.LocalPlayer.Score.Value = player.points;


        mod.FeedBack();
        Event_PucQuiz.question_next = true;

    }

    #region ClickEvents

    public void ClickPergunta1(ClickEvent click)
    {
        Choice_Event("chose_01");
    }
    public void ClickPergunta2(ClickEvent click)
    {
        Choice_Event("chose_02");
    }
    public void ClickPergunta3(ClickEvent click)
    {
        Choice_Event("chose_03");
    }
    public void ClickPergunta4(ClickEvent click)
    {
        Choice_Event("chose_04");
    }

    #endregion

    #region || Funcoes Rapidas ||

    public void Choice_Event(string chose) { Debug.Log(chose); Event_PucQuiz.question_event = chose; }
    private void Make_Chose() { if (choice_actualy == choice_max) { chose = true; } Event_PucQuiz.question_event = ""; }
    private void Choices_Reset()
    {
        Debug.Log("Reset Choices");
        attributes.choices[0] = false;
        attributes.choices[1] = false;
        attributes.choices[2] = false;
        attributes.choices[3] = false;
    }

    #endregion
}