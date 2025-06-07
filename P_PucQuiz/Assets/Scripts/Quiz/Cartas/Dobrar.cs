using UnityEngine;

[CreateAssetMenu(fileName = "Dobrar", menuName = "Cartas/Dobrar")]
public class Card_Dobrar : Cartas
{
    override
    public void Set()
    {
        instance = this;
        name = "Dobrar";
        types = Card_Types.Dobrar;
    }

    override
    public void Use()
    {
        if (!LayoutManager.instance.player.InCartas(types)) { return; }
        LayoutManager.instance.player.dobrar = this;
        LayoutManager.instance.player.RemoveCard(Card_Types.Dobrar);
    }
}
