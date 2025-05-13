
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.Runtime
{
    public class ListEx<T> where T : MonoBehaviour
    {
        public readonly List<T> self = new();

        private int _currentCount;

        public void UpdateItem<TM>(List<TM> data, GameObject goCache, Transform parent)
        {
            if (self.Count >= data.Count)
            {
                for (var i = data.Count; i < self.Count; i++)
                {
                    self[i].gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (var item in data.Select(d => Object.Instantiate(goCache, parent, false)).Select(go => go.GetComponent<T>()))
                {
                    self.Add(item);
                }
            }

            _currentCount = data.Count;
        }

        public List<T> GetActiveList()
        {
            var activeList = new List<T>();
            for (var i = 0; i < _currentCount; i++)
            {
                activeList.Add(self[i]);
            }
            return activeList;
        }
    }
}
