using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] Vector2 _launchForce = Vector2.zero;

    bool canLaunch = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canLaunch)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();
            if(playerRigidbody != null)
            {
                playerRigidbody.AddForce(_launchForce, ForceMode2D.Impulse);
            }

            Debug.Log("Push player");

            canLaunch = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canLaunch = true;
        }
    }
}
