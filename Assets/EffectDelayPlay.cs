using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace QGame.Utils
{
   public class EffectDelayPlay : MonoBehaviour
    {

       public string EffectName;
        /// <summary>
        /// 粒子系统
        /// </summary>
        private List<ParticleSystem> m_listParticleSystem;

        /// <summary>
        /// 延迟生效的对象
        /// </summary>
        public List<DelayParticleSystem> DelayParticleSystems;
       
       /// <summary>
       /// 延迟删除的对象
       /// </summary>
        public List<DelayParticleSystem> DelayDestroyGameObject;

        private List<Animator> m_listAnimator;

        /// <summary>
        /// 是否忽略时间缩放
        /// </summary>
        public bool m_blIgnoreScaleTime = false;
        /// <summary>
        /// 从播放开始后的释放时间，0表示不释放
        /// </summary>
        public float m_destroyTime = 0;

        public Action onComplete;
        /// <summary>
        /// 特效本身
        /// </summary>
        [HideInInspector]
        public GameObject effect;
        void Awake()
        {
            Component[] cs = this.gameObject.GetComponentsInChildren(typeof(ParticleSystem));

            m_listParticleSystem = new List<ParticleSystem>();

            foreach (ParticleSystem ps in cs)
            {
                m_listParticleSystem.Add(ps);
            }
            Component[] anis = this.gameObject.GetComponentsInChildren(typeof(Animator));

            m_listAnimator = new List<Animator>();
            foreach (Animator ani in anis)
            {
                m_listAnimator.Add(ani);
            }
            effect = this.gameObject;
        }

        private float realDeltaTime;
       // private float timeLastFrame;

        void Start()
        {
           // timeLastFrame = Time.realtimeSinceStartup;
            realDeltaTime = Time.deltaTime;
            DelayParticleSystems.Sort(Sort);
            DelayDestroyGameObject.Sort(Sort);
            if (DelayParticleSystems != null && DelayParticleSystems.Count > 0)
            {
                for (int i = 0; i < DelayParticleSystems.Count; i++)
                {
                    DelayParticleSystem dps = DelayParticleSystems[i];
                    dps.go.SetActive(false);
                }      
                StartCoroutine(UpdateSelf());         
            }

            if (DelayDestroyGameObject != null && DelayDestroyGameObject.Count > 0)
            {
               
                StartCoroutine(UpdateDestroyList());
            }

            if (m_destroyTime > 0 )
            {
                StartCoroutine(UpdateDestroy());    
            }
            //if (m_blIgnoreScaleTime)
            //{
            //    foreach (Animator ani in m_listAnimator)
            //    {
            //        ani.updateMode = AnimatorUpdateMode.UnscaledTime;
            //    }
            //}

        }

        public int Sort(DelayParticleSystem p1, DelayParticleSystem p2)
        {
            return p1.delayTime<=p2.delayTime?-1:1;
        }

        float _currentTime = 0;
        IEnumerator UpdateSelf()
        {
            if (DelayParticleSystems!=null)
            for (int i = 0; i < DelayParticleSystems.Count; i++)
            {
                DelayParticleSystem dps = DelayParticleSystems[i];
                while (_currentTime < dps.delayTime)
                {                    
                    yield return 0;
                }
                dps.go.SetActive(true);               
            }
        }

        IEnumerator UpdateDestroyList()
        {
            if (DelayDestroyGameObject != null)
                for (int i = 0; i < DelayDestroyGameObject.Count; i++)
                {
                    DelayParticleSystem dps = DelayDestroyGameObject[i];
                    while (_currentTime < dps.delayTime)
                    {
                        yield return 0;
                    }   
                    if (dps!=null)
                    dps.go.SetActive(false);
                }
        }

        IEnumerator UpdateDestroy()
        {
            while ( _currentTime< m_destroyTime)
            {             
                yield return 0;               
            }     
            StopAllCoroutines();
            if (onComplete != null)
                onComplete();
            Destroy(gameObject);              
        }

        private void play()
        {
            this.gameObject.SetActive(true);
        }
        private float _timeAtLastFrame;
        private float _deltaTime;


        void Update()
        {
            //if (m_blIgnoreScaleTime)
            //{
            //    realDeltaTime = Time.realtimeSinceStartup - timeLastFrame;              
            //}
            //else
            if (_blKaping)
            {
                realDeltaTime = 0;
            }
            else
            {
                realDeltaTime = Time.deltaTime;               
            }
           // timeLastFrame = Time.realtimeSinceStartup;
            _currentTime += realDeltaTime;
            //if (m_blIgnoreScaleTime)
            //{
            //    if (Time.timeScale != 1)
            //    {
            //        if (m_listParticleSystem != null)
            //            foreach (ParticleSystem ps in m_listParticleSystem)
            //            {
            //                if (ps!=null && ps.gameObject.activeInHierarchy == true)
            //                {
            //                    ps.Simulate(realDeltaTime, false, false);
            //                    ps.Play();
            //                }
            //            }
            //    }
            //}
        }

       /// <summary>
       /// 取消忽略时间缩放
       /// </summary>
        private void setDontIgnoreScaleTime()
        {
            m_blIgnoreScaleTime = false;
            foreach (Animator ani in m_listAnimator)
            {
                ani.updateMode = AnimatorUpdateMode.Normal;
            }
        }
        private bool _blKaping = false;
        public void SetKaping(bool blKaping)
        {
            _blKaping = blKaping;
            foreach (Animator ani in m_listAnimator)
            {
                ani.speed =blKaping?0:1;
            }
            foreach (ParticleSystem ps in m_listParticleSystem)
            {
                if (blKaping)
                    ps.Pause();
                else
                    ps.Play();
            }
        }
   }


    [System.Serializable]
    public class DelayParticleSystem
    {
        public GameObject go;
        public float delayTime;
    }
}