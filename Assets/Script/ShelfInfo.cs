using System;
using UnityEngine;

namespace Assets.Script
{
    [Serializable]
    public class Good
    {
        public int id;
        public string name;
        public string model_name;
        public string unit;
        public int num;
        public int cell_id;
    }

    [Serializable]
    public class Cell
    {
        public int id;
        public int no;
        public int floor_id;
        public Good[] goods;
    }

    [Serializable]
    public class Floor
    {
        public int id;
        public int no;
        public int shelf_id;
        public Cell[] cells;
    }

    [Serializable]
    public class Shelf
    {
        public int id;
        public int no;
        public int type;
        public Floor[] floors;
    }

    /// <summary>
    /// 记录类
    /// </summary>
    [Serializable]
    public class Record
    {
        public string name;
        public string action;
        public string time;
        public string num;
        public string unit;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
