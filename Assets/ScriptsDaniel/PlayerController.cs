using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedMovement;

    [SerializeField] private Vector2 address;

    private Rigidbody2D rbd2;

    void Start()
    {
        rbd2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        address = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate() {
        rbd2.MovePosition(rbd2.position + address * speedMovement *Time.fixedDeltaTime);
    }
}
