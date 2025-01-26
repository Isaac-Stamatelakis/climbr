using System;
using UnityEngine;

namespace Player
{
    public class MountainClimbMovement : MonoBehaviour
    {
        private const float MAX_REACH = 1f;
        
        private enum ArmTurn
        {
            Either,
            Left,
            Right
        }
        
        [SerializeField] private GameObject mLeftHand;
        [SerializeField] private GameObject mRightHand;
        [SerializeField] private float yOffset;
        [SerializeField] private float armSpeed = 3f;
        private ArmTurn climbingArm = ArmTurn.Left;
        private Camera mainCamera;
        private bool mouseDown;
        void Start()
        {
            mLeftHand.gameObject.SetActive(false);
            mRightHand.gameObject.SetActive(false);
            mainCamera = Camera.main;
        }
        
        public void MovementUpdate()
        {
            

            if (Input.GetMouseButtonDown(0)) mouseDown = true;
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                switch (climbingArm)
                {
                    case ArmTurn.Either:
                        break;
                    case ArmTurn.Left:
                        climbingArm = ArmTurn.Right;
                        break;
                    case ArmTurn.Right:
                        climbingArm = ArmTurn.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -1);
            }

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = transform.position.z;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            
            /*
             // Uses player distance
            float distance = Vector3.Distance(transform.position, worldPosition);
            if (distance > MAX_REACH)
            {
                Vector3 adjustedPosition = transform.position;
                adjustedPosition.y += yOffset;
                Vector3 direction = (worldPosition - adjustedPosition).normalized;
                worldPosition = adjustedPosition + direction * MAX_REACH;
            }
            */
            
            if ()

            switch (climbingArm)
            {
                case ArmTurn.Either:
                    break;
                case ArmTurn.Left:
                    if (worldPosition.x > transform.position.x) worldPosition.x = transform.position.x;
                    break;
                case ArmTurn.Right:
                    if (worldPosition.x < transform.position.x) worldPosition.x = transform.position.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (!mouseDown) return;
            
            switch (climbingArm)
            {
                case ArmTurn.Either:
                    break;
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

            if (mLeftHand.gameObject.activeInHierarchy && mRightHand.gameObject.activeInHierarchy)
            {
                Vector3 midPoint = Vector3.Lerp(mLeftHand.transform.position, mRightHand.transform.position, 0.5f);
                transform.position = new Vector3(midPoint.x, midPoint.y, transform.position.z);
            }
            
        }
        
    }
}
