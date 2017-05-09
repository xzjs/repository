using UnityEngine;

namespace Assets.Script
{
    public class RecordList : MonoBehaviour
    {
        public TweenPosition TweenPosition;
        public UIGrid Grid;
        public UILabel TotaLabel;
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void Close()
        {
            TweenPosition.PlayReverse();
            foreach (Transform cell in Grid.transform)
            {
                Destroy(cell.gameObject);
            }
        }
    }
}
