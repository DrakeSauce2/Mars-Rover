using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Player;

public class Player : MonoBehaviour
{

    [SerializeField] private GameplayUI gui;

    private PlayerController playerController;

    public GameplayUI GetGUI() { return gui; }

    [SerializeField] private float turnSpeed = 100f;

    private bool bIsMoving = false;
    public bool IsMoving() { return bIsMoving; }

    Vector3 preMoveLocation;

    Coroutine MoveToCoroutine;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController)
        {
            playerController.Init(this);
        }
    }

    public void PlayerMoveTo(Vector3 targetPosition)
    {
        MoveToCoroutine = StartCoroutine(PlayerMoveToCoroutine(targetPosition));
    }

    public IEnumerator PlayerMoveToCoroutine(Vector3 targetPosition)
    {
        bIsMoving = true;

        preMoveLocation = transform.position;

        // Get the direction to the target position
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Calculate the target rotation angle
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate towards the target position
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle)) > 0.01f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle);

        // Move towards the target position
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        bIsMoving = false;
    }

    public void ResetToPreviousLocation()
    {
        StopCoroutine(MoveToCoroutine);

        bIsMoving = false;

        transform.position = preMoveLocation;
    }

    public void DisablePlayer()
    {
        if (bIsMoving)
        {
            StopCoroutine(MoveToCoroutine);
        }

        playerController.DisableController();
    }

}
