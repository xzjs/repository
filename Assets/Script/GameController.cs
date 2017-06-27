using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject MainCamera, FirstPersonCamera;
    public Shelf[] Shelves;
    public Dictionary<string, List<GameObject>> GooDictionary;
    public TweenPosition TweenPosition;
    public Dictionary<string, Good> TotalDictionary;
    public string FlowUrl, PlanUrl;
    public string[] ModelNameStrings ={
        "baowenhu","default","fanghanfu","fangzaigongfu","humujing","jiaoxie","jijiushao","jiushengyi","kouzhao","maojinbei","maotan","mianru","mianyiku",
        "micafu","neiku","shoudiantong","shoutao","shuidai","toudeng","wazi","xiexingju","yuxie","yuyiku","zhangpeng","zhediechuang","zhiyuanzhifu"
    };

    // Use this for initialization
    void Start()
    {
        GooDictionary = new Dictionary<string, List<GameObject>>();
        TotalDictionary = new Dictionary<string, Good>();
        if (Application.isEditor)
        {
            LoadGoods("http://192.168.4.96:48093/web/shelfs.json");
            FlowUrl = "http://192.168.4.96:48093/web/flow.json";
            PlanUrl = "http://192.168.4.96:48093/web/plan.json";
        }
        else
        {
            Application.ExternalCall("LoadComplete");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 切换视角
    /// </summary>
    public void ChangeCamera()
    {
        if (MainCamera.activeSelf)
        {
            MainCamera.SetActive(false);
            FirstPersonCamera.SetActive(true);
        }
        else
        {
            MainCamera.SetActive(true);
            FirstPersonCamera.SetActive(false);
        }
    }

    /// <summary>
    /// 协程调用加载货物
    /// </summary>
    /// <param name="url"></param>
    public void LoadGoods(string url)
    {
        StartCoroutine(GetData(url));
    }

    /// <summary>
    /// 设置货物流动url
    /// </summary>
    /// <param name="url"></param>
    public void SetFlowUrl(string url = null)
    {
        FlowUrl = url;
    }

    /// <summary>
    /// 设置调度计划url
    /// </summary>
    /// <param name="url"></param>
    public void SetPlanUrl(string url = null)
    {
        PlanUrl = url;
    }

    /// <summary>
    /// 加载货物的协程
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator GetData(string url)
    {

        WWW getData = new WWW(url);
        yield return getData;
        try
        {
            if (getData.error != null)
            {
                Debug.Log(getData.error);
            }
            else
            {
                //Debug.Log(getData.text);
                var shelfs = JsonHelper.FromJson<Shelf>("{\"Items\": " + getData.text + "}");
                Shelves = shelfs;
                foreach (var shelf in shelfs)
                {
                    Dictionary<string, Good> dictionary = new Dictionary<string, Good>();
                    foreach (var floor in shelf.floors)
                    {
                        foreach (var cell in floor.cells)
                        {
                            if (cell.goods.Length > 0)
                            {
                                if (!ModelNameStrings.Contains(cell.goods[0].model_name))
                                {
                                    cell.goods[0].model_name = "default";
                                }
                                GameObject g = Resources.Load("Goods/" + cell.goods[0].model_name) as GameObject;
                                var c = GameObject.Find(string.Format("{0}-{1}-{2}", shelf.no, floor.no, cell.no)).transform;
                                var clone = Instantiate(g, c).transform;
                                if (GooDictionary.ContainsKey(clone.name))
                                {
                                    GooDictionary[clone.name].Add(clone.gameObject);
                                }
                                else
                                {
                                    GooDictionary[clone.name] = new List<GameObject>{
                                        clone.gameObject
                                    };
                                }
                                clone.Rotate(90, 0, 0);
                                if (shelf.type == 0)
                                {
                                    clone.localPosition = new Vector3(0, 0, 0.2f);
                                }
                            }
                            foreach (var good in cell.goods)
                            {
                                #region 货架字典

                                if (dictionary.ContainsKey(good.name))
                                {
                                    dictionary[good.name].num += good.num;
                                }
                                else
                                {
                                    dictionary[good.name] = new Good
                                    {
                                        id = 0,
                                        name = good.name,
                                        model_name = good.model_name,
                                        unit = good.unit,
                                        num = good.num
                                    };
                                }

                                #endregion

                                #region 总字典

                                if (TotalDictionary.ContainsKey(good.name))
                                {
                                    TotalDictionary[good.name].num += good.num;
                                }
                                else
                                {
                                    TotalDictionary[good.name] = new Good
                                    {
                                        id = 0,
                                        name = good.name,
                                        model_name = good.model_name,
                                        unit = good.unit,
                                        num = good.num
                                    };
                                }

                                #endregion
                            }


                        }
                    }
                    ShelfUIController shelfUiController =
                        GameObject.Find("Shelf" + shelf.no).GetComponentInChildren<ShelfUIController>();
                    shelfUiController.SetData(dictionary);
                    shelfUiController.ShelfNoLabel.text = shelf.no.ToString();
                }
                UiController uiController = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<UiController>();
                uiController.SetData(TotalDictionary);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
    }
}
