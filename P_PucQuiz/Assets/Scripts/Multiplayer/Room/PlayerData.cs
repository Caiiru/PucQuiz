using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //LOCAL Player data
    [SerializeField]private string _nickname;
    void Start()
    {
        var count = FindObjectsByType<PlayerData>(FindObjectsSortMode.None).Length;
        if (count > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetNickname(string nickName)
    {
        _nickname = nickName;
    }

    public string GetNickname()
    {
        if (string.IsNullOrWhiteSpace(_nickname))
        {
            _nickname = GetRandomNickName();
        }

        return _nickname;
    }

    public static string GetRandomNickName()
    {
        
        var rngPlayerNumber = Random.Range(0, 9999);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
