using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    IA_Player playerInput;

    Player owningPlayer;

    [SerializeField] Transform cursorTransform;
    [SerializeField] private LineRenderer lineRenderer;

    Vector3 targetLocation;

    private bool hasStartedSelection = false;

    private void Awake()
    {
        playerInput = new IA_Player();

        playerInput.Enable();

        playerInput.Main.SelectLocation.performed += SelectLocation;
        playerInput.Main.Deselect.performed += DeselectLocation;
        playerInput.Main.ConfirmAnswer.performed += ConfirmAnswer;
        playerInput.Main.Debug_Solve.performed += AutoSolve;
    }

    private void AutoSolve(InputAction.CallbackContext context)
    {
        if (!hasStartedSelection) return;

        GameplayUI.Instance.AutoSolve();
    }

    private void ConfirmAnswer(InputAction.CallbackContext context)
    {
        if (hasStartedSelection == false) return;

        if(GameplayUI.Instance.IsAnswerCorrect())
        {
            ExecuteSelection();

            GameManager.Instance.IncrementMoveCount();

            DeselectLocation(context);
        }
    }

    private void SelectLocation(InputAction.CallbackContext context)
    {
        if (hasStartedSelection) return;

        Vector3 targetLocation = UpdateMousePosition();
        if (IsStraightLine(targetLocation))
        {
            ShapeGenerator.Instance.SetLine(owningPlayer.transform.position, targetLocation);
        }
        else
        {
            ShapeGenerator.Instance.GenerateTriangleFromLine(owningPlayer.transform.position, targetLocation);
        }

        GameplayUI.Instance.SetFieldActive(true);

        hasStartedSelection = true;
    }

    

    private bool IsStraightLine(Vector3 targetPosition)
    {
        if (Mathf.Abs(targetPosition.y - owningPlayer.transform.position.y) <= 0.001f)
        {
            return true;
        }

        if (Mathf.Abs(targetPosition.x - owningPlayer.transform.position.x) <= 0.001f)
        {
            return true;
        }

        return false;
    }

    private void DeselectLocation(InputAction.CallbackContext context)
    {
        hasStartedSelection = false;

        ShapeGenerator.Instance.ClearMesh();
        GameplayUI.Instance.ClearText();

        GameplayUI.Instance.SetFieldActive(false);
    }

    private void ExecuteSelection()
    {
        if (owningPlayer.IsMoving()) return;

        if (owningPlayer.transform.position == targetLocation) return;

        owningPlayer.PlayerMoveTo(cursorTransform.position);
    }

    public void Init(Player owner)
    {
        owningPlayer = owner;
    }

    public void DisableController()
    {
        playerInput.Disable();
    }

    public void EnableController()
    {
        playerInput.Enable();
    }

    private void Update()
    {
        if (owningPlayer == null) return;
        if (owningPlayer.IsMoving()) return;
        if (hasStartedSelection) return;

        targetLocation = UpdateMousePosition();
    }

    private Vector2 UpdateMousePosition()
    {
        Vector2 mousePos = playerInput.Main.CursorPosition.ReadValue<Vector2>();

        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 worldPosition = new Vector2(Mathf.RoundToInt(cursorWorldPosition.x), Mathf.RoundToInt(cursorWorldPosition.y));

        cursorTransform.position = worldPosition;

        SetLineRendererPosition(worldPosition);

        return worldPosition;
    }

    private void SetLineRendererPosition(Vector3 endPoint)
    {
        if (owningPlayer)
        {
            lineRenderer.SetPosition(0, owningPlayer.transform.position);
            lineRenderer.SetPosition(1, endPoint);
        }
    }

}
