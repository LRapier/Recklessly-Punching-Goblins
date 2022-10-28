using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        if (PlayerController.me != null && !PlayerController.me.dead)
        {
            Camera.main.orthographicSize = 5;
            Vector3 targetPos = PlayerController.me.transform.position;
            targetPos.z = -10;
            transform.position = targetPos;
        }
        else
        {
            transform.position = new Vector3(0, 0, -10);
            Camera.main.orthographicSize = 15;
        }
    }
}
