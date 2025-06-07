using UnityEngine;
using UnityEngine.UIElements;

public class ScreenManager : MonoBehaviour
{
    #region Public Variables
    [SerializeField] private GameObject currentLayout;

    [Header("Menu Layouts")]
    public GameObject initialMenu;
    public GameObject codeMenu;
    public GameObject loginMenu;
    public GameObject waitingMenu;
    public GameObject lobbyMenu;

    #endregion


    #region References
    private UIDocument _currentDocument;
    #endregion


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    #region Singleton
    public static ScreenManager Instance;
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
    }
    #endregion
}
