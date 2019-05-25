using System;
using AIBehavior;
using QGame.Core.Utils;
using QGame.Utils;
using UnityEngine;

namespace QGame.Core.FightEnegin.Damage
{
    public class FightDefin
    {
        public static int defaultLayer = 0;
        public static int skillLayer = 8;

        public static EffectDelayPlay LoadSkill(GameObject skillPrefab, AIBehaviors attachActor, eSkillDirection skillDirection = eSkillDirection.orignal, bool blContainToPlayerControl = false, bool blInSkillLay = true)//,float durationTime=0
        {
            if (attachActor == null)
            {
                Debug.LogErrorFormat(" load skill null:" + skillPrefab.name);
            }
            EffectDelayPlay edp = LoadEffect(skillPrefab);
            if (edp != null)
            {
                if (blContainToPlayerControl)
                {
                    edp.gameObject.transform.SetParent(attachActor.transform);
                    TransformIdentity(edp.gameObject.transform);
                }
                else
                {
                    edp.transform.position = attachActor.transform.position;
                    if (skillDirection == eSkillDirection.self)
                    {
                        edp.transform.rotation = attachActor.transform.rotation;
                    }
                }
                if (blInSkillLay)
                    GameObjectUtils.setChildLayer(edp.gameObject, skillLayer);

            }

            return edp;
        }

        public static EffectDelayPlay LoadEffect(GameObject resPrefab)
        {
            if (resPrefab != null)
            {
                GameObject resEffect = GameObject.Instantiate(resPrefab);
                TransformIdentity(resEffect.transform);
                EffectDelayPlay edp = resEffect.GetComponent<EffectDelayPlay>();
                return edp;
            }
            else
            {
                Debug.LogError(" LoadEffect null");
                return null;
            }
        }

        public static EffectDelayPlay LoadEffect(GameObject resPrefab, Vector3 vPos)
        {
            if (resPrefab != null)
            {
                GameObject resEffect = (GameObject)GameObject.Instantiate(resPrefab, vPos, new Quaternion());
                EffectDelayPlay edp = resEffect.GetComponent<EffectDelayPlay>();
                return edp;
            }
            else
            {
                Debug.LogError(" LoadEffect null");
                return null;
            }
        }

        public static void TransformIdentity(Transform t)
        {
            t.localPosition = new Vector3(0, 0, 0);
            t.localRotation = new Quaternion();
            t.localScale = new Vector3(1, 1, 1);
        }
    }
}