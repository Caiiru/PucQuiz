using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public static class Event_PucQuiz
{
    # region @ Mods Events @

    public static bool change = true;

    #endregion

    # region @ Menu Events @

    [Tooltip("Resultado do Login.")]
    public static string login = "";
    [Tooltip("Layout atual da scene.")]
    public static string layout_actualy;
    [Tooltip("Scene ou grupo de layouts do momento.")]
    public static string scene_actualy;

    #endregion
    
    # region @ Question Events @

    public static string question_event = "";
    [Tooltip("'' = sem resultado | 'win' = acertou | 'lose' = errou.")]
    public static string question_result = "";
    public static bool start_layout = false;
    public static bool question_lock = false;
    public static bool question_end_list = false;
    [Tooltip("Verifica se pode trocar de pergunta.")]
    public static bool question_next = false;
    [Tooltip("Quantos pontos o player possui.")]
    public static float points = 0;

    #endregion

    #region @ Multiplayer Events @

    public static string player_name;
    public static QuizPlayer player;
    public static Dictionary<int, QuizPlayer> players;

    #endregion

    #region @ Generic Events @

    public static bool pause = false;

    #endregion

    public static void Change_Scene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
