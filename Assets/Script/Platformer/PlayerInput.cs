using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
	private bool keyLock;
	private bool inputRegistered;
	private KeyCodeSet keySet;
	
	public bool KeyLock { get { return keyLock; } }
	public bool Left { get { return !keyLock && inputRegistered && keySet.Left; }}
	public bool LeftDown { get { return !keyLock && inputRegistered && keySet.LeftDown; }}
	public bool LeftUp { get { return !keyLock && inputRegistered && keySet.LeftUp; }}
	public bool Right { get { return !keyLock && inputRegistered && keySet.Right; }}
	public bool RightDown { get { return !keyLock && inputRegistered && keySet.RightDown; }}
	public bool RightUp { get { return !keyLock && inputRegistered && keySet.RightUp; }}
	public bool Up { get { return !keyLock && inputRegistered && keySet.Up; }}
	public bool UpDown { get { return !keyLock && inputRegistered && keySet.UpDown; }}
	public bool UpUp { get { return !keyLock && inputRegistered && keySet.UpUp; }}
	public bool Down { get { return !keyLock && inputRegistered && keySet.Down; }}
	public bool DownDown { get { return !keyLock && inputRegistered && keySet.DownDown; }}
	public bool DownUp { get { return !keyLock && inputRegistered && keySet.DownUp; }}

	public bool Jump { get { return !keyLock && inputRegistered && keySet.Jump; } }
	public bool JumpDown { get { return !keyLock && inputRegistered && keySet.JumpDown; } }
	public bool JumpUp { get { return !keyLock && inputRegistered && keySet.JumpUp; } }

    public Vector2 ShootDirection { get {
        if (keyLock || !inputRegistered) return Vector2.zero;
        return keySet.ShootDirection;
    }}
    public bool Shoot { get { return !keyLock && inputRegistered && keySet.Shoot; } }
    public bool ShootDown { get { return !keyLock && inputRegistered && keySet.ShootDown; } }
    public bool ShootUp { get { return !keyLock && inputRegistered && keySet.ShootUp; } }

    // Update is called once per frame
    void Update()
    {
        if (!inputRegistered) WaitSignal();
        // TestKey();
    }

    public void TestKey() {
        if (Input.GetKey(KeyCode.JoystickButton0)) { Debug.Log(0); }
        if (Input.GetKey(KeyCode.JoystickButton1)) { Debug.Log(1); }
        if (Input.GetKey(KeyCode.JoystickButton2)) { Debug.Log(2); }
        if (Input.GetKey(KeyCode.JoystickButton3)) { Debug.Log(3); }
        if (Input.GetKey(KeyCode.JoystickButton4)) { Debug.Log(4); }
        if (Input.GetKey(KeyCode.JoystickButton5)) { Debug.Log(5); }
        if (Input.GetKey(KeyCode.JoystickButton6)) { Debug.Log(6); }
        if (Input.GetKey(KeyCode.JoystickButton7)) { Debug.Log(7); }
        if (Input.GetKey(KeyCode.JoystickButton8)) { Debug.Log(8); }
        if (Input.GetKey(KeyCode.JoystickButton9)) { Debug.Log(9); }
        if (Input.GetKey(KeyCode.JoystickButton10)) { Debug.Log(10); }
        if (Input.GetKey(KeyCode.JoystickButton11)) { Debug.Log(11); }
        if (Input.GetKey(KeyCode.JoystickButton12)) { Debug.Log(12); }
        if (Input.GetKey(KeyCode.JoystickButton13)) { Debug.Log(13); }
        if (Input.GetKey(KeyCode.JoystickButton14)) { Debug.Log(14); }
        if (Input.GetKey(KeyCode.JoystickButton15)) { Debug.Log(15); }
        if (Input.GetKey(KeyCode.JoystickButton16)) { Debug.Log(16); }
        if (Input.GetKey(KeyCode.JoystickButton17)) { Debug.Log(17); }
        if (Input.GetKey(KeyCode.JoystickButton18)) { Debug.Log(18); }
        if (Input.GetKey(KeyCode.JoystickButton19)) { Debug.Log(19); }
    }

	public void WaitSignal() {
		if (Keyboard1KeyCodeSet.AnyKey) {
			inputRegistered = true;
			keySet = new Keyboard1KeyCodeSet();
		}
        // else if (JoyStickKeyCodeSet.AnyKey) {
		// 	inputRegistered = true;
		// 	keySet = new JoyStickKeyCodeSet();
		// }
	}

    abstract class KeyCodeSet {
        public abstract bool Left { get; }
        public abstract bool LeftDown { get; }
        public abstract bool LeftUp { get; }
        public abstract bool Right { get; }
        public abstract bool RightUp { get; }
        public abstract bool RightDown { get; }
        public abstract bool Up { get; }
        public abstract bool UpDown { get; }
        public abstract bool UpUp { get; }
        public abstract bool Down { get; }
        public abstract bool DownDown { get; }
        public abstract bool DownUp { get; }
        public abstract bool Jump { get; }
        public abstract bool JumpDown { get; }
        public abstract bool JumpUp { get; }
        public abstract bool Shoot { get; }
        public abstract bool ShootDown { get; }
        public abstract bool ShootUp { get; }

        public abstract Vector2 ShootDirection { get; }

		public static bool AnyKey { get; }
    }

    private class Keyboard1KeyCodeSet: KeyCodeSet {
        static KeyCode left = KeyCode.A;
        static KeyCode right = KeyCode.D;
        static KeyCode up = KeyCode.W;
        static KeyCode down = KeyCode.S;
        static KeyCode jump = KeyCode.Space;
        static KeyCode shoot = KeyCode.LeftShift;

        public override bool Left { get { return Input.GetKey(left); } }
        public override bool LeftDown { get { return Input.GetKeyDown(left); } }
        public override bool LeftUp { get { return Input.GetKeyUp(left); } }
        public override bool Right { get { return Input.GetKey(right); } }
        public override bool RightDown { get { return Input.GetKeyDown(right); } }
        public override bool RightUp { get { return Input.GetKeyUp(right); } }
        public override bool Up { get { return Input.GetKey(up); } }
        public override bool UpDown { get { return Input.GetKeyDown(up); } }
        public override bool UpUp { get { return Input.GetKeyUp(up); } }
        public override bool Down { get { return Input.GetKey(down); } }
        public override bool DownDown { get { return Input.GetKeyDown(down); } }
        public override bool DownUp { get { return Input.GetKeyUp(down); } }
        public override bool Jump { get { return Input.GetKey(jump); } }
        public override bool JumpDown { get { return Input.GetKeyDown(jump); } }
        public override bool JumpUp { get { return Input.GetKeyUp(jump); } }
        public override bool Shoot { get { return Input.GetKey(shoot); } }
        public override bool ShootDown { get { return Input.GetKeyDown(shoot); } }
        public override bool ShootUp { get { return Input.GetKeyUp(shoot); } }

        public override Vector2 ShootDirection { get {
            Vector2 direction = new Vector2(Input.GetAxis("Secondary_Horizontal"),
                                            -Input.GetAxis("Secondary_Vertical"));
            if (Vector2.Distance(direction, Vector2.zero) < 1f) { return Vector2.zero; }
            return direction;
        } }
		
        public static bool AnyKey { get {
			return Input.GetKey(left) || Input.GetKey(right) || Input.GetKey(up) ||
				   Input.GetKey(down) || Input.GetKey(jump) || Input.GetKey(shoot);
		}}
    }

    private class JoyStickKeyCodeSet: KeyCodeSet {
        static KeyCode left = KeyCode.JoystickButton7;
        static KeyCode right = KeyCode.JoystickButton5;
        static KeyCode up = KeyCode.JoystickButton4;
        static KeyCode down = KeyCode.JoystickButton6;
        static KeyCode jump = KeyCode.JoystickButton14;
        static KeyCode shoot = KeyCode.JoystickButton10;

        public override bool Left { get { return Input.GetKey(left) || Input.GetAxisRaw("Horizontal_Joystick") == -1; } }
        public override bool LeftDown { get { return Input.GetKeyDown(left); } }
        public override bool LeftUp { get { return Input.GetKeyUp(left); } }
        public override bool Right { get { return Input.GetKey(right) || Input.GetAxisRaw("Horizontal_Joystick") == 1; } }
        public override bool RightDown { get { return Input.GetKeyDown(right); } }
        public override bool RightUp { get { return Input.GetKeyUp(right); } }
        public override bool Up { get { return Input.GetKey(up) || Input.GetAxisRaw("Vertical_Joystick") == -1;; } }
        public override bool UpDown { get { return Input.GetKeyDown(up); } }
        public override bool UpUp { get { return Input.GetKeyUp(up); } }
        public override bool Down { get { return Input.GetKey(down) || Input.GetAxisRaw("Vertical_Joystick") == 1;; } }
        public override bool DownDown { get { return Input.GetKeyDown(down); } }
        public override bool DownUp { get { return Input.GetKeyUp(down); } }
        public override bool Jump { get { return Input.GetKey(jump); } }
        public override bool JumpDown { get { return Input.GetKeyDown(jump); } }
        public override bool JumpUp { get { return Input.GetKeyUp(jump); } }
        public override bool Shoot { get { return Input.GetKey(shoot); } }
        public override bool ShootDown { get { return Input.GetKeyDown(shoot); } }
        public override bool ShootUp { get { return Input.GetKeyUp(shoot); } }

        public override Vector2 ShootDirection { get {
            Vector2 direction = new Vector2(Input.GetAxis("Secondary_Horizontal_Joystick"),
                                            Input.GetAxis("Secondary_Vertical_Joystick"));
            if (Vector2.Distance(direction, Vector2.zero) < 1f) { return Vector2.zero; }
            return direction;
        } }
		
        public static bool AnyKey { get {
			return Input.GetKey(left) || Input.GetKey(right) || Input.GetKey(up) ||
				   Input.GetKey(down) || Input.GetKey(jump) || Input.GetKey(shoot) ||
				   Mathf.Abs(Input.GetAxis("Horizontal_Joystick")) == 1 ||  Mathf.Abs(Input.GetAxis("Vertical_Joystick")) == 1;
		}}
    }
}
