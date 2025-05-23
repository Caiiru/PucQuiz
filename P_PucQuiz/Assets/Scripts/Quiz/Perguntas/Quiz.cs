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
    public Quiz_Attributes attributes;

    [Header("Event Variables")]
    private string question_event = Event_PucQuiz.question_event; //Puxa o evento de respota e qual pergunta foi respondida.
    private bool question_lock = Event_PucQuiz.question_lock; //Puxa o evento que bloqueia as escolhas.
    public bool pause = Event_PucQuiz.pause;

    public override void Pre_Load(GameObject obj)
    {

        if (attributes.timer.start == 0) { attributes.timer.start = 30; }
        attributes.timer.Reset();
    }

    public override void Start_Layout(GameObject obj)
    {
        Event_PucQuiz.start_layout = false;

        Pre_Load(obj);
        
        choice_max = attributes.choice_correct.Length;
        attributes.choices = new bool[attributes.options.Length];
        
        //throw new System.NotImplementedException();
    }

    public override void Update_Layout(GameObject obj)
    {
        if (Event_PucQuiz.start_layout) { Start_Layout(obj); }

        pause = Event_PucQuiz.pause;
        if (pause) { return; }

        /* ---- Lembrete ---- *\
         * Caso o jogo esteja
         * pausado, o codigo ira
         * quebrar aqui e nao
         * rodara o nada abaixo
        \*                    */

        if(!attributes.timer.infinity)
            switch (attributes.timer.time)
            {
                case 0:
                    End_Layout(obj);
                    break;
                default:
                    attributes.timer.Run();
                    //attributes.timer = attributes.timer - Time.deltaTime;
                    //if (attributes.timer < 0) { attributes.timer = 0; } //Quebra caso o numero seja negativo, mas n�o igual a 1.
                    break;
            }

        //Puxando Eventos relacionados a tentativa de resposta das questoes.
        question_event = Event_PucQuiz.question_event;
        question_lock = Event_PucQuiz.question_lock;

        if (question_event != "" && !question_lock)
        {
            if (chose && !attributes.change) { Debug.Log("Escolha feita."); return; } //Quebra a execucao do codigo.

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
        mod.FeedBack();

        if (Event_PucQuiz.question_next) { return; }

        Event_PucQuiz.question_event = "";
        Event_PucQuiz.question_lock = false;


        bool uncorrect = true;

        for (int i = 0; i < attributes.choices.Length; i++)
        {
            if (attributes.choices[i] = attributes.choice_correct[i])
            {
                uncorrect = false;
            }
            else
            {
                uncorrect = true;
            }
        }

        if(!uncorrect)
        { 
            Event_PucQuiz.question_result = "win";
            //Calcular pontos.
            //Event_PucQuiz.points += Config_PucQuiz.Get_Points(true,);
        }
        else
        {
            Event_PucQuiz.question_result = "lose";
            //Calcular pontos.
        }
        Event_PucQuiz.question_next = true;

    }

    #region ClickEvents

    public void ClickPergunta1(ClickEvent click)
    {
        Debug.Log("Pergunta1");
    }
    public void ClickPergunta2(ClickEvent click)
    {
        Debug.Log("Pergunta2");
    }
    public void ClickPergunta3(ClickEvent click)
    {
        Debug.Log("Pergunta3");
    }
    public void ClickPergunta4(ClickEvent click)
    {
        Debug.Log("Pergunta4");
    }

    #endregion

    #region || Funcoes Rapidas ||

    public void Choice_Event(string chose){ Debug.Log(chose); Event_PucQuiz.question_event = chose; }
    private void Make_Chose() { if (choice_actualy == choice_max) { chose = true; }; Event_PucQuiz.question_event = ""; }
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