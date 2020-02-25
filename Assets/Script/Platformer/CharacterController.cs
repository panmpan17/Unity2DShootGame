using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SmartBoxCollider)), RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    private new SmartBoxCollider collider;
    private new Rigidbody2D rigidbody;

    [SerializeField]
    private float movingAcceleration, movingDrag, movingMaxSpeed;

    [SerializeField]
    private float jumpingForce, jumpTime;
    private float jumpTimeCountDown;
    
    private void Awake() {
        collider = GetComponent<SmartBoxCollider>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector2 velocity = rigidbody.velocity;
        if (Keyboard.current.dKey.isPressed) {
            velocity.x = Mathf.MoveTowards(velocity.x, movingMaxSpeed, movingAcceleration * Time.deltaTime);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, -movingMaxSpeed, movingAcceleration * Time.deltaTime);
        }

        if (velocity.x > 0) velocity.x = Mathf.MoveTowards(velocity.x, 0, movingDrag * Time.deltaTime);

        rigidbody.velocity = velocity;
    }
}
