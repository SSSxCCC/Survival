using UnityEngine;

namespace Offline
{
    public class PlayerState : State, IPlayerState
    {
        private CommonPlayerState<PlayerState> commonPlayerState; // 公共实现

        protected override void Awake()
        {
            commonPlayerState = new CommonPlayerState<PlayerState>(this);
            base.Awake();
        }

        // 空血的玩家处理
        protected override void OnEmptyHealth()
        {
            GameManager.singleton.OnPlayerDeath();
        }

        // 生命值改变时调用
        protected override void OnChangeHp() { commonPlayerState.OnChangeHealth(); }

        // 防御力改变时调用
        protected override void OnChangeDfs() { commonPlayerState.OnChangeDefense(); }
    }
}