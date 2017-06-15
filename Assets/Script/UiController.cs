using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.AI;

public class UiController : MonoBehaviour
{
    public UILabel PerspectiveLabel, KeyLabel, TitleLabel;
    public UIGrid[] UiGrids;
    public GameObject DetaillView, PlanList, ShowSimilar, Summary;
    public TweenPosition[] TweenPositions;
    public GodPerspective GodPerspective;
    public UISlider[] Sliders;
    public GameController GameController;
    public UIToggle UiToggle;
    public UISprite FrontSight;
    public Detail Detail;
    public Transform FirstCameraTransform;
    public Texture2D MouseTexture2D, FootTexture2D, DefaultTexture2D;

    private Dictionary<string, Good> _dictionary;

    public Shader RimLightShader, StandShader;
    public Color RimColor = new Color(0.2f, 0.8f, 10.6f, 1);

    private MeshRenderer _mesh;
    private Shader _shader;
    private Transform _lastClickTransform, _lastClickFloor;
    private GameObject _lastClickCell, _lastClickGood;
    private List<GameObject> _showGoods;
    private string _mouseStatus;
    private NavMeshAgent _navMeshAgent;

    /// <summary>
    /// 显示/关闭物资汇总窗口
    /// </summary>
    public void ShowSummary()
    {
        Summary.SetActive(!Summary.activeSelf);
    }
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
    public void ShowData(int index, Dictionary<string, Good> dictionary)
    {
        try
        {
            foreach (Transform cell in UiGrids[index].transform)
            {
                Destroy(cell.gameObject);
            }
            foreach (var info in dictionary)
            {
                GameObject dataItem;
                if (index < 2)
                {
                    dataItem = Resources.Load("UI/BigItem") as GameObject;
                }
                else
                {
                    dataItem = Resources.Load("UI/Item") as GameObject;
                }
                dataItem.GetComponent<DataItem>().SetGood(info.Value);
                UiGrids[index].gameObject.AddChild(dataItem);
            }
            UiGrids[index].repositionNow = true;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
        }
    }

    /// <summary>
    /// 搜索
    /// </summary>
    public void Search()
    {
        Dictionary<string, Good> dictionary = new Dictionary<string, Good>();
        foreach (var info in _dictionary)
        {
            if (info.Key.Contains(KeyLabel.text))
            {
                dictionary[info.Key] = info.Value;
            }
        }
        ShowData(1, dictionary);
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="dictionary">数据</param>
    public void SetData(Dictionary<string, Good> dictionary)
    {
        _dictionary = dictionary;
        ShowData(0, dictionary);
    }

    void Start()
    {
        _lastClickTransform = null;
        _showGoods = new List<GameObject>();
        Cursor.SetCursor(DefaultTexture2D, Vector2.zero, CursorMode.ForceSoftware);
        _navMeshAgent = FirstCameraTransform.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (UICamera.Raycast(Input.mousePosition)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        #region 更换准星图标

        if (Physics.Raycast(ray, out hit))
        {
            GameObject g = hit.transform.gameObject;
            if (g.tag == "Shelf" || g.tag == "Floor" || g.tag == "Cell" || g.tag == "Good")
            {
                if (_mouseStatus != "hand")
                {
                    _mouseStatus = "hand";
                    Cursor.SetCursor(MouseTexture2D, Vector2.zero, CursorMode.ForceSoftware);
                }
            }
            else if (g.tag == "Plane")
            {
                if (_mouseStatus != "foot")
                {
                    _mouseStatus = "foot";
                    Cursor.SetCursor(FootTexture2D, Vector2.zero, CursorMode.ForceSoftware);
                }
            }
            else
            {
                if (_mouseStatus != "default")
                {
                    _mouseStatus = "default";
                    Cursor.SetCursor(DefaultTexture2D, Vector2.zero, CursorMode.ForceSoftware);
                }
            }
        }
        else
        {
            if (_mouseStatus != "default")
            {
                _mouseStatus = "default";
                Cursor.SetCursor(DefaultTexture2D, Vector2.zero, CursorMode.ForceSoftware);
            }
        }

        #endregion

        #region 取消目标点

        if (_navMeshAgent.enabled)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                _navMeshAgent.enabled = false;
            }
        }
        #endregion

        if (!Input.GetButtonDown("Fire1")) return;
        CancelHighlight();
        if (Physics.Raycast(ray, out hit))
        {
            GameObject g = hit.transform.gameObject;
            Debug.Log(g.name);
            Debug.Log(g.tag);
            switch (g.tag)
            {
                case "Shelf":
                    Transform shelf = GameObject.Find(string.Format("Shelf{0}", g.name)).transform;
                    _lastClickTransform = shelf;
                    foreach (Transform childTransform in shelf)
                    {
                        ChangeShader(childTransform.gameObject, RimLightShader, true);
                    }
                    break;
                case "Floor":
                    Transform floor = GameObject.Find(string.Format("Floor{0}", g.name)).transform;
                    _lastClickFloor = floor;
                    foreach (Transform _cell in floor)
                    {
                        ChangeShader(_cell.gameObject, RimLightShader, true);
                    }
                    //ShowDetail(g.name, g.tag);
                    MyClick(g.name, g.tag);
                    break;
                case "Cell":
                    GameObject cell = GameObject.Find(g.name);
                    _lastClickCell = cell;
                    ChangeShader(cell, RimLightShader, true);
                    //ShowDetail(g.name, g.tag);
                    MyClick(g.name, g.tag);
                    break;
                case "Good":
                    _lastClickGood = g;
                    ChangeShader(g, RimLightShader, true);
                    //ShowDetail(g.transform.parent.name, "Cell");
                    MyClick(g.transform.parent.name, "Cell");
                    break;
                case "Plane":
                    Vector3 targetPos = hit.point;
                    _navMeshAgent.enabled = true;
                    _navMeshAgent.destination = targetPos;
             
                    
                    break;

            }
        }
    }

    /// <summary>
    /// 显示详细信息
    /// </summary>
    /// <param name="clickName"></param>
    /// <param name="clickTag"></param>
    public void ShowDetail(string clickName, string clickTag)
    {
        TweenPositions[0].PlayForward();
        TitleLabel.text = clickName;
        string[] strings = clickName.Split('-');
        int shelfIndex = Convert.ToInt32(strings[0]) - 1;
        int floorIndex = Convert.ToInt32(strings[1]) - 1;
        Dictionary<string, Good> dictionary = new Dictionary<string, Good>();
        //Good good;
        switch (clickTag)
        {
            case "Floor":
                Cell[] cells = GameController.Shelves[shelfIndex].floors[floorIndex].cells;

                foreach (var _cell in cells)
                {
                    foreach (var good in _cell.goods)
                    {
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
                                unit = good.unit
                            };
                        }
                    }
                }
                break;
            case "Cell":
                if (_lastClickGood != null)
                {
                    ShowSimilar.SetActive(true);
                }
                int cellIndex = Convert.ToInt32(strings[2]) - 1;
                Cell cell = GameController.Shelves[shelfIndex].floors[floorIndex].cells[cellIndex];
                foreach (var good in cell.goods)
                {
                    dictionary[good.name] = good;
                    Detail.SetData(good);
                }
                break;
        }
        ShowData(2, dictionary);
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    public void CloseDetail()
    {
        TweenPositions[0].PlayReverse();
        if (ShowSimilar.activeSelf)
        {
            ShowSimilar.SetActive(false);
        }
    }

    /// <summary>
    /// 显示同类货物
    /// </summary>
    /// <param name="goodName">货物名称</param>
    public void ShowSimilarGood(string goodName = null)
    {
        if (goodName == null)
        {
            goodName = _lastClickGood.name;
        }
        Dictionary<string, List<GameObject>> dictionary = GameController.GetComponent<GameController>().GooDictionary;
        if (!dictionary.ContainsKey(goodName)) return;
        _showGoods = dictionary[goodName];
        foreach (var o in dictionary[goodName])
        {
            ChangeShader(o, RimLightShader, true);
        }
    }

    /// <summary>
    /// 变更shader
    /// </summary>
    /// <param name="o"></param>
    /// <param name="shader"></param>
    /// <param name="color"></param>
    public void ChangeShader(GameObject o, Shader shader, bool color = false)
    {
        _mesh = o.GetComponent<MeshRenderer>();
        if (_mesh == null) return;
        Material[] materials = _mesh.materials;
        foreach (var material in materials)
        {
            material.shader = shader;
            if (color)
            {
                material.SetColor("_RimColor", RimColor);
            }
        }
    }

    /// <summary>
    /// 调度计划按钮点击事件
    /// </summary>
    public void PlanListButtonClick()
    {
        //PlanList.SetActive(!PlanList.activeSelf);
        if (PlanList.activeSelf)
        {
            PlanList.SetActive(false);
        }
        else
        {
            PlanList.SetActive(true);
            StartCoroutine(LoadPlan());
        }
    }

    /// <summary>
    /// 修改配置
    /// </summary>
    public void Config()
    {
        FirstCameraTransform.GetComponent<NavMeshAgent>().speed = Sliders[0].value * 7f;
        FirstCameraTransform.GetComponent<MyFirstPersonController>().speed = Sliders[2].value * 4;
        GodPerspective.SensitivetyMouseWheel = Sliders[1].value * 20;
        if (UiToggle.value)
        {
            GameController.GetComponent<AudioSource>().Play();
        }
        else
        {
            GameController.GetComponent<AudioSource>().Stop();
        }
        CloseMenu();
    }

    /// <summary>
    /// 关闭配置菜单
    /// </summary>
    public void CloseMenu()
    {
        TweenPositions[1].PlayReverse();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Close()
    {
        Application.Quit();
    }

    /// <summary>
    /// 取消高亮
    /// </summary>
    public void CancelHighlight()
    {
        if (_lastClickTransform != null)
        {
            foreach (Transform childTransform in _lastClickTransform)
            {
                ChangeShader(childTransform.gameObject, StandShader);
            }
        }
        if (_lastClickFloor != null)
        {
            foreach (Transform cell in _lastClickFloor)
            {
                ChangeShader(cell.gameObject, StandShader);
            }
        }
        if (_lastClickCell != null)
        {
            ChangeShader(_lastClickCell, StandShader);
        }
        if (_lastClickGood != null)
        {
            ChangeShader(_lastClickGood.gameObject, StandShader);
        }
        if (_showGoods.Count > 0)
        {
            foreach (var good in _showGoods)
            {
                ChangeShader(good, StandShader);
            }
        }
    }

    public void MyClick(string name, string tag)
    {
        if (Application.isEditor)
        {
            ShowDetail(name, tag);
        }
        else
        {
            string[] strings = name.Split('-');
            int shelfIndex = Convert.ToInt32(strings[0]) - 1;
            int floorIndex = Convert.ToInt32(strings[1]) - 1;
            Dictionary<string, Good> dictionary = new Dictionary<string, Good>();
            string json = null;
            switch (tag)
            {
                case "Floor":
                    Floor floor = GameController.Shelves[shelfIndex].floors[floorIndex];
                    json = JsonUtility.ToJson(floor);
                    break;
                case "Cell":
                    int cellIndex = Convert.ToInt32(strings[2]) - 1;
                    Cell cell = GameController.Shelves[shelfIndex].floors[floorIndex].cells[cellIndex];
                    json = JsonUtility.ToJson(cell);
                    break;
            }
            Application.ExternalCall("ClickCallback", name, tag, json);
        }
    }

    public void GameShow(string name_tag)
    {
        string[] ss = name_tag.Split('_');
        ShowDetail(ss[0], ss[1]);
    }

    public void ShowMenu()
    {
        TweenPositions[1].PlayForward();
    }

    private IEnumerator LoadPlan()
    {
        WWW getData = new WWW(GameController.PlanUrl);
        yield return getData;

        if (getData.error != null)
        {
            Debug.Log(getData.error);
        }
        else
        {
            Debug.Log(getData.text);
            UIGrid grid = PlanList.GetComponentInChildren<UIGrid>();
            foreach (Transform cell in grid.transform)
            {
                Destroy(cell.gameObject);
            }
            string[] plans = JsonHelper.FromJson<string>("{\"Items\":" + getData.text + "}");
            foreach (var plan in plans)
            {
                GameObject labelGameObject = Resources.Load("UI/Label") as GameObject;
                labelGameObject.GetComponent<UILabel>().text = plan;
                grid.gameObject.AddChild(labelGameObject);
            }
            grid.repositionNow = true;
        }
    }
}
