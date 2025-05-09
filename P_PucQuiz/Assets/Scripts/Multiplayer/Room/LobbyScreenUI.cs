using System;
using TMPro;
using UnityEngine;

public class LobbyScreenUI : MonoBehaviour
{
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomCodeText;
    void Start()
    {
        if (playerListText == null || roomCodeText == null)
        {
            throw new NotImplementedException();
        }
        
    }

    public void UpdatePlayersListUI(String text)
    {
        playerListText.text = text;
    }

    public void UpdateRoomCodeText(String text)
    {
        roomCodeText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
