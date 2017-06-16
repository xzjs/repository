using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public class Detail : MonoBehaviour
    {
        public UILabel TotaLabel;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="good">货物数据</param>
        public void SetData(Good good)
        {
            Dictionary<string, Good> totalDictionary =
                GameObject.Find("GameController").GetComponent<GameController>().TotalDictionary;
            Good g = totalDictionary[good.name];
            TotaLabel.text = string.Format("货物总计：{0} {1}", g.num, g.unit);
        }
    }
}
