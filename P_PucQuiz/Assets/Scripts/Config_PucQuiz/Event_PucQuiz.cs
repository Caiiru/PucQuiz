using UnityEngine;
using UnityEngine.SceneManagement;
 

public static class Event_PucQuiz
{
    //Mods Events
    public static bool change = true;

    //Menu Events
    public static string login = "";
    public static string layout_actualy;
    public static string scene_actualy;

    //Question Events
    public static string question_event = "";
    public static string question_result = "";
    public static bool start_layout = false;
    public static bool question_lock = false;
    public static bool question_end_list = false;
    public static bool question_next = false;
    public static float points = 0;


    //Generic Events
    public static bool pause = false;

    public static void Change_Scene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
