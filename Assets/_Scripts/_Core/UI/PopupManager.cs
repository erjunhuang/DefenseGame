using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;
namespace Core.Popup
{
    public class PopupManager : ClassSingleton<PopupManager>
    {   
      public  class PopupStack
        {
            public GameObject gameObject;
            public object[] _uiParams;

            public PopupStack(GameObject gameObject, params object[] _uiParams) {
                this.gameObject = gameObject;
                this._uiParams = _uiParams;
            }
        }
        private Dictionary<Type, PopupStack> dicOpenUIs = null;
        public override void Init()
        {
            dicOpenUIs = new Dictionary<Type, PopupStack>();
        }

        public void addPopup(Type panel, params object[] _uiParams)
        {
            if (!dicOpenUIs.ContainsKey(panel))
            {
                string _path = UIPathDefines.UI_PREFAB + panel;
                GameObject _uiObject = MonoBehaviour.Instantiate(Resources.Load(_path)) as GameObject;
                PopupStack popupStack = new PopupStack(_uiObject, _uiParams);
                dicOpenUIs.Add(panel, popupStack);
            }
        }

        #region Close UI Type
        public void removeAllPopup()
        {
            List<Type> _keyList = new List<Type>(dicOpenUIs.Keys);
            foreach (Type gameObject in _keyList)
            {
                this.RemovePopup(gameObject);
            }
            dicOpenUIs.Clear();
        }

        public void RemovePopup(Type panel)
        {
            if (panel == null) return;
            if(dicOpenUIs.ContainsKey(panel))
            {
                PopupStack popupStack;
                if (dicOpenUIs.TryGetValue(panel, out popupStack))
                {
                    dicOpenUIs.Remove(panel);
                    GameObject.Destroy(popupStack.gameObject);
                    Debug.Log("删除成功");
                }
            }
        }
        #endregion
    }
}
