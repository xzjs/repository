using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public class UiController : MonoBehaviour
    {
        public UILabel PerspectiveLabel, KeyLabel,TitleLabel;
        public UIGrid[] UiGrids;
        public GameObject DetaillView;

        private Dictionary<string, GoodInfo> _dictionary;

        public Shader RimLightShader;
        public Color RimColor = new Color(0.2f, 0.8f, 10.6f, 1);

        private MeshRenderer _mesh;
        private Color _color;
        private Shader _shader;
        private List<GameObject> _shelves;
        private Transform _lastClickTransform, _lastClickFloor, _lastClickCell;
        private GameController _gameController;

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
                foreach (var info in dictionary)
                {
                    GameObject dataItem;
                    if (index < 2)
                    {
                        dataItem = Resources.Load("UI/BigItem") as GameObject;
                    }
                    else
                    {
                        dataItem=Resources.Load("UI/Item") as GameObject;
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
            _gameController = GameObject.Find("GameObejct").GetComponent<GameController>();
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
                    if (_lastClickTransform != null)
                    {
                        foreach (Transform childTransform in _lastClickTransform)
                        {
                            _mesh = childTransform.gameObject.GetComponentInChildren<MeshRenderer>();
                            _mesh.material.shader = _shader;
                        }
                    }
                    Transform shelf = GameObject.Find(string.Format("Shelf{0}", g.name)).transform;
                    _lastClickTransform = shelf;
                    foreach (Transform childTransform in shelf)
                    {
                        _mesh = childTransform.gameObject.GetComponentInChildren<MeshRenderer>();

                        _shader = _mesh.material.shader;

                        _mesh.material.shader = RimLightShader;
                        _mesh.material.SetColor("_RimColor", RimColor);
                    }
                    break;
                case "Floor":
                    if (_lastClickFloor != null)
                    {
                        foreach (Transform cell in _lastClickFloor)
                        {
                            _mesh = cell.gameObject.GetComponent<MeshRenderer>();
                            _mesh.material.shader = _shader;
                        }
                    }
                    Transform floor = GameObject.Find(string.Format("Floor{0}", g.name)).transform;
                    _lastClickFloor = floor;
                    foreach (Transform cell in floor)
                    {
                        _mesh = cell.gameObject.GetComponent<MeshRenderer>();
                        _shader = _mesh.material.shader;
                        _mesh.material.shader = RimLightShader;
                        _mesh.material.SetColor("_RimColor", RimColor);
                    }
                    ShowDetail(g.name,g.tag);
                    break;
            }
        }

        /// <summary>
        /// 显示详细信息面板
        /// </summary>
        /// <param name="floor"></param>
        public void ShowDetail(string name,string tag)
        {
            DetaillView.gameObject.SetActive(true);
            TitleLabel.text = name;
            string[] strings = name.Split('-');
            Shelf[] shelves = _gameController.Shelves;
            if (tag == "floor")
            {
                int shelfIndex = Convert.ToInt32(strings[0]) - 1;
                int floorIndex = Convert.ToInt32(strings[1]) - 1;
                Cell[] cells = shelves[shelfIndex].floors[floorIndex].cells;
                Dictionary<string, GoodInfo> dictionary = new Dictionary<string, GoodInfo>();
                foreach (var cell in cells)
                {
                    Good good = cell.good;
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
                ShowData(2, dictionary);
            }
        }
    }
}
