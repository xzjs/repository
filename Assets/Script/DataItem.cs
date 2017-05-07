using UnityEngine;

namespace Assets.Script
{
    public class DataItem : MonoBehaviour
    {
        public UILabel NameLabel, NumLabel, UnitLabel;
        public string GoodName;

        private Good _good;

        public void Click()
        {
            UiController uiController = GameObject.Find("UI Root").GetComponent<UiController>();
            uiController.ShowSimilarGood(string.Format("{0}(Clone)", GoodName));
        }

        /// <summary>
        /// 设置货物
        /// </summary>
        /// <param name="good"></param>
        public void SetGood(Good good)
        {
            GoodName = good.model_name;
            NameLabel.text = good.name;
            NumLabel.text = good.num.ToString();
            UnitLabel.text = good.unit;
        }
    }
}
