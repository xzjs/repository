using UnityEngine;

namespace Assets.Script
{
    public class RecordItem : MonoBehaviour
    {
        public UILabel NameLabel, ActionLabel, TimeLabel, NumLabel, UnitLabel;
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="record">记录数据</param>
        public void SetData(Record record)
        {
            NameLabel.text = record.name;
            ActionLabel.text = record.action;
            TimeLabel.text = record.time;
            NumLabel.text = record.num;
            UnitLabel.text = record.unit;
        }
    }
}
