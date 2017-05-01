using UnityEngine;

namespace Assets.Script
{
    public class UiController : MonoBehaviour
    {
        public UILabel PerspectiveLabel;

        public void ChangePerspective()
        {
            PerspectiveLabel.text = PerspectiveLabel.text == "漫游视角" ? "全局视角" : "漫游视角";
            GameObject controller = GameObject.FindWithTag("GameController");
            controller.GetComponent<GameController>().ChangeCamera();
        }
    }
}
