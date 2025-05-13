using System; 
using TMPro; 
using UnityEngine; 

[Serializable]
public class Quiz : Perguntas
{
    #region @Variaveis Locais

    [Header("Variaveis Locais")]
    public int choice_max; //Numero max de escolhas.
    public int choice_actualy; //Numero atual escolhidos.
    public bool chose; //J� fez a escolha?

    [Header("Questions")]
    public TextMeshProUGUI points_text;
    public TextMeshProUGUI[] questions_text;

    #endregion

    public Quiz_Attributes attributes;

    #region @Events

    [Header("Event Variables")]
    private string question_event = Event_PucQuiz.question_event; //Puxa o evento de respota e qual pergunta foi respondida.
    private bool question_lock = Event_PucQuiz.question_lock; //Puxa o evento que bloqueia as escolhas.
    public bool pause = Event_PucQuiz.pause; //

    #endregion

    public override void Pre_Load(GameObject mod)
    {
        if(attributes.timer.start == 0) { attributes.timer.start = 30; }
        attributes.timer.Reset();

        question_text.text = attributes.question;

        for(int i = 0; i < questions_text.Length; i++)
        {
            if (questions_text[i] != null && attributes.options[i] != null)
            {
                questions_text[i].text = attributes.options[i];
            }
        }
    }

    public override void Start_Layout(GameObject mod)
    {
        Event_PucQuiz.start_layout = false;

        Pre_Load(mod);
        
        choice_max = attributes.choice_correct.Length;
        attributes.choices = new bool[attributes.options.Length];
        
        //throw new System.NotImplementedException();
    }

    public override void Update_Layout(GameObject mod)
    {
        if (Event_PucQuiz.start_layout) { Start_Layout(mod); }

        pause = Event_PucQuiz.pause;
        if (pause) { return; }

        /* ---- Lembrete ---- *\
         * Caso o jogo esteja
         * pausado, o codigo ira
         * quebrar aqui e n�o
         * rodara o nada abaixo
        \*                    */
        points_text.text = "Points : "+((int)Event_PucQuiz.points+" | Tempo : "+ ((int)attributes.timer.time));

        if(!attributes.timer.infinity)
            switch (attributes.timer.time)
            {
                case 0:
                    End_Layout(mod);
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
            if (chose && !attributes.change) { Debug.Log("Escolha feita."); return; } //Quebra a execu��o do codigo.

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
                    throw new System.Exception("Evento n�o reconhecido.");
            }

            Make_Chose();
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
        FeedBack();

        if (Event_PucQuiz.question_next) { return; }

        Event_PucQuiz.question_event = "";
        Event_PucQuiz.question_lock = false;


        bool uncorrect = true;

        for (int i = 0; i < attributes.choices.Length; i++)
        {
            if (attributes.choices[i] == true)
            {
                bool search = true;

                for(int o = 0; o < attributes.choice_correct.Length; o++)
                {
                    if (attributes.choice_correct[o] == i+1) { search = false; uncorrect = false; break; }
                }

                if(search) { uncorrect = true; }
            }
        }

        if(!uncorrect)
        { 
            Event_PucQuiz.question_result = "win";
            Event_PucQuiz.points += points;
        }
        else
        {
            Event_PucQuiz.question_result = "lose";
        }
        Event_PucQuiz.question_next = true;

    }

    #region || Fun��es Gerais ||

    private void FeedBack()
    {
        for(int i = 0; i < attributes.choices.Length; i++)
        {
            for(int o = 0; o < attributes.choice_correct.Length; o++)
            {
                if (i == attributes.choice_correct[o]-1)
                {
                    questions_text[i].color = Color.green; break;
                }
                else
                {
                    questions_text[i].color = Color.red;
                }
            }
        }
    }

    #endregion

    #region || Fun��es Rapidas ||

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