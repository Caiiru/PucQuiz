using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Tradutor
{
    public class RestAPI:MonoBehaviour
    {
        private string URL = "http://graspserver-dev.eba-n3incx6t.us-east-1.elasticbeanstalk.com/api/";

        public int index;

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
            using (UnityWebRequest request = UnityWebRequest.Get(URL))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(request.result);
                }
                else
                {
                    string json = request.downloadHandler.text;
                    SimpleJSON.JSONNode quizInfo = SimpleJSON.JSON.Parse(json);

                    quizTitle.text = quizInfo[index]["title"];
                    /*
                    Quiz q = new Quiz();
                    q.title=quizInfo[index]["title"];
                    */
                }
            }
        }

        IEnumerator Login(Dictionary<string,string> login)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL + "/users/login", login))
            {
                yield return request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.result);
                }
                else
                {

                }
            }
        }
    }
}