using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class ShelfUIController : MonoBehaviour
{

    public UILabel ShelfNoLabel;
    public UIGrid DataGrid;

    public void SetData(Dictionary<string,Good> dictionary)
    {
        foreach (var info in dictionary)
        {
            GameObject dataItem = Resources.Load("UI/TableItem") as GameObject;
            dataItem.GetComponent<DataItem>().SetGood(info.Value);
            DataGrid.gameObject.AddChild(dataItem);
        }
        DataGrid.repositionNow = true;
    }
}
