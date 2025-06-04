using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [Tooltip("Max 5 letters")]
    public string text;

    public string[] letters;
    private TextMeshProUGUI[] tmLetters;
    public List<Color> colors;

    public float colorOffset = 0.2f;
    public float letterSize = 5f;
    private float _brotherSize;
    void Start()
    {
        letters = new string[5];
        tmLetters = new TextMeshProUGUI[5];
       
        for(int i = 0; i<5; i++)
        {
            //letters[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
            letters[i] = text[i].ToString();
            tmLetters[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            tmLetters[i].text = letters[i];
            transform.GetChild(i).GetComponent<letterScript>().SetIndex(i);
            transform.GetChild(i).GetComponent<letterScript>().SetFinalSize(letterSize);
            _brotherSize = letterSize*0.8f;
        }
        ResetColors();
    }

    public void ChangeColor(int Index)
    {
        ResetColors();

        switch (Index)
        {
            case 0:
                tmLetters[Index].color = colors[0];
                tmLetters[Index+1].color = colors[0]*colorOffset;

                tmLetters[Index].transform.DOScale(new Vector3(letterSize,letterSize,letterSize), 0.5f);
                tmLetters[Index + 1].transform.DOScale(_brotherSize, 0.5f);
                break;

            case 1:
                tmLetters[Index - 1].color = colors[Index] * colorOffset;
                tmLetters[Index].color = colors[Index];
                tmLetters[Index + 1].color = colors[Index] * colorOffset;


                tmLetters[Index].transform.DOScale(new Vector3(letterSize, letterSize, letterSize), 0.5f);
                tmLetters[Index + 1].transform.DOScale(_brotherSize, 0.5f);
                tmLetters[Index -1].transform.DOScale(_brotherSize, 0.5f);
                break;

            case 2:
                tmLetters[Index - 1].color = colors[Index] * colorOffset;
                tmLetters[Index].color = colors[Index];
                tmLetters[Index + 1].color = colors[Index] * colorOffset;

                tmLetters[Index].transform.DOScale(new Vector3(letterSize, letterSize, letterSize), 0.5f);
                tmLetters[Index + 1].transform.DOScale(_brotherSize, 0.5f);
                tmLetters[Index - 1].transform.DOScale(_brotherSize, 0.5f);
                break;

            case 3:
                tmLetters[Index - 1].color = colors[Index] * colorOffset;
                tmLetters[Index].color = colors[Index];
                tmLetters[Index + 1].color = colors[Index] * colorOffset;

                tmLetters[Index].transform.DOScale(new Vector3(letterSize, letterSize, letterSize), 0.5f);
                tmLetters[Index + 1].transform.DOScale(_brotherSize, 0.5f);
                tmLetters[Index - 1].transform.DOScale(_brotherSize, 0.5f);
                break;

            case 4:
                tmLetters[Index - 1].color = colors[Index] * colorOffset;
                tmLetters[Index].color = colors[Index];

                tmLetters[Index].transform.DOScale(new Vector3(letterSize, letterSize, letterSize), 0.5f); 
                tmLetters[Index - 1].transform.DOScale(_brotherSize, 0.5f);
                break;

            case 5:
                tmLetters[Index - 1].color = colors[Index] * colorOffset;
                tmLetters[Index].color = colors[Index];

                tmLetters[Index].transform.DOScale(new Vector3(letterSize, letterSize, letterSize), 0.5f);
                tmLetters[Index - 1].transform.DOScale(_brotherSize, 0.5f);
                break;
        }

    }
    public void ResetColors()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            tmLetters[i].color = Color.white;
            tmLetters[i].transform.DOScale(1f, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
