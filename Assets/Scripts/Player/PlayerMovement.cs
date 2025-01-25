using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody rb;
        public float Speed = 20;

        public void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Speed*Vector3.right);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(Speed*Vector3.left);
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(Speed*Vector3.forward);
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(Speed*Vector3.back);
            }
        }
    }
}
