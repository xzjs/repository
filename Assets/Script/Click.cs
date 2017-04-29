using UnityEngine;

namespace Assets.Script
{
    public class Click : MonoBehaviour
    {
        public Shader RimLightShader;
        public Color RimColor = new Color(0.2f, 0.8f, 10.6f, 1);

        private MeshRenderer _mesh;
        private Color _color;
        private Shader _shader;

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

                    foreach (Transform childTransform in shelf)
                    {
                        _mesh = childTransform.gameObject.GetComponentInChildren<MeshRenderer>();

                        _shader = _mesh.material.shader;

                        _mesh.material.shader = RimLightShader;
                        _mesh.material.SetColor("_RimColor", RimColor);
                    }
                }
            }
        }
    }
}
