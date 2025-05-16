using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
