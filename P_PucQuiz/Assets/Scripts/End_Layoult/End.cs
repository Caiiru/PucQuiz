using TMPro;
using UnityEngine;

public class End : MonoBehaviour
{
    //public UnityEngine.UI.Text points;
    public TextMeshProUGUI points;
    public static float local_points = 0;
    public float speed = 5;

    public Timer time;

    void Start()
    {
        if (Modos.get.question_actualy_index != Modos.get.attributes.Length)
        {
            //TODO -> FIX - An object reference is required for the non-static field, method, or property 'Modos.question_actualy_index'

            Modos.get.question_actualy_index++;
        }

        points.text = "Points = " + End.local_points;
        time.Reset();
    }

    void Update()
    {

        if (Event_PucQuiz.points > End.local_points)
        {
            End.local_points = End.local_points + (Time.deltaTime * speed);

            if (End.local_points > Event_PucQuiz.points)
            {
                End.local_points = Event_PucQuiz.points;
            }

            points.text = "Points = " + ((int)End.local_points);
        }
        else
        {
            time.Run();
        }

        if(time.End())
        {
            if (Modos.get.Final())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Start);
                }
            }
            else
            {
                Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
            }
        }
    }
}
