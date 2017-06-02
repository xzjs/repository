using System;
using System.Collections;
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
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            UiController uiController = GameObject.Find("UI Root").GetComponent<UiController>();
            uiController.CancelHighlight();
            uiController.ShowSimilarGood(string.Format("{0}(Clone)", GoodName));
            StartCoroutine(GetData(gameController.FlowUrl, GoodName));
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

        /// <summary>
        /// 加载货物的协程
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator GetData(string url, string param)
        {
            WWW getData = new WWW(url + "?name=" + param);
            yield return getData;
            try
            {
                if (getData.error != null)
                {
                    Debug.Log(getData.error);
                }
                else
                {
                    Debug.Log(getData.text);
                    RecordList recordList = GameObject.Find("RecordList").GetComponent<RecordList>();
                    foreach (Transform cell in recordList.Grid.transform)
                    {
                        Destroy(cell.gameObject);
                    }
                    Record[] records = JsonHelper.FromJson<Record>("{\"Items\":" + getData.text + "}");
                    foreach (var record in records)
                    {
                        GameObject recordGameObject = Resources.Load("UI/RecordItem") as GameObject;
                        recordGameObject.GetComponent<RecordItem>().SetData(new Record
                        {
                            name = record.name,
                            num = record.num,
                            unit = record.unit,
                            action = record.action,
                            time = record.time
                        });
                        recordList.Grid.gameObject.AddChild(recordGameObject);
                    }
                    
                    recordList.Grid.repositionNow = true;
                    recordList.TotaLabel.text = string.Format("现库存：{0}{1}", NumLabel.text, UnitLabel.text);
                    recordList.TweenPosition.PlayForward();
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception.ToString());
            }
        }
    }
}
