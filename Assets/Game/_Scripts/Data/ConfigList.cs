using QGame.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QGame.Core.Config
{
    public class ConfigList<T> : List<T> where T : ConfigBase
    {
        public T GetOne(int id)
        {
            return Find(x => x.Id == id);
        }

        public List<T> GetList(params int[] ids)
        {
            return FindAll(x => Array.Exists(ids, id => id == x.Id));
        }

        public Dictionary<int, T> GetDict(params int[] ids)
        {
            var list = new Dictionary<int, T>();

            foreach (var item in this)
            {
                if (Array.Exists(ids, id => id == item.Id))
                    list.Add(item.Id, item);

                if (ids.Length == list.Count)
                    break;
            }

            return list;
        }

        public Dictionary<int, T> GetDict(List<int> ids)
        {
            var list = new Dictionary<int, T>();

            foreach (var item in this)
            {
                if (ids.Exists(x => x.Equals(item.Id)))
                    list.Add(item.Id, item);

                if (ids.Count == list.Count)
                    break;
            }

            return list;
        }
    }
}
