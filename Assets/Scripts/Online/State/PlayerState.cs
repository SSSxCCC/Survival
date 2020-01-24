using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class PlayerState : State, IPlayerState
    {
        public Sprite[] maleFemaleSprites;

        [SyncVar(hook = "ChangeGender")] private int gender;

        private CommonPlayerState<PlayerState> commonPlayerState; // 公共实现

        protected override void Awake()
        {
            commonPlayerState = new CommonPlayerState<PlayerState>(this);
            base.Awake();
        }
        
        [Client]
        public override void OnStartLocalPlayer()
        {
            healthBar.color = Color.green; // 自己的血条变为绿色
            CmdChangeGender(OnlineGameSetting.gender); // 并且按照设置改变性别

            // 显示状态（生命值，防御力等）
            OnChangeHp();
            OnChangeDfs();
        }

        // 要服务器根据自己的设置改变性别
        [Command]
        private void CmdChangeGender(int gender) { this.gender = gender; }

        // 改变性别
        [Client]
        private void ChangeGender(int gender)
        {
            //this.gender = gender; 这里不需要这句话，因为即使是服务器改变，客户端会调用OnStartLocalPlayer，使得每个客户端显示的模型仍然正常
            spriteRenderer.sprite = maleFemaleSprites[gender];
        }

        // 为新来的玩家正常显示之前玩家的性别
        [Client]
        public override void OnStartClient()
        {
            base.OnStartClient();
            ChangeGender(gender);
        }

        // 空血的玩家处理
        [Server]
        protected override void OnEmptyHealth()
        {
            GetComponent<WeaponManager>().RemoveWeapons();
            GameManager.singleton.OnKilledPlayer();
        }

        // 生命值改变时调用
        [Client]
        protected override void OnChangeHp()
        {
            if (isLocalPlayer)
                commonPlayerState.OnChangeHealth();
        }

        // 防御力改变时调用
        [Client]
        protected override void OnChangeDfs()
        {
            if (isLocalPlayer)
                commonPlayerState.OnChangeDefense();
        }

        // 离开游戏的客户端要被判定为死亡
        [ServerCallback]
        public override void OnNetworkDestroy()
        {
            health = 0;
        }
    }
}