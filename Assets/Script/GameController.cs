using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using FirstPersonController = Assets.Script.FirstPersonController;

public class GameController : MonoBehaviour
{
    public GameObject MainCamera, FpsController;
    public Shelf[] Shelves;
    public Dictionary<string, List<GameObject>> GooDictionary;
    public TweenPosition TweenPosition;
    public Dictionary<string, Good> TotalDictionary;

    private bool is_control = true;
    // Use this for initialization
    void Start()
    {
        GooDictionary = new Dictionary<string, List<GameObject>>();
        TotalDictionary=new Dictionary<string, Good>();
        StartCoroutine(GetData("http://repository.xzjs.love/api/shelfs"));
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
            FpsController.SetActive(true);
        }
        else
        {
            MainCamera.SetActive(true); ;
            FpsController.SetActive(false);
        }
    }

    private IEnumerator GetData(string url)
    {

        WWW getData = new WWW("http://repository.xzjs.love/api/shelfs");
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
                var shelfs = JsonHelper.FromJson<Shelf>("{\"Items\":" + getData.text + "}");
                Shelves = shelfs;
                foreach (var shelf in shelfs)
                {
                    Dictionary<string, Good> dictionary = new Dictionary<string, Good>();
                    foreach (var floor in shelf.floors)
                    {
                        foreach (var cell in floor.cells)
                        {
                            var good = cell.good;
                            if (good == null) continue;
                            GameObject g = Resources.Load("Goods/" + good.model_name) as GameObject;
                            var c = GameObject.Find(string.Format("{0}-{1}-{2}", shelf.no, floor.no, cell.no)).transform;
                            var clone = Instantiate(g, c).transform;
                            if (GooDictionary.ContainsKey(clone.name))
                            {
                                GooDictionary[clone.name].Add(clone.gameObject);
                            }
                            else
                            {
                                GooDictionary[clone.name] = new List<GameObject>
                                {
                                    clone.gameObject
                                };
                            }
                            clone.Rotate(90, 0, 0);
                            if (shelf.type == 0)
                            {
                                clone.localPosition = new Vector3(0, 0, 0.2f);
                            }

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
