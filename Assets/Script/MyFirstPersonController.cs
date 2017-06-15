using UnityEngine;

namespace Assets.Script
{
    public class MyFirstPersonController : MonoBehaviour {
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
                transform.GetComponentInChildren<Camera>().transform.Rotate(-x,0,0);
                transform.Rotate(0,y,0, Space.World);
            }
            float h = Input.GetAxis("Horizontal"); //获取水平轴
            float v = Input.GetAxis("Vertical"); //获取垂直轴
            transform.Translate(h * Time.deltaTime * speed, 0, v * Time.deltaTime * speed);
        }
    }
}
