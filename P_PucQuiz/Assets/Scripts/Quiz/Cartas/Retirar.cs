using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Carta", menuName = "Cartas/Retirar")]
public class Card_Retirar : Cartas
{
    override
    public void Set()
    {
        instance = this;
        //Não precisa ser hardcoded o nome, o intuito de ser scriptable object é a alteração facil pelo editor;
        //name = "Retirar";
        types = Card_Types.Retirar;
    }

    override
    public void Use()
    {
        LayoutManager manager = LayoutManager.instance;
        if (!manager.player.InCartas(types)) { return; }

        int index = manager.quiz.question_actualy_index;
        Quiz_Attributes attributes = manager.quiz.attributes[index];

        bool bad_use = true;

        int question_number = -1;
        float question_probability = 0;

        for(int i = 0; i < attributes.choice_correct.Length; i++)
        {
            if (!attributes.choice_correct[i])
            {
                bad_use = false;
                float rand = UnityEngine.Random.Range(0f,100f);
                if(question_probability < rand)
                {
                    question_number = i;
                    question_probability = rand;
                }
            }
        }

        if(!bad_use)
        {
            Debug.Log("Question Number = " + question_number);
            Debug.Log("A resposta e = Resposta_" + question_number.ToString());
            Button button = manager.quiz.doc.rootVisualElement.Q<Button>("Resposta_" + (question_number+1).ToString());

            button.style.color = Color.grey;
            button.SetEnabled(false);

            manager.player.RemoveCard(Card_Types.Retirar);
        }
    }
}
