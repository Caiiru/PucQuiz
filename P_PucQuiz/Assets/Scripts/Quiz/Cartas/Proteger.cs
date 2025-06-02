using UnityEngine;
using static Cartas;

[CreateAssetMenu(fileName = "Proteger", menuName = "Cartas/Proteger")]
public class Card_Proteger : Cartas
{
    override
    public void Set()
    {
        instance = this;
        name = "Proteger";
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
