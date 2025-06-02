using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandlerUI : NetworkBehaviour
{

    UIDocument document; 
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
                StartCoroutine(ShowQuestion(0.5f));
                break;
        }
    }

    IEnumerator ShowQuestion(float delayTime)
    {
        //DEV.Instance.DevPrint("Showing Questions");
        yield return new WaitForSeconds(delayTime);
        document = FindAnyObjectByType<UIDocument>();
        //document.visualTreeAsset = questionDocument;

        var textContainer = document.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = document.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = document.rootVisualElement.Q<VisualElement>("GridContainer");

        if (textContainer == null)
        {
            Debug.LogError($"Cant find question text container, current Document: {document.name}");
        }
        /*
        textContainer.AddToClassList("QuestionTextStart");
        answersContainer.AddToClassList("ScaleUpStart");
        timerContainer.AddToClassList("TimerStart");*/

        textContainer.RemoveFromClassList("QuestionText_Anim");
        answersContainer.RemoveFromClassList("Buttons_Anim"); 
        timerContainer.RemoveFromClassList("TimerText_Anim"); 
    }
 
}
