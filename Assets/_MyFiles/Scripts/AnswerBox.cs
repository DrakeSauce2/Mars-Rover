using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerBox : MonoBehaviour
{
    [Header("Input Text")]
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnInputValueChanged(string newValue)
    {
        string filteredValue = string.Empty;
        foreach (char c in newValue)
        {
            if (char.IsDigit(c) || c == '.')
            {
                filteredValue += c;
            }
        }

        inputField.text = filteredValue;
    }

    public void SetTextFromValue(float value)
    {
        inputField.text = value.ToString();
    }

    public void ClearValue()
    {
        inputField.text = string.Empty;
    }

    public float ReadValue()
    {
        if (inputField.text == "")
        {
            Debug.LogError("Input Field Is Empty");
            return EmptyContainerValue();
        }

        float inputValue;
        if (float.TryParse(inputField.text, out inputValue))
        {
            return inputValue;
        }
        else
        {
            Debug.LogError("Float Parse Failed");
            return EmptyContainerValue();
        }
    }

    public static float EmptyContainerValue()
    {
        return -100000000000000;
    }

}
