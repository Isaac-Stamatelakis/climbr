using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class MountainClimbMovement : MonoBehaviour
    {
        internal enum MovementMode
        {
            Arm,
            Key
        }
        
        private enum ArmTurn
        {
            Left,
            Right
        }
        
        [SerializeField] private GameObject mLeftHand;
        [SerializeField] private GameObject mRightHand;
        [SerializeField] private float minArmReach = 0.25f;
        [SerializeField] private float maxArmReach = 2f;
        [SerializeField] private float armConnectionPoint = 0.3f;
        [SerializeField] private float armSpeed = 10f;
        [SerializeField] private MovementMode movementMode;
        [SerializeField] private MountainGenerator mountainGenerator;
        private ArmTurn climbingArm = ArmTurn.Left;
        private Camera mainCamera;
        private bool mouseDown;
        private Rigidbody rb;
        
        void Start()
        {
            mLeftHand.gameObject.SetActive(false);
            mRightHand.gameObject.SetActive(false);
            mainCamera = Camera.main;
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize()
        {
            mLeftHand.gameObject.SetActive(true);
            mLeftHand.transform.position = transform.position + new Vector3(-1, 2, 0) * 0.5f;
            
            mRightHand.gameObject.SetActive(true);
            mRightHand.transform.position = transform.position + new Vector3(1, 2, 0) * 0.5f;

            var vector3 = mainCamera.transform.position;
            vector3.z = -1;
            mainCamera.transform.position = vector3;
        }

        private void SwitchArm()
        {
            switch (climbingArm)
            {
                case ArmTurn.Left:
                    climbingArm = ArmTurn.Right;
                    break;
                case ArmTurn.Right:
                    climbingArm = ArmTurn.Left;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = transform.position.z;
            return mainCamera.ScreenToWorldPoint(mousePosition);
        }

        private void ClampWorldPosition(ref Vector3 worldPosition)
        {
            Vector3 adjustedPosition = transform.position;
            adjustedPosition.y += armConnectionPoint;
            Vector3 direction = (worldPosition - adjustedPosition).normalized;
            float distance = Vector3.Distance(transform.position, worldPosition);
            
            if (distance > maxArmReach)
            {
                worldPosition = adjustedPosition + direction * maxArmReach;
            } else if (distance < minArmReach)
            {
                worldPosition = adjustedPosition + direction * minArmReach;
            }

            // This might be slightly off because of camera angle
            float angle = Mathf.Atan2(worldPosition.y - adjustedPosition.y, worldPosition.x - adjustedPosition.x) * Mathf.Rad2Deg;
            
            switch (climbingArm)
            {
                case ArmTurn.Left:
                    if (worldPosition.x > transform.position.x) worldPosition.x = transform.position.x;
                    break;
                case ArmTurn.Right:
                    if (worldPosition.x < transform.position.x) worldPosition.x = transform.position.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator MoveCoroutine()
        {
            Vector3 midPoint = Vector3.Lerp(mLeftHand.transform.position, mRightHand.transform.position, 0.5f);
            
            Vector3 playerMidPoint = new Vector3(midPoint.x, midPoint.y - armConnectionPoint, transform.position.z);
            Vector3 cameraMidPoint = new Vector3(midPoint.x, midPoint.y + 2, -1);
            Vector3 initialCameraPosition = mainCamera.transform.position;
            Vector3 initialPlayerPosition = transform.position;
            const int ITERATIONS = 10;
            for (int i = 0; i < ITERATIONS; i++)
            {
                mainCamera.transform.position = Vector3.Lerp(initialCameraPosition,cameraMidPoint,(float)i/ITERATIONS);
                transform.position = Vector3.Lerp(initialPlayerPosition,playerMidPoint,(float)i/ITERATIONS);
                yield return null;
            }
        }

        private void BasicMovement()
        {
            const float MOVE_SPEED = 5f;
            Vector3 velocity = rb.velocity;
            velocity.x = 0;
            velocity.y = 0;
            if (Input.GetKey(KeyCode.A))
            {
                velocity.x = -MOVE_SPEED;
            }

            if (Input.GetKey(KeyCode.D))
            {
                velocity.x = MOVE_SPEED;
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                velocity.y = MOVE_SPEED;
            }

            if (Input.GetKey(KeyCode.S))
            {
                velocity.y = -MOVE_SPEED;
            }
            
            rb.velocity = velocity;
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -1);
           
        }
        
        public void MovementUpdate()
        {
            mountainGenerator.Refresh(transform);
            if (movementMode == MovementMode.Key)
            {
                BasicMovement();
                return;
            }
            if (Input.GetMouseButtonDown(0)) mouseDown = true;
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                SwitchArm();
                StartCoroutine(MoveCoroutine());
            }

            Vector3 worldPosition = GetMouseWorldPosition();
            ClampWorldPosition(ref worldPosition);
            
            
            if (!mouseDown) return;
            
            switch (climbingArm)
            {
                case ArmTurn.Left:
                    mLeftHand.gameObject.SetActive(true);
                    mLeftHand.transform.position = Vector3.MoveTowards(
                        mLeftHand.transform.position, 
                        worldPosition, 
                        armSpeed * Time.deltaTime
                    );
                    break;
                case ArmTurn.Right:
                    mRightHand.gameObject.SetActive(true);
                    mRightHand.transform.position = Vector3.MoveTowards(
                        mRightHand.transform.position, 
                        worldPosition, 
                        armSpeed * Time.deltaTime
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}
