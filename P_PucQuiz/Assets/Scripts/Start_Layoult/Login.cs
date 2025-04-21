using Mono.Cecil.Cil;
using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class Login : MonoBehaviour
{
    public GameObject login, codeORcreate;

    public string layout_actualy;

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
            Debug.Log("Login = Sucesso");
            layout_actualy = "createORcode";
            codeORcreate.SetActive(true);
            login.SetActive(false);
        }
        else
        {
            if (layout_actualy == "createORcode" && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Create Game = Sucesso");
                Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
            }
        }

        if (layout_actualy == "createORcode" && (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)))
        {
            Debug.Log("Enter Game = Sucesso");
            Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
        }
    }


}
