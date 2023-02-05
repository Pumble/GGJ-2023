using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Lo que debe seguir

    public float smoothSpeed = 0.125f; // Es como el tiempo de espera, que tardara en empezar a seguir al jugador

    public Vector3 offset; //Posicion de la camara, pero en la que modificamos el z, para poder ver
    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
