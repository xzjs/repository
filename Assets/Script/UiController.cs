using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public class UiController : MonoBehaviour
    {
        public UILabel PerspectiveLabel;
        public UIGrid[] UiGrids;
        public string Key;

        private Dictionary<string, GoodInfo> _dictionary;

        public void ChangePerspective()
        {
            PerspectiveLabel.text = PerspectiveLabel.text == "漫游视角" ? "全局视角" : "漫游视角";
            GameObject controller = GameObject.FindWithTag("GameController");
            controller.GetComponent<GameController>().ChangeCamera();
        }

        /// <summary>
        /// 呈现数据
        /// </summary>
        /// <param name="index">显示表格序号</param>
        /// <param name="dictionary">要显示的数据</param>
        public void ShowData(int index, Dictionary<string, GoodInfo> dictionary)
        {
            foreach (var info in dictionary)
            {
                GameObject dataItem = Resources.Load("UI/BigItem") as GameObject;
                dataItem.GetComponent<DataItem>().NameLabel.text = info.Key;
                dataItem.GetComponent<DataItem>().NumLabel.text = info.Value.num.ToString();
                dataItem.GetComponent<DataItem>().UnitLabel.text = info.Value.unit;
                UiGrids[index].gameObject.AddChild(dataItem);
            }
            UiGrids[index].repositionNow = true;
        }

        public void Search()
        {

        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="dictionary">数据</param>
        public void SetData(Dictionary<string, GoodInfo> dictionary)
        {
            _dictionary = dictionary;
            ShowData(0, dictionary);
        }
    }
}
