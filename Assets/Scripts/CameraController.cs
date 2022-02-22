using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //Class taken from public domain: https://stackoverflow.com/questions/58328209/how-to-make-a-free-fly-camera-script-in-unity-with-acceleration-and-decceleratio
    //Modified for use case
    
    
    [Header("Constants")]

    //unity controls and constants input
    public float AccelerationMod;
    public float XAxisSensitivity;
    public float YAxisSensitivity;
    public float DecelerationMod;

    [Space]

    [Range(0, 89)] public float MaxXAngle = 60f;

    [Space]

    public float MaximumMovementSpeed = 1f;

    private float maxMove;

    [Header("Controls")]

    public KeyCode Forwards = KeyCode.W;
    public KeyCode Backwards = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Up = KeyCode.Space;
    public KeyCode Down = KeyCode.LeftControl;
    public KeyCode Sprint = KeyCode.LeftShift;

    private Vector3 _moveSpeed;
    private bool sprint;
    
    private void Start() {
        _moveSpeed = Vector3.zero;
        maxMove = MaximumMovementSpeed;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        HandleMouseRotation();

        HandleKeyInput();

        // clamp the move speed
        maxMove = sprint ? MaximumMovementSpeed * 4 : MaximumMovementSpeed;
        
        if(_moveSpeed.magnitude > maxMove) {
            _moveSpeed = _moveSpeed.normalized * maxMove;
        }

        transform.Translate(_moveSpeed);
    }

    private void HandleKeyInput() {
        bool keyDown = false;

        //key input detection
        if (Input.GetKey(Forwards)) {
            keyDown = true;
            _moveSpeed.z += 0.01f * (sprint ? 4 : 1);
        } else {
            if (_moveSpeed.z > 0) _moveSpeed.z -= 0.02f * (sprint ? 4 : 1);;
        }

        if (Input.GetKey(Backwards)) {
            keyDown = true;
            _moveSpeed.z -= 0.01f;
        } else {
            if (_moveSpeed.z < 0) _moveSpeed.z += 0.02f;
        }

        if (Input.GetKey(Left)) {
            keyDown = true;
            _moveSpeed.x -= 0.01f;
        } else {
            if (_moveSpeed.x < 0) _moveSpeed.x += 0.02f;
        }

        if (Input.GetKey(Right)) {
            keyDown = true;
            _moveSpeed.x += 0.01f;
        } else {
            if (_moveSpeed.x > 0) _moveSpeed.x -= 0.02f;
        }

        if (Input.GetKey(Up)) {
            keyDown = true;
            _moveSpeed.y += 0.01f;
        } else {
            if (_moveSpeed.y > 0) _moveSpeed.y -= 0.02f;
        }

        if (Input.GetKey(Down)) {
            keyDown = true;
            _moveSpeed.y -= 0.01f;
        } else {
            if (_moveSpeed.y < 0) _moveSpeed.y += 0.02f;
        }

        if (Input.GetKey(Sprint)) {
            keyDown = true;
            sprint = true;
        }
        else {
            sprint = false;
        }

        if (!keyDown) {
            if (_moveSpeed.x < 0.02f && _moveSpeed.x > -0.02f) _moveSpeed.x = 0.0f;
            if (_moveSpeed.y < 0.02f && _moveSpeed.y > -0.02f) _moveSpeed.y = 0.0f;
            if (_moveSpeed.z < 0.02f && _moveSpeed.z > -0.02f) _moveSpeed.z = 0.0f;
        }
    }

    private float _rotationX;

    private void HandleMouseRotation()
    {
        //mouse input
        var rotationHorizontal = XAxisSensitivity * Input.GetAxis("Mouse X");
        var rotationVertical = YAxisSensitivity * Input.GetAxis("Mouse Y");

        //applying mouse rotation
        // always rotate Y in global world space to avoid gimbal lock
        transform.Rotate(Vector3.up * rotationHorizontal, Space.World);

        var rotationY = transform.localEulerAngles.y;

        _rotationX += rotationVertical;
        _rotationX = Mathf.Clamp(_rotationX, -MaxXAngle, MaxXAngle);

        transform.localEulerAngles = new Vector3(-_rotationX, rotationY, 0);
    }

    private void HandleDeceleration(Vector3 acceleration)
    {
        //deceleration functionality
        if (Mathf.Approximately(Mathf.Abs(acceleration.x), 0))
        {
            if (Mathf.Abs(_moveSpeed.x) < DecelerationMod)
            {
                _moveSpeed.x = 0;
            }
            else
            {
                _moveSpeed.x -= DecelerationMod * Mathf.Sign(_moveSpeed.x);
            }
        }

        if (Mathf.Approximately(Mathf.Abs(acceleration.y), 0))
        {
            if (Mathf.Abs(_moveSpeed.y) < DecelerationMod)
            {
                _moveSpeed.y = 0;
            }
            else
            {
                _moveSpeed.y -= DecelerationMod * Mathf.Sign(_moveSpeed.y);
            }
        }

        if (Mathf.Approximately(Mathf.Abs(acceleration.z), 0))
        {
            if (Mathf.Abs(_moveSpeed.z) < DecelerationMod)
            {
                _moveSpeed.z = 0;
            }
            else
            {
                _moveSpeed.z -= DecelerationMod * Mathf.Sign(_moveSpeed.z);
            }
        }
    }
}