using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SmartBoxCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private int horizontalDetectPointCount = 2, verticalDetectPointCount = 2;
    [SerializeField]
    private float leftRayDis = 0.02f, rightRayDis = 0.02f, upRayDis = 0.02f, downRayDis = 0.02f;

    private ContactPoints[] leftContactPoints, rightContactPoints, upContactPoints, downContactPoints;

    private new BoxCollider2D collider;

    private bool IsGrounded = false;

    private void CalculateContactPoints() {
        upContactPoints = new ContactPoints[horizontalDetectPointCount];
        downContactPoints = new ContactPoints[horizontalDetectPointCount];
        leftContactPoints = new ContactPoints[verticalDetectPointCount];
        rightContactPoints = new ContactPoints[verticalDetectPointCount];

        float xMin = collider.offset.x - (collider.size.x / 2);
        float xMax = collider.offset.x + (collider.size.x / 2);
        float yMin = collider.offset.y - (collider.size.y / 2);
        float yMax = collider.offset.y + (collider.size.y / 2);

        for (int i = 0; i < horizontalDetectPointCount; i++) {
            upContactPoints[i] = new ContactPoints(
                new Vector2(Mathf.Lerp(xMin, xMax, i / (horizontalDetectPointCount - 1f)), yMax),
                new Vector2(0, upRayDis)
            );

            downContactPoints[i] = new ContactPoints(
                new Vector2(Mathf.Lerp(xMin, xMax, i / (horizontalDetectPointCount - 1f)), yMin),
                new Vector2(0, -downRayDis)
            );
        }

        for (int i = 0; i < verticalDetectPointCount; i++)
        {
            leftContactPoints[i] = new ContactPoints(
                new Vector2(xMin, Mathf.Lerp(yMin, yMax, i / (verticalDetectPointCount - 1f))),
                new Vector2(-leftRayDis, 0)
            );

            rightContactPoints[i] = new ContactPoints(
                new Vector2(xMax, Mathf.Lerp(yMin, yMax, i / (verticalDetectPointCount - 1f))),
                new Vector2(rightRayDis, 0)
            );
        }
    }

    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        CalculateContactPoints();
    }

    private void FixedUpdate() {
        for (int i = 0; i < downContactPoints.Length; i++) {
            downContactPoints[i].Raycast(transform.position, groundLayer);
            Debug.DrawRay(
                transform.position + (Vector3)downContactPoints[i].Position,
                downContactPoints[i].Dis,
                downContactPoints[i].LastCastHit? Color.red: Color.green,
                0.05f
            );
        }
    }

    // private void OnDrawGizmosSelected() {
    //     if (collider == null) collider = GetComponent<BoxCollider2D>();

    //     ContactPoints[] points = GetAllContactPoints();
    //     Gizmos.color = Color.red;
    //     Vector2 position = transform.position;
    //     for (int i = 0; i < points.Length; i++) {
    //         Gizmos.DrawLine(position + points[i].Position, position + points[i].Position + points[i].Dis);
    //     }
    // }

    private struct ContactPoints {
        public Vector2 Position;
        public Vector2 Dis;
        public bool LastCastHit;

        public ContactPoints(Vector2 position, Vector2 dis) {
            Position = position;
            Dis = dis;
            LastCastHit = false;
        }

        public RaycastHit2D Raycast(Vector2 offset, LayerMask layer) {
            RaycastHit2D hit = Physics2D.Raycast(offset + Position, Dis.normalized, Dis.magnitude, layer);
            LastCastHit = hit.collider != null;
            return hit;
        }
    }
}
