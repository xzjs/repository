﻿using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class ShelfUIController : MonoBehaviour
{

    public UILabel ShelfNoLabel;
    public UIGrid DataGrid;

    public void SetData(Dictionary<string,GoodInfo> dictionary)
    {
        foreach (var info in dictionary)
        {
            GameObject dataItem = Resources.Load("UI/TableItem") as GameObject;
            dataItem.GetComponent<DataItem>().NameLabel.text = info.Key;
            dataItem.GetComponent<DataItem>().NumLabel.text = info.Value.num.ToString();
            dataItem.GetComponent<DataItem>().UnitLabel.text = info.Value.unit;
            DataGrid.gameObject.AddChild(dataItem);
        }
        DataGrid.repositionNow = true;
    }
}
