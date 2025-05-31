using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class End
{
    Config_PucQuiz config;
    public Dictionary<int,QuizPlayer> players => Event_PucQuiz.players;
    private LayoutManager manager;
    public UIDocument doc;
    public int length => players.Count;

    public Timer time;

    [Header("Layouts")]
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] layout;

    public void Awake(GameObject obj)
    {
        manager = obj.GetComponent<LayoutManager>();
        doc = obj.GetComponent<UIDocument>();
    }

    public void Start(GameObject obj)
    {
        time.Reset();
    }

    public void Update(GameObject obj)
    {
        if(!time.End())
        {
            time.Run();
        }
        else
        {
            Return(obj);
        }
    }

    private void Return(GameObject obj)
    {
        manager.end_start = true;
        Modos quiz = manager.quiz;

        if (Event_PucQuiz.layout_actualy == "Rank")
        {
            manager.ChangeMenu("Quiz", quiz.attributes[quiz.question_actualy_index].question_type.ToString());
        }
        else
        {
            manager.ChangeMenu("Start", "Start");
        }
    }

    private void SetLayout()
    {
        for (int i = 0; i < layout.Length; i++)
        {
            try
            {

                if (layout[i].getValue1() == Event_PucQuiz.layout_actualy)
                {
                    switch (layout[i].getValue1())
                    {
                        case "Rank":

                            Debug.Log("Rank Set Start");

                            Debug.Log("Rank % = Start");
                            int point_1 = 100;
                            int point_2 = 78;
                            int point_3 = 50;
                            int point_4 = 20;

                            int porcent_1 = 1;
                            int porcent_2 = ((100 * point_2) / point_1) / 100;
                            int porcent_3 = ((100 * point_3) / point_1) / 100;
                            int porcent_4 = ((100 * point_4) / point_1) / 100;

                            doc.rootVisualElement.Q("Progress1").style.width = new Length(porcent_1 * 100, LengthUnit.Percent);
                            doc.rootVisualElement.Q("Progress2").style.width = new Length(porcent_2 * 100, LengthUnit.Percent);
                            doc.rootVisualElement.Q("Progress3").style.width = new Length(porcent_3 * 100, LengthUnit.Percent);
                            doc.rootVisualElement.Q("Progress4").style.width = new Length(porcent_4 * 100, LengthUnit.Percent);

                            Debug.Log("Rank Name = Start");
                            doc.rootVisualElement.Q<Label>("PlayerName1").name = "Player1";
                            doc.rootVisualElement.Q<Label>("PlayerName2").name = "Player2";
                            doc.rootVisualElement.Q<Label>("PlayerName3").name = "Player3";
                            doc.rootVisualElement.Q<Label>("PlayerName4").name = "Player4";

                            Debug.Log("Rank Set End");

                            break;
                        case "End":

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

        Event_PucQuiz.scene_actualy = "End";
        Event_PucQuiz.layout_actualy = menu_new;

        GameObject background = null;

        try
        {
            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i].getValue1() == menu_new)
                {
                    background = layout[i].getValue2();
                    doc.visualTreeAsset = layout[i].getValue3();
                }

            }

            if (!background.activeSelf && background != null) { background.SetActive(true); }

            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i].getValue1() != menu_new && layout[i].getValue2() != background)
                {
                    layout[i].getValue2().SetActive(false);
                }
            }
        }
        catch (Exception error)
        {
            Debug.Log(error);
        }

        SetLayout();
    }
}
