using System;
using UnityEngine;

namespace Assets.Script
{
    public class GodPerspective : MonoBehaviour
    {
        public float Near = 20.0f;
        public float Far = 100.0f;

        public float SensitivityX = 2f;
        public float SensitivityY = 2f;
        public float SensitivetyZ = 2f;
        public float SensitivetyMove = 2f;
        public float SensitivetyMouseWheel = 20f;

        public float Speed = 5f;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // 滚轮实现镜头缩进和拉远
            if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.1)
            {
                GetComponent<Camera>().fieldOfView = GetComponent<Camera>().fieldOfView - Input.GetAxis("Mouse ScrollWheel") * SensitivetyMouseWheel;
                GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, Near, Far);
            }
            //鼠标右键实现视角转动，类似第一人称视角
            if (Input.GetMouseButton(0))
            {
                float rotationX = Input.GetAxis("Mouse X") * SensitivityX;
                float rotationY = Input.GetAxis("Mouse Y") * SensitivityY;
                transform.Rotate(-rotationY, rotationX, 0);
            }

            ////键盘按钮←和→实现视角水平旋转
            //if (Math.Abs(Input.GetAxis("Horizontal")) > 0.1)
            //{
            //    float rotationZ = Input.GetAxis("Horizontal") * SensitivetyZ;
            //    transform.Rotate(0, 0, rotationZ);
            //}

            if (Math.Abs(Input.GetAxis("Vertical")) > 0.1)
            {
                GetComponent<Camera>().transform.Translate(new Vector3(0, 0, Input.GetAxis("Vertical") * Speed * Time.deltaTime));
            }
            if (Math.Abs(Input.GetAxis("Horizontal")) > 0.1)
            {
                GetComponent<Camera>().transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Speed * Time.deltaTime, 0, 0));
            }
        }
    }
}
