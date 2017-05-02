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
            foreach (var shelf in shelfs)
            {
                //Dictionary<(string,string),int> dictionary=new Dictionary<(), int>();
                foreach (var floor in shelf.floors)
                {
                    foreach (var cell in floor.cells)
                    {
                        var good = cell.good;
                        if (good == null) continue;
                        GameObject g = Resources.Load("Goods/" + good.model_name) as GameObject;
                        var c = GameObject.Find(string.Format("{0}-{1}-{2}", shelf.no, floor.no, cell.no)).transform;
                        var clone = Instantiate(g, c).transform;
                        clone.Rotate(90, 0, 0);
                        clone.localPosition = new Vector3(0, -0.4f, 0.7f);
                        
                    }
                }
            }
        }
    }
}
