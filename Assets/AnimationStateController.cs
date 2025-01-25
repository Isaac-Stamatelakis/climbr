using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
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
        bool inputed = false;
        foreach (var (keyCodes, direction) in KeyList)
        {
            if (!IsInputing(keyCodes)) continue;
            rb.AddForce(Speed*direction);
            transform.rotation = Quaternion.LookRotation(direction);
            inputed = true;
        }
        if (inputed) animator.Play("Walking");
    }
}
