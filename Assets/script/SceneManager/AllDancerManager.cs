/*************************************************************
   Copyright(C) 2017 by dayugame
   All rights reserved.
   
   AllDancerManager.cs
   PartyRhythmGame
   
   Created by WuIslet on 2018-01-16.
   
*************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace isletspace
{
    /// <summary>
    /// 
    /// </summary>
    public class AllDancerManager : MonoBehaviour
    {
        public Transform allDancer;
        public Transform allTorch;

        private void Start()
        {
            GameManager.BeforeUpdateGamePhase += ChangePhase;
        }

        private void OnDestroy()
        {
            GameManager.BeforeUpdateGamePhase -= ChangePhase;
        }

        public void ChangePhase()
        {
            var torch = allTorch.GetChild(GameManager.gamePhase).gameObject;
            torch.SetActive(false);
            torch = allTorch.GetChild(GameManager.gamePhase + 1).gameObject;
            torch.SetActive(true);
        }

        public Vector3 CloseUp(int pos)
        {
            var target = allDancer.GetChild(pos);

            var ani = target.GetComponent<DancerAni>();
            ani.DoPose();

            return target.position;
        }

        public void PlayLightSpot(int pos)
        {
            var ani = allDancer.GetChild(pos).GetComponent<DancerAni>();
            ani.DoLightSpotMove();
        }
    }
}
