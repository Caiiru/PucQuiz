using System;
using Tradutor;
using UnityEngine;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] menu;
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
    private void ClickStart(ClickEvent evt)
    {
        Debug.Log("Start = Sucesso");

        //menu[0].getValue2().SetActive(false);
        //menu[1].getValue2().SetActive(true);
        ChangeMenu("Login");
    }

    private void ClickLogin(ClickEvent click)
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

    private void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("Não foi atribuido um valor ao novo menu buscado."); return; }

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

            }
        }

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

    #endregion

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