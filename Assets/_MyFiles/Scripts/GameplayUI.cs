using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq.Expressions;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance;

    [Header("Input Field")]
    [SerializeField] private GameObject inputField;

    [Header("Triangle Text")]
    [SerializeField] private TextMeshProUGUI adjSideText;
    [SerializeField] private TextMeshProUGUI oppSideText;
    [SerializeField] private TextMeshProUGUI hypSideText;

    [Header("Answer Boxes")]
    [SerializeField] private AnswerBox rValueAB;
    [SerializeField] private AnswerBox thetaValueAB;
    [SerializeField] private Toggle toggleRadDeg;

    [Header("EndScreen")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private TextMeshProUGUI collisionsCountText;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        continueButton.onClick.AddListener(GameManager.Instance.ContinueToTargetScene);
    }

    public void SetFieldActive(bool state)
    {
        inputField.SetActive(state);
    }

    public void SetEndScreenActive(bool state)
    {
        endScreen.SetActive(state);

        moveCountText.text = "Total Moves: " + GameManager.Instance.GetMoveCount().ToString();
        collisionsCountText.text = "Collisions: " + GameManager.Instance.GetCollisionsCount().ToString();
    }

    public bool IsAnswerInDegrees()
    {
        return toggleRadDeg.isOn;
    }

    public void SetTriangleSideLengths(float adjSideLength, float oppSideLength, Vector3 adjWorldPos, Vector3 oppWorldPos, Vector3 hypWorldPos)
    {
        adjSideText.transform.position = Camera.main.WorldToScreenPoint(adjWorldPos);
        oppSideText.transform.position = Camera.main.WorldToScreenPoint(oppWorldPos);
        hypSideText.transform.position = Camera.main.WorldToScreenPoint(hypWorldPos);

        adjSideText.text = adjSideLength.ToString();
        oppSideText.text = oppSideLength.ToString();

        hypSideText.text = "r = √(" + adjSideLength + "² + " + oppSideLength + "²" + ")";
    }

    public void SetHypTextZAngle(float angle)
    {
        hypSideText.transform.localEulerAngles = new Vector3(hypSideText.transform.localEulerAngles.x, hypSideText.transform.localEulerAngles.y, angle);
    }

    public void SetLineLength(float lineLength, Vector3 midPoint)
    {
        adjSideText.transform.position = Camera.main.WorldToScreenPoint(midPoint);

        adjSideText.text = lineLength.ToString();
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

        float tolerance = 0.01f;

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

    public void AutoSolve()
    {
        rValueAB.SetTextFromValue(ShapeGenerator.Instance.GetTargetDistance());
        thetaValueAB.SetTextFromValue(ShapeGenerator.Instance.GetTargetAngle());
    }

}
