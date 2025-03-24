using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Quiz : Perguntas
{
    [Header("Opções")]
    [Space]
    public int choice_correct;
    public bool chose;
    [Header("---------------")]
    public string Option1;
    public string Option2, Option3, Option4;
    [Header("---------------")]
    public bool choice1;
    public bool choice2, choice3, choice4;

    [Header("Time")]
    public float timer = 30;
    public bool pause = Event_PucQuiz.pause;

    [Header("Event Variables")]
    private string question_event = Event_PucQuiz.question_event;
    private bool question_lock = Event_PucQuiz.question_lock;
    private bool change = Event_PucQuiz.change;
    public override void Pre_Load(GameObject mod)
    {
        if(timer == 0) { timer = 30; }
    }

    public override void Start_Layout(GameObject mod)
    {
        //throw new System.NotImplementedException();
    }
    public override void Update_Layout(GameObject mod)
    {
        pause = Event_PucQuiz.pause;
        if (pause) { return; }

        /* ---- Lembrete ---- *\
         * Caso o jogo esteja
         * pausado, o codigo ira
         * quebrar aqui e não
         * rodara o nada abaixo
        \*                    */

        switch (timer)
        {
            case -1:
                break;
            case 0:
                End_Layout(mod);
                break;
            default:
                timer = timer - Time.deltaTime;
                if (timer < 0) { timer = 0; } //Quebra caso o numero seja negativo, mas não igual a 1.
                break;
        }

        //Puxando Eventos relacionados a tentativa de resposta das questoes.
        question_event = Event_PucQuiz.question_event;
        question_lock = Event_PucQuiz.question_lock;

        if (question_event != "" && !question_lock)
        {
            if (chose && change) { return; } //Quebra a execução do codigo.

            Choices_Reset();

            switch (question_event)
            {
                case "chose_01":
                    choice1 = true;
                    Make_Chose();
                    break;
                case "chose_02":
                    choice2 = true;
                    Make_Chose();
                    break;
                case "chose_03":
                    choice3 = true;
                    Make_Chose();
                    break;
                case "chose_04":
                    choice4 = true;
                    Make_Chose();
                    break;
                default:
                    throw new System.Exception("Evento não reconhecido.");
            }
        }

        /* ---- Lembrete ---- *\
         * Caso o usuario decida
         * não permitir mudar a
         * escolha, o programa
         * ira deixar de ser
         * executado desta parte
         * para baixo.
        \*                    */
    }
    public override void End_Layout(GameObject mod)
    {
        throw new System.NotImplementedException();
    }

    #region || Funções Rapidas ||

    private void Make_Chose() { chose = true; Event_PucQuiz.question_event = ""; }
    private void Choices_Reset(){ choice1 = false; choice2 = false; choice3 = false; choice4 = false; }

    #endregion
}
