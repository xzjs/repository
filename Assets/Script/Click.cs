using UnityEngine;

namespace Assets.Script
{
    public class Click : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Transform shelf = hit.transform;
                    if (shelf.tag != "Shelf")
                    {
                        shelf = shelf.parent;
                    }

                    Debug.Log(shelf.gameObject.name);
                }
            }
        }
    }
}
