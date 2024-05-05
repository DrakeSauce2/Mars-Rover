using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq.Expressions;

public class GameplayUI : MonoBehaviour
{
    [Header("Input Field")]
    [SerializeField] private GameObject inputField;

    [Header("Triangle Text")]
    [SerializeField] private TextMeshProUGUI adjSideText;
    [SerializeField] private TextMeshProUGUI oppSideText;
    [SerializeField] private TextMeshProUGUI hypSideText;

    [Header("Answer Boxes")]
    [SerializeField] private AnswerBox rValueAB;
    [SerializeField] private AnswerBox thetaValueAB;

    public void SetFieldActive(bool state)
    {
        inputField.SetActive(state);
    }

    public void SetTriangleSideLengths(float adjSideLength, float oppSideLength, Vector3 adjWorldPos, Vector3 oppWorldPos, Vector3 hypWorldPos)
    {
        adjSideText.transform.position = Camera.main.WorldToScreenPoint(adjWorldPos);
        oppSideText.transform.position = Camera.main.WorldToScreenPoint(oppWorldPos);
        hypSideText.transform.position = Camera.main.WorldToScreenPoint(hypWorldPos);

        adjSideText.text = "" + adjSideLength;
        oppSideText.text = "" + oppSideLength;
        hypSideText.text = "r = √(" + adjSideLength + "² + " + oppSideLength + "²" + ")";
    }

    public void SetHypTextZAngle(float angle)
    {
        hypSideText.transform.localEulerAngles = new Vector3(hypSideText.transform.localEulerAngles.x, hypSideText.transform.localEulerAngles.y, angle);
    }

    public void ClearText()
    {
        adjSideText.text = "";
        oppSideText.text = "";
        hypSideText.text = "";

        rValueAB.ClearValue();
        thetaValueAB.ClearValue();
    }

    public bool IsAnswerCorrect()
    {
        float rValue = rValueAB.ReadValue();
        float thetaValue = thetaValueAB.ReadValue();

        if (!IsAnswerValid(rValue, thetaValue)) return false;

        float targetRValue = ShapeGenerator.Instance.GetTargetDistance();
        float targetThetaValue = ShapeGenerator.Instance.GetTargetAngle();

        float tolerance = 0.001f;

        if (Mathf.Abs(rValue - targetRValue) > tolerance)
        {
            Debug.Log("Incorrect R Value: " + rValue + " != " + targetRValue);
            return false;
        }

        if (Mathf.Abs(thetaValue - targetThetaValue) > tolerance)
        {
            Debug.Log("Incorrect Theta Value: " + thetaValue + " != " + targetThetaValue);
            return false;
        }

        return true;
    }

    private bool IsAnswerValid(float rValue, float thetaValue)
    {
        if (rValue == AnswerBox.EmptyContainerValue())
        {
            Debug.Log("Invalid Value: r");
            return false;
        }

        if (thetaValue == AnswerBox.EmptyContainerValue())
        {
            Debug.Log("Invalid Value: r");
            return false;
        }

        return true;
    }

}
