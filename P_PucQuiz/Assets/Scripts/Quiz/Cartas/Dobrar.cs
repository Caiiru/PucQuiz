using UnityEngine;

[CreateAssetMenu(fileName = "Dobrar", menuName = "Cartas/Dobrar")]
public class Card_Dobrar : Cartas
{
    override
    public void Set()
    {
        instance = this;

        //Não precisa ser hardcoded o nome, o intuito de ser scriptable object é a alteração facil pelo editor;
        //name = "Dobrar";
        types = Card_Types.Dobrar;
    }

    override
    public void Use()
    {
        //if (!LayoutManager.instance.player.InCartas(types)) { return; }
        LayoutManager.instance.player.dobrar = this;
        //LayoutManager.instance.player.RemoveCard(Card_Types.Dobrar);
    }
}
