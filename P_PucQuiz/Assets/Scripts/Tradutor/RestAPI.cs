using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Tradutor
{
    public class RestAPI:MonoBehaviour
    {
        private static string _URL = "http://graspserver-dev.eba-n3incx6t.us-east-1.elasticbeanstalk.com/api/";
        public static bool login = false;

        private string URL = "http://graspserver-dev.eba-n3incx6t.us-east-1.elasticbeanstalk.com/api/";
        
        public int index;

        public string typeText = "name";
        public TMPro.TextMeshProUGUI quizTitle;

        public bool getData = false;
        void Start()
        {
            
        }

        void Update()
        {
            if (getData)
            {
                getData = false;
                StartCoroutine(GetData());
            }
        }
        
        
        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator GetData()
        {
            UnityWebRequest request = UnityWebRequest.Get(URL + "users");
            
                yield return request.SendWebRequest();
 

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(request.result);
                }
                else
                {
                    string json = request.downloadHandler.text;
                    SimpleJSON.JSONNode userInfo = SimpleJSON.JSON.Parse(json);

                    quizTitle.text = userInfo[index][typeText];
                    /*
                    Quiz q = new Quiz();
                    q.title=quizInfo[index]["title"];
                    */
                }
        }

        public static IEnumerator Login(string email, string senha)
        {
            usuario login_usuario = new usuario();
            login_usuario.email = email;
            login_usuario.password = senha;
            string login_json = JsonUtility.ToJson(login_usuario);

            using (UnityWebRequest request = UnityWebRequest.Post(_URL+"users/login",login_json,"application/json"))
            {

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.result);
                }
                if(request.responseCode == 200)
                {
                    Debug.Log("Login e senha corretos.");
                    Event_PucQuiz.login = "true";
                }
                else
                {
                    Debug.Log("Code = "+request.responseCode);
                    Event_PucQuiz.login = "false";
                }
            }
        }
    }
}