using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandlerUI : NetworkBehaviour
{

    public UIDocument document;
    public VisualTreeAsset questionDocument;
    public EventHandler OnStateChanged;

    void Start()
    {
        GameManager.Instance.CurrentGameState.OnValueChanged += ChangeStateUI;
    }

    public void ChangeStateUI(GameState previousState, GameState newState)
    { 
        switch (newState)
        {
            case GameState.DisplayingQuestion:
                Event_PucQuiz.scene_actualy = "Quiz"; 
                Event_PucQuiz.layout_actualy = "Quiz"; 
                ShowQuestion();
                break;
        }
    }

    private void ShowQuestion()
    {
        DEV.Instance.DevPrint("Showing Questions");
        Debug.Log(Event_PucQuiz.scene_actualy);

        document = FindAnyObjectByType<UIDocument>();
        document.visualTreeAsset = questionDocument;

        Debug.Log(document.visualTreeAsset.name);
        var textContainer = document.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = document.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = document.rootVisualElement.Q<VisualElement>("Container-Resposta1");

        if (textContainer == null)
        {
            Debug.LogError("Cant find question text container");
        }
        textContainer.AddToClassList("QuestionTextStart");
        answersContainer.AddToClassList("ScaleUpStart");
        timerContainer.AddToClassList("TimerStart");
 
        //textContainer.RemoveFromClassList("QuestionTextStart");
        //answersContainer.RemoveFromClassList("ScaleUpStart"); 
        //timerContainer.RemoveFromClassList("TimerStart"); 
    }
}
