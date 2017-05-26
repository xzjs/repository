using UnityEngine;

namespace Assets.Script
{
    public class FirstPersonController : MonoBehaviour {
        public float speed = 2f;
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
            if (Input.GetMouseButton(1))
            {
                float y = Input.GetAxis("Mouse X") * speed;
                float x = Input.GetAxis("Mouse Y") * speed;
                transform.Rotate(-x,0,0);
                transform.Rotate(0,y,0, Space.World);
            }
        }
    }
}
