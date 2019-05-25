using System.Collections;
using UnityEngine;
namespace Core.Popup
{
    public abstract class Panel : MonoBehaviour
    {
        #region status
        protected EnumObjectState _state = EnumObjectState.None;
        public event StateChangeEvent StateChanged;
        public EnumObjectState State
        {
            protected get
            {
                return this._state;
            }
            set
            {
                if (this._state == value)
                {
                    return;
                }
                EnumObjectState oldState = this._state;
                this._state = value;
                if (null != StateChanged)
                {
                    StateChanged(this, this._state, oldState);  //调用事件
                }
            }
        }
        #endregion
        //Awake
        void Awake()
        {
            this.State = EnumObjectState.Initial;
            OnAwake();
        }
        protected virtual void OnAwake()
        {
            this.State = EnumObjectState.Loading;
            this.OnPlayOpenUIAudio();    
        }
        protected virtual void OnPlayOpenUIAudio() { }

        // Start
        void Start()
        {
            OnStart();
        }
        protected virtual void OnStart() { }

        // Update
        void Update()
        {
            if (this._state == EnumObjectState.Ready)
            {
                OnUpdate(Time.deltaTime);
            }
        }
        protected virtual void OnUpdate(float delatTime) { }

        // Release
        public void Close()
        {
            this.State = EnumObjectState.Closing;
            PopupManager.Instance.RemovePopup(this.GetType());
            OnClose();
        }
        protected virtual void OnClose()
        {
            this.State = EnumObjectState.None;
            this.OnPlayCloseUIAudio();
        }
        protected virtual void OnPlayCloseUIAudio() { }

        // OnDestroy
        void OnDestroy()
        {
            this.State = EnumObjectState.None;
        }

        //得到传进来的参数 显示界面
        public void showPanel_(params object[] uiParams)
        {
            this.State = EnumObjectState.Loading;
            PopupManager.Instance.addPopup(this.GetType(), uiParams);
            //异步加载
            CoroutineController.Instance.StartCoroutine(AsyncOnLoadData());
        }


        private IEnumerator AsyncOnLoadData()
        {
            yield return new WaitForSeconds(0);
            if (this.State == EnumObjectState.Loading)
            {
                this.OnLoadData();
                this.State = EnumObjectState.Ready;
            }
        }
        protected virtual void OnLoadData() { }
    }
}
