using System;
using Tradutor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Login : MonoBehaviour
{
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] menu;
    public string email, senha;
    public UIDocument doc;

    public string layout_actualy;

    private void Awake()
    {
        Modos.get = null;
        layout_actualy = "Start";
    }

    void Start()
    {
        ChangeMenu("Start");
    }

    void Update()
    {
        switch(Event_PucQuiz.login)
        {
            case "":
                break;
            case "true":
                Debug.Log("Login = Sucesso");
                RestAPI.login = false;
                Event_PucQuiz.login = "";
                break;
            case "false":
                Debug.Log("Login = Erro");
                RestAPI.login = false;
                Event_PucQuiz.login = "";
                break;
            default:
                Debug.LogError("O evento login possui um valor indevido.");
                break;
        }
    }

    #region # Click Events #
    private void ClickStart(ClickEvent evt) //Bot�o que transita da tela inicial para a tela de login.
    {
        Debug.Log("Start = Sucesso");

        //menu[0].getValue2().SetActive(false);
        //menu[1].getValue2().SetActive(true);
        ChangeMenu("Login");
    }

    private void ClickLogin(ClickEvent click) //Bot�o que confere se o email e senha est�o corretos e permite logar caso estejam.
    {
        try
        {
            if (RestAPI.login == false)
            {
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
        switch(layout_actualy)
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
        switch(layout_actualy)
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

        if(layout_actualy == "Codigo1")
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

                if (menu[i].getValue1() == layout_actualy)
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

        layout_actualy = menu_new;

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
            }
        }

        SetButtons();
    }

    private void OnDisable()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            try
            {
                switch (menu[i].getValue1())
                {
                    case "Start":
                        doc.rootVisualElement.Q<Button>("Start").UnregisterCallback<ClickEvent>(ClickStart);
                        break;
                    case "Login":
                        doc.rootVisualElement.Q<Button>("Login").UnregisterCallback<ClickEvent>(ClickLogin);
                        break;
                    case "CreateOrCode":
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
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