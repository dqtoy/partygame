﻿/*************************************************************
   Copyright(C) 2017 by dayugame
   All rights reserved.
   
   SettlePanel.cs
   PartyRhythmGame
   
   Created by WuIslet on 2018-01-17.
   
*************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace isletspace
{
    /// <summary>
    /// 
    /// </summary>
    public class SettlePanel : PanelBase
    {
        public AllDancerManager SceneManager;
        public CloseUpUI closeUp;

        private Vector3 target;

        public override void DoStart(UIManager manager)
        {
            base.DoStart(manager);
            SceneManager.AllDrumConstant();
            DoCloseUp();
        }

        public void DoCloseUp()
        {
            target = SceneManager.CloseUp(GameManager.GetMVPTarget());
            Director.Instance.RegistChangeCamera("AllDancerScene", OnAllDancerCloseUp);
            Invoke("DelayShowCloseUp", 0.5f);
        }

        public void DelayShowCloseUp()
        {
            //closeUp.Show(); //TODO 需要界面延迟显示的时候使用
        }

        private void OnAllDancerCloseUp(SceneCamera camera)
        {
            camera.AllDancerCloseUp(target);
            Director.Instance.UnRegistChangeCamera(OnAllDancerCloseUp);
        }
    }
}
