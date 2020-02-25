using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysic : MonoBehaviour {
	// private static LayerMask groundLayer;

	public int Direction { get { return transform.localScale.x > 0? 1: -1; } }
	public Vector2 Velocity { get { return velocity; } }

	public bool IsGrounded { get { return isGrounded; } }
	public bool IsFaceWall { get { return isFaceWall; } }
	private bool isGrounded, isFaceWall;

	public bool IsJumping { get { return state == ActionState.Jumping; } }
	public bool IsFalling { get { return state == ActionState.Falling; } }
	public bool IsWallGripping { get { return state == ActionState.WallGripping; } }

	Rigidbody2D rigid2d;
	PlayerInput input;

	private Vector2 velocity;
	[SerializeField]
	private LayerMask groundLayer;
	[SerializeField]
	private float accelerateSpeed, maxSpeed, slowDownSpeed;
	[SerializeField]
	private float jupmForce, jumpTime;
	[SerializeField]
	private float fallDownMultiplier, maxFallSpeed;
	[SerializeField]
	private float gripFalllMultiplier, gripTime;
	[SerializeField]
	private float wallJumpTime, wallJumpSpeed, wallJumpForce;

	private float originalGravity;
	private float jupmTimeCount, keepDirectionCount, gripCount;
	private int grippingDirection, keepDirection;

	private ActionState state = ActionState.Idle;
	private enum ActionState { Idle, Walking, Jumping, Falling, WallGripping }

	// Use this for initialization
	void Awake () {
		rigid2d = GetComponent<Rigidbody2D>();
		input = GetComponent<PlayerInput>();

        originalGravity = rigid2d.gravityScale;
	}

	void Turn(int direction) {
		transform.localScale = new Vector3((direction > 0? 1: -1) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	}
	void Turn(int direction, ref bool turnning) {
		transform.localScale = new Vector3((direction > 0? 1: -1) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		turnning = true;
	}

	void HandleMovement() {
		if (keepDirectionCount > 0) {
			keepDirectionCount -= Time.deltaTime;

			if (input.Left && keepDirection == -1) {
				velocity.x = Mathf.Max(velocity.x - accelerateSpeed * Time.deltaTime, -maxSpeed);
				Turn(-1);
			} else if (input.Right && keepDirection == 1) {
				velocity.x = Mathf.Min(velocity.x + accelerateSpeed * Time.deltaTime, maxSpeed);
				Turn(1);
			}
			return;
		}

		if (input.Left) {
			if (velocity.x >= 0) {
				velocity.x = -accelerateSpeed * Time.deltaTime; Turn(-1);
			} else
				velocity.x = Mathf.Max(velocity.x - accelerateSpeed * Time.deltaTime, -maxSpeed);
		} else if (input.Right) {
			if (velocity.x <= 0) {
				velocity.x = accelerateSpeed * Time.deltaTime; Turn(1);
			} else
				velocity.x = Mathf.Min(velocity.x + accelerateSpeed * Time.deltaTime, maxSpeed);
		} else if (velocity.x != 0) {
			velocity.x = Mathf.MoveTowards(velocity.x, 0, slowDownSpeed);
		}
	}

	void SnapToWall() {
		rigid2d.gravityScale = 0;
		velocity.y = maxFallSpeed * gripFalllMultiplier;
		grippingDirection = Direction;
		gripCount = gripTime;
	}

	void HandleJumping() {
		if (input.JumpDown && !IsJumping) {
			if (IsGrounded) {
				velocity.y = jupmForce;
				jupmTimeCount = jumpTime;

				// RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, Vector2.down, 2f, groundLayer);
				// SpriteSheetParticleController.SpawnParticle(ParticleType.Jump, (Vector3) hit.point);
			} else if (IsWallGripping) {
                LooseGrip();
				velocity.x = -grippingDirection * maxSpeed * wallJumpSpeed;
				velocity.y = jupmForce * wallJumpForce;
				jupmTimeCount = jumpTime;
				keepDirectionCount = wallJumpTime;
				keepDirection = -grippingDirection;

				// RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, grippingDirection == 1? Vector2.right: Vector2.left, 2f, groundLayer);
				// SpriteSheetParticleController.SpawnParticle(ParticleType.WallJump, (Vector3) hit.point, keepDirection);
			}
		} else if (input.Jump && IsJumping) {
			if (jupmTimeCount > 0) {
				jupmTimeCount -= Time.deltaTime;
				velocity.y = jupmForce;
			}
		} else if (input.JumpUp && IsJumping) jupmTimeCount = 0;
	}

	void LooseGrip() {
		rigid2d.gravityScale = originalGravity;
	}

	void HandleWallGrip() {
		if (IsWallGripping) {
			if (IsGrounded || !IsFaceWall) LooseGrip();
			else if (grippingDirection != Direction) LooseGrip();
			// else {
			// 	if (gripCount > 0) {
			// 		gripCount = Mathf.MoveTowards(gripCount, 0, Time.deltaTime);
			// 		velocity.y = Mathf.Lerp(0, maxFallSpeed * gripFalllMultiplier, (gripTime - gripCount) / gripTime);
			// 	}
			// }
		} else if (IsFalling && !IsWallGripping && IsFaceWall && !IsGrounded) {
			SnapToWall();
		}
	}

	ActionState DecideState() {
		if (rigid2d.gravityScale == 0) return ActionState.WallGripping;

		if (velocity.y > 0.1f) return ActionState.Jumping;
		else if (velocity.y < -0.1f) {
			return ActionState.Falling;
		}
		else return Mathf.Abs(velocity.x) > 0.1f? ActionState.Walking: ActionState.Idle;
	}

	void ApplyToNewState(ActionState newState) {
		switch (newState) {
			default:
				break;
		}
	}

	void Update() {
		velocity = rigid2d.velocity;
		HandleMovement();
		HandleWallGrip();
		HandleJumping();

		// Make falling faster
		if (velocity.y < -0.1f && !IsWallGripping) {
			velocity.y = Mathf.Max(velocity.y - fallDownMultiplier * Time.deltaTime, maxFallSpeed);
		}

		state = DecideState();
		rigid2d.velocity = velocity;
	}

	void FixedUpdate() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.33f, groundLayer);
		Debug.DrawRay(transform.position, Vector2.down * 0.33f, Color.red, 0.05f);
        isGrounded = hit.collider != null;

        hit = Physics2D.Raycast(transform.position, Vector2.right * Direction, 0.33f, groundLayer);
		Debug.DrawRay(transform.position, Vector2.right * Direction * 0.33f, Color.red);
		isFaceWall = hit.collider != null;
	}
}
