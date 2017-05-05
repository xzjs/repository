using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public class UiController : MonoBehaviour
    {
        public UILabel PerspectiveLabel, KeyLabel, TitleLabel;
        public UIGrid[] UiGrids;
        public GameObject DetaillView, GameController, PlanList, FpsController, MainCarema;
        public UIButton ShowSimilarGoodsButton;
        public TweenPosition[] TweenPositions;

        private Dictionary<string, GoodInfo> _dictionary;

        public Shader RimLightShader, StandShader;
        public Color RimColor = new Color(0.2f, 0.8f, 10.6f, 1);

        private MeshRenderer _mesh;
        private Shader _shader;
        private Transform _lastClickTransform, _lastClickFloor;
        private GameObject _lastClickCell, _lastClickGood;
        private List<GameObject> _showGoods;

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
                    dataItem.GetComponent<DataItem>().NameLabel.text = info.Key;
                    dataItem.GetComponent<DataItem>().NumLabel.text = info.Value.num.ToString();
                    dataItem.GetComponent<DataItem>().UnitLabel.text = info.Value.unit;
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
            Dictionary<string, GoodInfo> dictionary = new Dictionary<string, GoodInfo>();
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
        public void SetData(Dictionary<string, GoodInfo> dictionary)
        {
            _dictionary = dictionary;
            ShowData(0, dictionary);
        }

        void Start()
        {
            _lastClickTransform = null;
            _showGoods = new List<GameObject>();
        }

        void Update()
        {
            if (!Input.GetButtonDown("Fire1")) return;
            if (UICamera.Raycast(Input.mousePosition)) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return;
            GameObject g = hit.transform.gameObject;
            Debug.Log(g.name);
            Debug.Log(g.tag);
            switch (g.tag)
            {
                case "Shelf":
                    //清除之前的shader
                    if (_lastClickTransform != null)
                    {
                        foreach (Transform childTransform in _lastClickTransform)
                        {
                            ChangeShader(childTransform.gameObject, StandShader);
                        }
                    }
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
                    ShowDetail(g.name, g.tag);
                    break;
                case "Cell":
                    GameObject cell = GameObject.Find(g.name);
                    _lastClickCell = cell;
                    ChangeShader(cell, RimLightShader, true);
                    ShowDetail(g.name, g.tag);
                    break;
                case "Good":
                    _lastClickGood = g;
                    ChangeShader(g, RimLightShader, true);
                    ShowDetail(g.transform.parent.name, "Cell");
                    break;
            }
        }

        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="clickName"></param>
        /// <param name="clickTag"></param>
        public void ShowDetail(string clickName, string clickTag)
        {
            //DetaillView.gameObject.SetActive(true);
            TweenPositions[0].PlayForward();
            TitleLabel.text = clickName;
            string[] strings = clickName.Split('-');
            GameController gameController = GameController.GetComponent<GameController>();
            int shelfIndex = Convert.ToInt32(strings[0]) - 1;
            int floorIndex = Convert.ToInt32(strings[1]) - 1;
            Dictionary<string, GoodInfo> dictionary = new Dictionary<string, GoodInfo>();
            Good good;
            switch (clickTag)
            {
                case "Floor":
                    Cell[] cells = gameController.Shelves[shelfIndex].floors[floorIndex].cells;

                    foreach (var _cell in cells)
                    {
                        good = _cell.good;
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
                    }
                    break;
                case "Cell":
                    ShowSimilarGoodsButton.gameObject.SetActive(true);
                    int cellIndex = Convert.ToInt32(strings[2]) - 1;
                    Cell cell = gameController.Shelves[shelfIndex].floors[floorIndex].cells[cellIndex];
                    good = cell.good;
                    dictionary[good.name] = new GoodInfo
                    {
                        num = good.num,
                        unit = good.unit
                    };
                    break;
            }
            ShowData(2, dictionary);
            gameController.ShowMenu();
        }

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void CloseMenu()
        {
            //DetaillView.gameObject.SetActive(false);
            TweenPositions[0].PlayReverse();
            GameController gameController = GameController.GetComponent<GameController>();
            gameController.ShowMenu();
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
            if (ShowSimilarGoodsButton.gameObject.activeSelf)
            {
                ShowSimilarGoodsButton.gameObject.SetActive(false);
            }
        }

        public void ShowSimilarGood()
        {
            if (_showGoods.Count > 0)
            {
                foreach (var good in _showGoods)
                {
                    ChangeShader(good, StandShader);
                }
            }
            Dictionary<string, List<GameObject>> dictionary =
                GameController.GetComponent<GameController>().GooDictionary;
            if (!dictionary.ContainsKey(_lastClickGood.name)) return;
            _showGoods = dictionary[_lastClickGood.name];
            foreach (var o in dictionary[_lastClickGood.name])
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

        public void PlanListButtonClick()
        {
            if (PlanList.activeSelf)
            {
                PlanList.SetActive(false);
                GameController.GetComponent<GameController>().ShowMenu();
            }
            else
            {
                PlanList.SetActive(true);
            }
        }
    }
}
