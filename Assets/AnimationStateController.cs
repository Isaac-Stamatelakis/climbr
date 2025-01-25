using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CalendarUnit = UnityEngine.iOS.CalendarUnit;

public class AnimationStateController : MonoBehaviour
{
    private const string RUN_NAME = "Standard_Run";
    private const string WALK_NAME = "Walking";
    private Animator animator;
    private Rigidbody rb;
    public float Speed = 5;

    private List<(KeyCode[], Vector3 direction)> KeyList = new List<(KeyCode[], Vector3 direction)>
    {
        (new[] { KeyCode.UpArrow, KeyCode.W }, Vector3.back),
        (new[] { KeyCode.DownArrow, KeyCode.S }, Vector3.forward),
        (new[] { KeyCode.LeftArrow, KeyCode.A }, Vector3.right),
        (new[] { KeyCode.RightArrow, KeyCode.D }, Vector3.left),
    };
    
    // AnimatorStart is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
    }

    private bool IsInputing(KeyCode[] keycodes)
    {
        foreach (KeyCode keyCode in keycodes)
        {
            if (Input.GetKey(keyCode)) return true;
        }

        return false;
    }
    // Update is called once per frame
    void Update()
    {
        const float RUN_SPEED_MULTPLIER = 1.5f;
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool inputed = false;
        foreach (var (keyCodes, direction) in KeyList)
        {
            if (!IsInputing(keyCodes)) continue;
            float moveSpeed = Speed;
            if (running) moveSpeed *= RUN_SPEED_MULTPLIER;
            
            rb.AddForce(moveSpeed*direction);
            transform.rotation = Quaternion.LookRotation(direction);
            inputed = true;
        }

        if (inputed)
        {
            animator.Play(running ? RUN_NAME : WALK_NAME);
        }
    }
}
