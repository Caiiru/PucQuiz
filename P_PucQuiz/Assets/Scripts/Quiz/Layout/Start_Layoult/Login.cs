using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Login
{
    [Header("Basic Variables")]
    public UIDocument doc;
    public LayoutManager manager;

    [Header("Login Variables")]
    [SerializeField] private bool test = false;
    public string email;
    public string senha;

    [Header("Layouts")]
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] menu;




    public void Awake()
    {
        Event_PucQuiz.layout_actualy = "Start"; 
    }

    public void Start()
    {
        ChangeMenu("Start");
        DeveloperConsole.Console.AddCommand("loginquiz", LoginStartQuiz);

    }

    public void Update()
    {
        /* A cada frame essa verificação é feita
        switch (Event_PucQuiz.login.ToLower())
        {
            case "":
                break;
            case "true":
                Debug.Log("Login = Sucesso");
                //RestAPI.login = false;
                RestAPI.login = false;

                Event_PucQuiz.player = new QuizPlayer();
                Event_PucQuiz.player.SetPlayerName(email);
                Event_PucQuiz.player.SetPlayerPoints(0);

                if(Event_PucQuiz.player != null) { Debug.Log("Player exist."); }

                Event_PucQuiz.login = "";
                Event_PucQuiz.scene_actualy = "Quiz";
                break;
            case "false":
                Debug.Log("Login = Erro");
                //RestAPI.login = false;
                Event_PucQuiz.login = "";
                break;
            default:
                Debug.LogError("O evento login possui um valor indevido.");
                break;
        }
        */
    }

    #region # Click Events #
    private void ClickStart(ClickEvent evt) //Bot�o que transita da tela inicial para a tela de login.
    {
        Debug.Log("Start = Sucesso");

        //menu[0].getValue2().SetActive(false);
        //menu[1].getValue2().SetActive(true);
        ChangeMenu("Codigo");
    }

    private void ClickLogin(ClickEvent click) //Bot�o que confere se o email e senha est�o corretos e permite logar caso estejam.
    {
        /*
        try
        {
            if (RestAPI.login == false && Event_PucQuiz.login == "")
            {
                Debug.Log("Email = " + doc.rootVisualElement.Q<TextField>("Email").text);
                Debug.Log("Senha = " + doc.rootVisualElement.Q<TextField>("Senha").text);

                RestAPI.login = true;
                if (!test)
                {
                    manager.StartCoroutine(RestAPI.Login(doc.rootVisualElement.Q<TextField>("Email").text,
                                                         doc.rootVisualElement.Q<TextField>("Senha").text));
                }
                else
                {
                    Event_PucQuiz.login = "true";
                }
            }
            else
            {
                Debug.Log("E isso");
            }
        }
        catch
        {
            Debug.Log("Erro ao solicitar o login");
        }
        */
    }
    private void ClickStartQuiz(ClickEvent clickEvent)
    {
        //GameManager.Instance.StartQuiz_Rpc();
        GameManager.Instance.StartQuizRpc();


    }

    void LoginStartQuiz(string[] args)
    {
        Debug.Log("LoginStartQuiz");
        DeveloperConsole.Console.Print("Login Start Quiz");
        Event_PucQuiz.scene_actualy = "Quiz";
    }

    private void ClickRegister(ClickEvent click) //Bot�o que transita da tela de login para a tela de registro.
    {
        ChangeMenu("Registro1");
    }

    private void ClickProximo(ClickEvent click) //Bot�o que verifica se as senhas batem e passa para a tela de nickname.
    {
        //Fazer a logica de verificar se as senhas s�o iguais.

        ChangeMenu("Registro2");
    }

    private void ClickFinalizar(ClickEvent click) //Bot�o que fnaliza o login do usuario e retorna para a tela de login.
    {
        //Fazer a logica de verificar se o nome de usuario j� n�o esta em uso.

        ChangeMenu("Login");
    }

    private void ClickVoltar(ClickEvent click) //Bot�o que retorna para a tela anterior.
    {
        switch (Event_PucQuiz.layout_actualy)
        {
            case "Registro1":
                ChangeMenu("Login");
                break;
            case "Registro2":
                ChangeMenu("Registro1");
                break;
        }
    }

    private void ClickCodigo(ClickEvent click) //Bot�o que passa da tela de login para a tela de codigo ou a tela de guest.
    {
        switch (Event_PucQuiz.layout_actualy)
        {
            case "Login":
                ChangeMenu("Codigo1");
                break;
            case "Logado":
                ChangeMenu("Codigo2");
                break;
        }
    }

    private async void ClickEntrar(ClickEvent click) //Bot�o que verifica o codigo e vai para a tela de espera caso seja encontrado.
    {
        string code = doc.rootVisualElement.Q<TextField>("Code").value.ToUpper();
        string userName = doc.rootVisualElement.Q<TextField>("Name").value;
 
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogError("User name is null or empty");
            return;
        }

        //GameManager.Instance.onJoiningGame?.Invoke(this,null);
        ChangeMenu("Conectando");
        if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
        {
            var host = await GameManager.Instance.StartHostWithRelay(30, userName);

            if (host != null)
            {
                ChangeMenu("CriarPartida");
            }
        }
        else
        {
            if (await GameManager.Instance.StartClientWithRelay(code, userName))
            {
                ChangeMenu("CriarPartida");
                GameManager.Instance.onPlayerJoined?.Invoke(this, null);
            }
        } 

    }

    private void ClickCriarPartida(ClickEvent click) //Bot�o que vai da tela de login para a de criar partida ou cria a partida.
    {
        //Adicionar logica que verifica se o usuario selecionou um quiz existente na sua conta.
        ChangeMenu("Criando");
    }

    private void ClickCriarQuiz(ClickEvent click) //Bot�o que vai para a tela de criar quiz para vc desenvolver o seu quiz.
    {
        Debug.Log("Esta mecanica inda nao foi implementada.");
        //if(false)
        //ChangeMenu("Criando");
    }
    #endregion

    private void SetButtons()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            try
            {

                if (menu[i].getValue1() == Event_PucQuiz.layout_actualy)
                {
                    switch (menu[i].getValue1())
                    {
                        case "Start":
                            doc.rootVisualElement.Q<Button>("Play").RegisterCallback<ClickEvent>(ClickStart);
                            break;
                        case "Login":
                            doc.rootVisualElement.Q<Button>("Login").RegisterCallback<ClickEvent>(ClickLogin);
                            break;
                        case "CreateOrCode":
                            break;
                        case "Codigo":
                            doc.rootVisualElement.Q<Button>("Entrar").RegisterCallback<ClickEvent>(ClickEntrar);
                            break;
                        case "CriarPartida":
                            doc.rootVisualElement.Q<Label>("CodeText").text = $"Codigo: {GameManager.Instance.JoinCode}";
                            //QuizLobby.Instance.onUpdateLobbyUI?.Invoke(this, null);
                            CheckHostStatus();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }

    public void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("N�o foi atribuido um valor ao novo menu buscado."); return; }

        Event_PucQuiz.scene_actualy = "Menu";
        Event_PucQuiz.layout_actualy = menu_new;

        GameObject background = null;

        try
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].getValue1() == menu_new)
                {
                    background = menu[i].getValue2();
                    doc.visualTreeAsset = menu[i].getValue3();
                }

            }

            if (!background.activeSelf && background != null) { background.SetActive(true); }

            for (int i = 0; i < menu.Length; i++)
            {
                if (menu[i].getValue1() != menu_new && menu[i].getValue2() != background)
                {
                    menu[i].getValue2().SetActive(false);
                }
            }
        }
        catch (Exception error)
        {
            // Debug.Log(error);
        }

        SetButtons();
    }


    private void JoiningLobby(object sender, EventArgs e)
    {
        //Verifica se esta se juntando, tentando conectar e muda para uma tela de aguardo. 
        ChangeMenu("Conectando");
    }
 

    void CheckHostStatus()
    {
        //Verifica se o jogador é host ou não para deixar ativo o boão de iniciar quiz/partida
        var _startButton = doc.rootVisualElement.Q<Button>("Iniciar");
        if (!GameManager.Instance.IsHost)
        {
            //NOT HOST:
            _startButton.parent.Remove(_startButton);

        }
        _startButton.RegisterCallback<ClickEvent>(ClickStartQuiz);


    }

    public void StartQuiz()
    {

        Event_PucQuiz.scene_actualy = "Quiz";
    }


}

[System.Serializable]
public class usuario
{
    public string email;
    public string password;
}

[Serializable]
public class DictionaryThree<Tvalue1, Tvalue2, Tvalue3>
{
    [SerializeField] private Tvalue1 value1;
    [SerializeField] private Tvalue2 value2;
    [SerializeField] private Tvalue3 value3;

    /*
    public Type typeKey()
    {
        return key.GetType();
    }
    public Type type1()
    {
        return value1.GetType();
    }
    public Type type2()
    {
        return value2.GetType();
    }
    */

    public Tvalue1 getValue1()
    {
        return value1;
    }
    public Tvalue2 getValue2()
    {
        return value2;
    }

    public Tvalue3 getValue3()
    {
        return value3;
    }
}