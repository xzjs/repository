using UnityEngine;

namespace Assets.Script
{
    public class MyFirstPersonController : MonoBehaviour
    {
        public float speed = 2f;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                float y = Input.GetAxis("Mouse X") * speed;
                float x = Input.GetAxis("Mouse Y") * speed;
                transform.GetComponentInChildren<Camera>().transform.Rotate(-x, 0, 0);
                transform.Rotate(0, y, 0, Space.World);
            }

            Vector3 moveDirection = Vector3.zero;
            CharacterController controller = GetComponent<CharacterController>();

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}
