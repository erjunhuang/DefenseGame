using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QGame.Core.Scene
{
    class SDKMain : MonoBehaviour
    {

        public void Awake()
        {
            if (SceneManager.currScene.sceneType==SceneType.Start)
            {
                Application.LoadLevel(0);
                return;
            }
            CreateView();
        }

        void CreateView()
        {
            SceneManager.currScene.CreateView();
        }
    }
}
