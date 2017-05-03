using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour
{
    public GameObject FpsController, MainCamera;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(GetData("http://repository.xzjs.love/api/shelfs"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            FirstPersonController first = FpsController.GetComponent<FirstPersonController>();
            first.enabled = !first.enabled;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
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
        if (getData.error != null)
        {
            Debug.Log(getData.error);
        }
        else
        {
            Debug.Log(getData.text);
            var shelfs = JsonHelper.FromJson<Shelf>("{\"Items\":" + getData.text + "}");
            Dictionary<string, GoodInfo> totalDictionary = new Dictionary<string, GoodInfo>();
            foreach (var shelf in shelfs)
            {
                Dictionary<string, GoodInfo> dictionary = new Dictionary<string, GoodInfo>();
                foreach (var floor in shelf.floors)
                {
                    foreach (var cell in floor.cells)
                    {
                        var good = cell.good;
                        if (good == null) continue;
                        GameObject g = Resources.Load("Goods/" + good.model_name) as GameObject;
                        var c = GameObject.Find(string.Format("{0}-{1}-{2}", shelf.no, floor.no, cell.no)).transform;
                        var clone = Instantiate(g, c).transform;
                        var temp = clone.localScale.y;
                        clone.Rotate(90, 0, 0);
                        //clone.Translate(Vector3.up * 0.5f * clone.localScale.y, Space.Self);
                        clone.localPosition = new Vector3(0, 0, 0.2f);

                        #region 货架字典

                        if (dictionary.ContainsKey(good.name))
                        {
                            dictionary[good.name].num += good.num;
                        }
                        else
                        {
                            dictionary[good.name] = new GoodInfo
                            {
                                num = good.num,
                                unit = good.unit
                            };
                        }

                        #endregion

                        #region 总字典

                        if (totalDictionary.ContainsKey(good.name))
                        {
                            totalDictionary[good.name].num += good.num;
                        }
                        else
                        {
                            totalDictionary[good.name] = new GoodInfo
                            {
                                num = good.num,
                                unit = good.unit
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
            uiController.SetData(totalDictionary);
        }
    }
}
