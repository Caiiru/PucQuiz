using UnityEngine;

public class QuizPlayer : MonoBehaviour
{

    [Header("Player Name")]
    private string _playerName;
    public string PlayerName => _playerName;
    [Space]
    [Header("Points")]

    private int _points;
    public int Points => _points;
    public void SetPlayerName(string newName)
    {
        _playerName = newName;
    }

    public void SetPlayerPoints(int points)
    {
        //garantee new points is greather than 0 
        _points = points <= 0 ? 0 : points;

    }
}
