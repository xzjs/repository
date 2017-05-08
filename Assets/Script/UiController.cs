﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Script
{
    public class UiController : MonoBehaviour
    {
        public UILabel PerspectiveLabel, KeyLabel, TitleLabel;
        public UIGrid[] UiGrids;
        public GameObject DetaillView, PlanList;
        public UIButton ShowSimilarGoodsButton;
        public TweenPosition[] TweenPositions;
        public FirstPersonController First;
        public GodPerspective GodPerspective;
        public UISlider[] Sliders;
        public GameController GameController;
        public UIToggle UiToggle;
        public UISprite FrontSight;

        private Dictionary<string, Good> _dictionary;

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
                    if (FrontSight.spriteName != "CanClick")
                    {
                        FrontSight.spriteName = "CanClick";
                    }
                }
                else
                {
                    if (FrontSight.spriteName != "Click")
                    {
                        FrontSight.spriteName = "Click";
                    }
                }
            }
            else
            {
                if (FrontSight.spriteName != "Click")
                {
                    FrontSight.spriteName = "Click";
                }
            }

            #endregion

            if (!Input.GetButtonDown("Fire1")) return;

            #region 取消高亮

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

            #endregion

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
            Good good;
            switch (clickTag)
            {
                case "Floor":
                    Cell[] cells = GameController.Shelves[shelfIndex].floors[floorIndex].cells;

                    foreach (var _cell in cells)
                    {
                        good = _cell.good;
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
                    break;
                case "Cell":
                    if (_lastClickGood != null)
                    {
                        ShowSimilarGoodsButton.gameObject.SetActive(true);
                    }
                    int cellIndex = Convert.ToInt32(strings[2]) - 1;
                    Cell cell = GameController.Shelves[shelfIndex].floors[floorIndex].cells[cellIndex];
                    good = cell.good;
                    dictionary[good.name] = good;
                    break;
            }
            ShowData(2, dictionary);
            GameController.TakeOrReleaseController();
        }

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void CloseDetail()
        {
            TweenPositions[0].PlayReverse();
            GameController.TakeOrReleaseController();
            if (ShowSimilarGoodsButton.gameObject.activeSelf)
            {
                ShowSimilarGoodsButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 显示同类货物
        /// </summary>
        /// <param name="goodName">货物名称</param>
        public void ShowSimilarGood(string goodName=null)
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

        public void PlanListButtonClick()
        {
            if (PlanList.activeSelf)
            {
                PlanList.SetActive(false);
                GameController.TakeOrReleaseController();
            }
            else
            {
                PlanList.SetActive(true);
            }
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        public void Config()
        {
            First.m_WalkSpeed = Sliders[0].value * 10;
            First.m_MouseLook.XSensitivity = Sliders[2].value * 4;
            First.m_MouseLook.YSensitivity = Sliders[2].value * 4;
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
            GameController.TakeOrReleaseController();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void Close()
        {
            Application.Quit();
        }
    }
}
