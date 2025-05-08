using Mono.Cecil.Cil;
using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Login : MonoBehaviour
{
    public GameObject login, codeORcreate;

    

<<<<<<< Updated upstream
=======
    private void Awake()
    {
        Modos.get = null;
        Event_PucQuiz.layout_actualy = "Start";
    }

>>>>>>> Stashed changes
    void Start()
    {
        Modos.get = null;
        layout_actualy = "login";
        login.SetActive(true);
        codeORcreate.SetActive(false);
    }

    void Update()
    {

        if (layout_actualy == "login" && Input.GetKeyDown(KeyCode.Space))
        {
<<<<<<< Updated upstream
            Debug.Log("Login = Sucesso");
            layout_actualy = "createORcode";
            codeORcreate.SetActive(true);
            login.SetActive(false);
=======
            case "":
                break;
            case "true":
                Debug.Log("Login = Sucesso");
                RestAPI.login = false;
                Event_PucQuiz.login = "";
                ChangeMenu("Jogo");
                break;
            case "false":
                Debug.Log("Login = Erro");
                RestAPI.login = false;
                Event_PucQuiz.login = "";
                break;
            default:
                Debug.LogError("O evento login possui um valor indevido.");
                break;
>>>>>>> Stashed changes
        }
        else
        {
            if (layout_actualy == "createORcode" && Input.GetKeyDown(KeyCode.Space))
            {
<<<<<<< Updated upstream
                Debug.Log("Create Game = Sucesso");
                Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
=======
                Debug.Log("Email = " + doc.rootVisualElement.Q<TextField>("Email").text);
                Debug.Log("Senha = " + doc.rootVisualElement.Q<TextField>("Senha").text);

                RestAPI.login = true;
                StartCoroutine(RestAPI.Login(doc.rootVisualElement.Q<TextField>("Email").text,
                                             doc.rootVisualElement.Q<TextField>("Senha").text));
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
        switch(Event_PucQuiz.layout_actualy)
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
        switch(Event_PucQuiz.layout_actualy)
        {
            case "Login":
                ChangeMenu("Codigo1");
                break;
            case "Logado":
                ChangeMenu("Codigo2");
                break;
        }
    }

    private void ClickEntrar(ClickEvent click) //Bot�o que verifica o codigo e vai para a tela de espera caso seja encontrado.
    {
        //Adicionar logica que verifica se existe uma sala com esse codigo.

        if(Event_PucQuiz.layout_actualy == "Codigo1")
        {
            ChangeMenu("Conectando");
            return;
        }

        //Colocar logica que verifica se o usuario colocou algum nome.
        ChangeMenu("");
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
                    }
                }
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }
    private void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("N�o foi atribuido um valor ao novo menu buscado."); return; }

        Event_PucQuiz.layout_actualy = menu_new;

        for (int i = 0; i < menu.Length; i++)
        {
            try
            {
                if (menu[i].getValue1() == menu_new)
                {
                    menu[i].getValue2().SetActive(true);
                    doc.visualTreeAsset = menu[i].getValue3();
                }
                else
                {
                    menu[i].getValue2().SetActive(false);
                }
            }
            catch (Exception error)
            {
                Debug.LogError(error);
>>>>>>> Stashed changes
            }
        }

        if (layout_actualy == "createORcode" && (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)))
        {
            Debug.Log("Enter Game = Sucesso");
            Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
        }
    }


}
