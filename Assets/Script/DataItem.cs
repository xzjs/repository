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
            RecordList recordList = GameObject.Find("RecordList").GetComponent<RecordList>();
            GameObject record=Resources.Load("UI/RecordItem") as GameObject;
            record.GetComponent<RecordItem>().SetData(new Record
            {
                name = NameLabel.text,
                num=NumLabel.text,
                unit = UnitLabel.text,
                action = "入库",
                time = "2017-5-8 10:06:59"
            });
            recordList.Grid.gameObject.AddChild(record);
            recordList.Grid.repositionNow = true;
            recordList.TotaLabel.text = string.Format("现库存：{0}{1}", NumLabel.text, UnitLabel.text);
            recordList.TweenPosition.PlayForward();
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
