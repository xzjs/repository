using UnityEngine;

namespace Assets.Script
{
    public class test : MonoBehaviour
    {

        public UILabel Label;
        public void Click()
        {
            Application.ExternalCall("test","hello world");
        }

        public void Change(string str)
        {
            Label.text = str;
        }
    }
}
