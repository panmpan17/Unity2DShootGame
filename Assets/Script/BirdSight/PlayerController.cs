using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BirdSight {
    public class PlayerController : MonoBehaviour
    {
        static public Camera mainCamera;

        private static Vector2 DegreeToVector2(float degree)
        {
            float radian = Mathf.Deg2Rad * degree;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        PlayerInputMap inputActions;
        new Rigidbody2D rigidbody2D;

        [SerializeField]
        int health;
        int fullHealth;

        [SerializeField]
        float speedMultiplier = 1;
        Vector2 moveVelocity;

        [SerializeField]
        Timer dashTimer, dashCoolDownTimer;
        [SerializeField]
        float dashSpeed = 3;
        [SerializeField]
        AnimationCurve dashSpeedCurve;
        [SerializeField]
        ParticleSystem dashEffect;
        bool dashing, dashCoolDown;
        public bool Dashing { get { return dashing; } }

        [SerializeField]
        Transform aimCursor;
        [SerializeField]
        float aimRotateSpeed = 3;
        float aimRotateVelocity;
        float aimAngle;
        bool mouseAiming;
        [SerializeField]
        float bulletSpeed = 3;
        [SerializeField]
        Timer bulletColddownTimer;
        bool bulletColddown;

        [SerializeField]
        float knockBackForce;
        [SerializeField]
        Timer knockBackTimer;
        bool knockback;

        new Collider2D collider;

        void SetupInput() {
            inputActions = new PlayerInputMap();
            inputActions.Playing.Move.performed += UpdateMoveVelocity;
            inputActions.Playing.Move.canceled += UpdateMoveVelocity;

            inputActions.Playing.Dash.performed += DashPerform;

            inputActions.Playing.Shoot.performed += ShootPerform;

            inputActions.Playing.MouseMove.performed += UpdateMousePosition;
            inputActions.Playing.MouseMove.canceled += UpdateMousePosition;

            inputActions.Playing.RotateCrosshair.performed += RotateCrosshair;
            inputActions.Playing.RotateCrosshair.canceled += RotateCrosshair;

            inputActions.Playing.Pause.performed += delegate { GameMgr.mgr.PauseGame(); };
        }

        private void Awake() {
            mainCamera = Camera.main;
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();

            fullHealth = health;

            SetupInput();
        }

        public void Reset() {
            health = fullHealth;
            GameMgr.mgr.UpdateHealthBar((float)health / (float)fullHealth);
        }

        private void OnEnable() { inputActions.Enable(); }
        private void OnDisable() { inputActions.Disable(); }

        private void UpdateMoveVelocity(InputAction.CallbackContext context) {
            if (dashing) return;
            moveVelocity = context.ReadValue<Vector2>();
        }

        private void DashPerform(InputAction.CallbackContext context) {
            if (dashing || dashCoolDown || (moveVelocity.x == 0 && moveVelocity.y == 0)) return;
            dashing = true;
            dashCoolDown = true;

            dashEffect.Play();

            dashTimer.Reset();
            dashCoolDownTimer.Reset();

            rigidbody2D.velocity = Mathf.Lerp(1, dashSpeed, dashSpeedCurve.Evaluate(dashTimer.Progress)) * speedMultiplier * moveVelocity;
        }

        private void ShootPerform(InputAction.CallbackContext context) {
            if (bulletColddown) return;

            bulletColddownTimer.Reset();
            bulletColddown = true;
            Vector2 vec = aimCursor.position - transform.position;
            Bullet bullet = Bullet.Instantiate();
            bullet.Setup(transform.position, vec.normalized * bulletSpeed);
        }

        private void UpdateMousePosition(InputAction.CallbackContext context) {
            mouseAiming = true;

            Vector3 cursorPos = mainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            cursorPos.z = 0;
            aimCursor.position = cursorPos;
        }

        protected void RotateCrosshair(InputAction.CallbackContext context) {
            aimRotateVelocity = -context.ReadValue<float>();
        }

        private void FixedUpdate() {
            if (knockback) {
                if (knockBackTimer.UpdateEnd) knockback = false;
                return;
            }

            if (dashing) {
                if (dashTimer.FixedUpdateEnd) {
                    dashing = false;
                    dashEffect.Stop();
                }
                else rigidbody2D.velocity = Mathf.Lerp(1, dashSpeed, dashSpeedCurve.Evaluate(dashTimer.Progress)) * speedMultiplier * moveVelocity;
            }
            else rigidbody2D.velocity = speedMultiplier * moveVelocity;

            if (aimRotateVelocity != 0) {
                aimAngle += aimRotateVelocity * aimRotateSpeed;
                aimCursor.position = transform.position + (Vector3)DegreeToVector2(aimAngle) * 3;
            }

            if (dashCoolDown && dashCoolDownTimer.FixedUpdateEnd) dashCoolDown = false;
            if (bulletColddown && bulletColddownTimer.UpdateEnd) bulletColddown = false;
        }

        public void OnDamage(Transform other, int amount) {
            health -= amount;
            if (health < 0) {
                GameMgr.mgr.GameOver();
                enabled = false;
                rigidbody2D.velocity = Vector2.zero;
                return;
            }
            
            if (health == 0) health = 1;

            GameMgr.mgr.UpdateHealthBar((float) health / (float) fullHealth);

            knockback = true;

            Vector2 vec = transform.position - other.position;
            rigidbody2D.velocity = vec.normalized * knockBackForce;
            knockBackTimer.Reset();

            StartCoroutine(GameMgr.mgr.ShakeCamera(5, 0.15f));
        }
    }
}