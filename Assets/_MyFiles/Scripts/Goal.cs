using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player)
            {
                player.DisablePlayer();
                GameplayUI.Instance.SetEndScreenActive(true);

                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
    }
}
