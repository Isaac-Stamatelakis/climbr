using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    internal enum PlayerMovementMode
    {
        Ground,
        Climbing,
        Platform
    }
    public class AnimationStateController : MonoBehaviour
    {
        
        private const string RUN_NAME = "Standard_Run";
        private const string WALK_NAME = "Walking";
        private Animator animator;
        private Rigidbody rb;
        public float Speed = 5;
        private int wallCollisionCount;
        public bool CollidingWithWall => wallCollisionCount > 0;
        private PlayerMovementMode movementMode;
        private MountainClimbMovement mountainClimbMovement;
        
        private List<(KeyCode[], Vector3 direction)> KeyList = new List<(KeyCode[], Vector3 direction)>
        {
            (new[] { KeyCode.UpArrow, KeyCode.W }, Vector3.forward),
            (new[] { KeyCode.DownArrow, KeyCode.S }, Vector3.back),
            (new[] { KeyCode.LeftArrow, KeyCode.A }, Vector3.left),
            (new[] { KeyCode.RightArrow, KeyCode.D }, Vector3.right),
        };

        public void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Wall")) return;
            wallCollisionCount++;
        }

        public void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Wall")) return;
            wallCollisionCount--;
        }


        // AnimatorStart is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            mountainClimbMovement = GetComponent<MountainClimbMovement>();
            
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
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (movementMode == PlayerMovementMode.Climbing)
                {
                    movementMode = PlayerMovementMode.Ground;
                }
                else if (CollidingWithWall)
                {
                    movementMode = PlayerMovementMode.Climbing;
                }
                
            }

            switch (movementMode)
            {
                case PlayerMovementMode.Ground:
                    GroundMovementUpdate();
                    break;
                case PlayerMovementMode.Climbing:
                    mountainClimbMovement.MovementUpdate();
                    break;
                case PlayerMovementMode.Platform:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GroundMovementUpdate()
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
}
