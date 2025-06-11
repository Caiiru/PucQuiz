using UnityEngine;
using static Cartas;

[CreateAssetMenu(fileName = "Proteger", menuName = "Cartas/Proteger")]
public class Card_Proteger : Cartas
{
    override
    public void Set()
    {
        instance = this;
        //Não precisa ser hardcoded o nome, o intuito de ser scriptable object é a alteração facil pelo editor;
        //name = "Proteger";
        types = Card_Types.Proteger;
    }

    override
    public void Use()
    {
        if (!LayoutManager.instance.player.InCartas(types)) { return; }
        LayoutManager.instance.player.protetor = this;
        LayoutManager.instance.player.RemoveCard(Card_Types.Proteger);
    }
}
