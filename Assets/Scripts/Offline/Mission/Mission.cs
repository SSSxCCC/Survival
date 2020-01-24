using System;
using UnityEngine;

namespace Offline
{
    /// <summary>
    /// 任务类，从头开始应该调用<see cref="Start"/>方法，从存档读取的任务应该调用<see cref="LoadFromData"/>方法。
    /// </summary>
    public abstract class Mission : ScriptableObject
    {
        public string missionKey;

        public LocalizationItem missionNameItem;
        public LocalizationItem missionDescriptionItem;

        /// <summary>
        /// 任务名
        /// </summary>
        public string missionName { get { return missionNameItem.GetText(); } }

        /// <summary>
        /// 本任务内容描述
        /// </summary>
        public string missionDescription { get { return missionDescriptionItem.GetText(); } }

        /// <summary>
        /// 对任务初始化，注册任务，显示任务描述，并从进度为0开始任务。
        /// </summary>
        public void Start()
        {
            Initialize();
            
            GameManager.singleton.RegisterMission(this);

            string text = missionName + "  " + missionDescription;
            GameManager.singleton.ShowText(new HintText(text, Color.blue, 3f));

            OnStart();
        }

        /// <summary>
        /// 将任务状态设置为初始状态（任务进度为0开始）。
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// 用于初始化引用或注册事件等内容。
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 任务条件满足时调用，显示任务完成，并结束任务。
        /// </summary>
        protected void Complete()
        {
            OnComplete();
            string text = missionName + "  " + GameManager.singleton.completeItem.GetText();
            GameManager.singleton.ShowText(new HintText(text, Color.green, 3f));
            GameManager.singleton.OnMissionComplete(this);
        }

        /// <summary>
        /// 用于回收一些内存或事件等内容。
        /// </summary>
        protected abstract void OnComplete();



        public Data SaveToData()
        {
            return new Data(missionKey, GetMissionData());
        }

        protected abstract string GetMissionData();

        /// <summary>
        /// 对任务初始化，注册任务，并从存档继续任务。
        /// </summary>
        public void LoadFromData(string missionData)
        {
            Initialize();

            GameManager.singleton.RegisterMission(this);

            SetMissionData(missionData);
        }

        /// <summary>
        /// 将任务状态设置为从存档读取的状态。
        /// </summary>
        /// <param name="missionData">从存档读取的此任务的保存内容</param>
        protected abstract void SetMissionData(string missionData);

        [Serializable]
        public class Data
        {
            public string missionKey;
            public string missionData;

            public Data(string missionKey, string missionData)
            {
                this.missionKey = missionKey;
                this.missionData = missionData;
            }
        }
    }
}