﻿/*************************************************************
   Copyright(C) 2017 by dayugame
   All rights reserved.
   
   DancerAni.cs
   PartyRhythmGame
   
   Created by WuIslet on 2018-01-15.
   
*************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace isletspace
{
    /// <summary>
    /// 
    /// </summary>
    public class DancerAni : MonoBehaviour
    {
        Animator animator;
        Vector3 mid;
        Transform lightEffectPos;

        GuAniManager guManager;
        PoseLightManager poseLightManager;

        bool isUsedOneHand;

        #region 常数
        Vector3 target = new Vector3(-500, 0, 0);
        #endregion
        
        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            guManager = Utils.FindDirectChildComponent<GuAniManager>("gu", transform);
            poseLightManager = Utils.FindDirectChildComponent<PoseLightManager>("fff_Object028", transform);
            mid = (transform.position + target) / 2;
            lightEffectPos = transform.Find("LightEffect");
        }

        public void DoPose()
        {
            animator.SetTrigger("pose");
            int type = Random.Range(1, 7);
            animator.SetFloat("poseType", type);
        }

        public void DoDrum(int type, float time)
        {
            float rate = 1;
            if (time != 0 && time < 0.6f)
            {
                rate = 0.6f / time;
            }
            animator.speed = rate;

            animator.SetTrigger("drum");

            int realType = type;
            switch (type)
            {
                case 1:
                    realType = isUsedOneHand ? 1 : 2;
                    isUsedOneHand = !isUsedOneHand;
                    break;
                case 2:
                    realType = 3;
                    break;
                case 3:
                    realType = 4;
                    break;
                case 9:
                    realType = 5;
                    break;
            }
            animator.SetFloat("drumType", realType);
        }

        public void DoCheer(bool flag)
        {
            animator.SetBool("cheerFlag", flag);
            if (flag)
            {
                int type = Random.Range(1, 4);
                animator.SetFloat("cheerType", type);
            }
        }

        #region 光点飞行动画
        public void DoLightSpotMove(float startTime = 0, System.Action callback = null)
        {
            StartCoroutine(MoveLightSpot(20, 3f, startTime, callback));
        }

        private IEnumerator MoveLightSpot(int d, float time, float startTime, System.Action callback)
        {
            if (startTime > 0)
            {
                yield return new WaitForSeconds(startTime);
            }

            var obj = Pool.CreateObject("lightspot/lightspot", lightEffectPos);

            //move obj
            mid.y = Random.Range(5.0f, 10.0f);
            float timegap = time / d;
            for (int i = 0; i < d; ++i)
            {
                Vector3 point = BezierGenerate.CreatePoint(obj.transform.position, mid, target, (i + 1f) / d);
                obj.transform.DOMove(point, timegap).SetEase(Ease.Linear);
                yield return new WaitForSeconds(timegap);
            }
            
            if(callback != null)
            {
                callback();
            }

            Destroy(obj);
        }
        #endregion

        #region 片头动作
        public void PlayOP()
        {
            animator.SetTrigger("OP");
        }
        public void PlayOPPose()
        {
            animator.SetTrigger("OPpose");
        }
        public void PlayJoin()
        {
            animator.SetTrigger("startjoin");
        }
        public void PlayEndJoin()
        {
            animator.SetTrigger("endjoin");
        }
        public void DoYourTurn(bool flag)
        {
            if (flag)
            {
                Invoke("DelayGo", 0.25f);
            }
            animator.SetBool("EndLeading", flag);
        }

        private void DelayGo()
        {
            SoundManager.Instance.PlayGO();
        }
        #endregion

        #region 击鼓特效
        public void DoAddGuLight()
        {
            guManager.Begin();
        }

        public void DoDelGuLight()
        {
            guManager.End();
        }
        #endregion

        #region 特写特效
        public void DoAddPoseLight()
        {
            poseLightManager.Begin();;
        }

        public void DoDelPoseLight()
        {
            poseLightManager.End();
        }

        #endregion


        #region 自动序列击鼓
        public IEnumerator DrumRandom(int count, float totalTime)
        {
            for (int i = 0; i < count; ++i)
            {
                float gap = Random.Range(0.5f, 1f);
                int type = Random.Range(1, 4);
                if (Random.Range(0f, 1f) < 0.3f)
                {
                    DoDrum(type, gap);
                }
                yield return new WaitForSeconds(gap);
                totalTime -= gap;
                if(totalTime < 0)
                {
                    animator.speed = 1; //因为DoDrum会改变速度，结束时需要改回来。
                    yield break;
                }
            }
            animator.speed = 1; //因为DoDrum会改变速度，结束时需要改回来。
        }
        public IEnumerator DrumConstant(int type, int count, float gap)
        {
            for (int i = 0; i < count; ++i)
            {
                DoDrum(type, gap);
                yield return new WaitForSeconds(gap);
            }
            animator.speed = 1; //因为DoDrum会改变速度，结束时需要改回来。
        }

        public IEnumerator DrumList(List<int> beat, List<int> beattime, AddCallback callback = null)
        {
            int preTime = 0;
            for (int i = 0; i < beat.Count; ++i)
            {
                float gap = beattime[i] / 1000.0f;
                float drumDelayTime = gap * 0.7f;
                drumDelayTime = drumDelayTime > 0.3f ? 0.3f : drumDelayTime;
                yield return new WaitForSeconds(gap - drumDelayTime);
                DoDrum(beat[i], gap);
                yield return new WaitForSeconds(drumDelayTime);
                if(callback != null)
                {
                    callback(beat.Count - i, beat[i]);
                }
                preTime = beattime[i];
            }
            animator.speed = 1; //因为DoDrum会改变速度，结束时需要改回来。
        }

        public IEnumerator DrumSolo()
        {
            float preTime = 0;
            for (int i = 0; i < GameManager.soloList.Count; ++i)
            {
                float gap = GameManager.soloList[i] - preTime;
                yield return new WaitForSeconds(gap);
                int beat = ((i + 1) == GameManager.soloList.Count) ? 9 : 1; //特殊判断，最后一拍用type9。
                DoDrum(beat, gap);
                preTime = GameManager.soloList[i];
            }
            animator.speed = 1; //因为DoDrum会改变速度，结束时需要改回来。
        }
        #endregion
    }
}
