using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public abstract class State : NetworkBehaviour {
    [SerializeField] [SyncVar(hook = "OnChangeMaxHealth")] int m_MaxHealth = 1; // 最大生命值
    public int maxHealth { get { return m_MaxHealth; }
        set {
            m_MaxHealth = value;
            if (m_MaxHealth < health) health = m_MaxHealth;
        }
    }

    [SerializeField] [SyncVar(hook = "OnChangeHealth")] int m_Health = 1; // 当前生命值
    public int health { get { return m_Health; }
        set {
            m_Health = Mathf.Min(value, maxHealth); // 阻止当前生命值超过最大生命值
            if (isServer && m_Health <= 0) OnEmptyHealth();
        }
    }

    [SerializeField] [SyncVar(hook = "OnChangeDefense")] int m_Defense; // 防御力
    public int defense { get { return m_Defense; } set { m_Defense = value; } }

    [SerializeField] bool m_CanBeStunned; // 可被击晕的
    public bool canBeStunned { get { return m_CanBeStunned; } set { m_CanBeStunned = value; } }

    [SyncVar] float m_Stun; // 眩晕时间
    public float stun { get { return m_Stun; }
        set {
            if (canBeStunned)
                m_Stun = value > 0 ? value : 0;
        }
    }

    [SerializeField] Group m_Group; // 所属的组
    public Group group { get { return m_Group; } set { m_Group = value; } }

    [SerializeField] SpriteRenderer m_SpriteRenderer; // 渲染器，用于死亡染色
    public SpriteRenderer spriteRenderer { get { return m_SpriteRenderer; } set { m_SpriteRenderer = value; } }

    [SerializeField] RawImage m_HealthBar; // 血管
    public RawImage healthBar { get { return m_HealthBar; } set { m_HealthBar = value; } }

    [SerializeField] GameObject m_DamagedEffectPrefab; // 受伤特效
    public GameObject damagedEffectPrefab { get { return m_DamagedEffectPrefab; } set { m_DamagedEffectPrefab = value; } }

    [SyncVar(hook = "OnChangeLayer")] int m_Layer = -1; // 单位所在物理层
    public int layer { get { return m_Layer >= 0 ? m_Layer : gameObject.layer; } set { m_Layer = value; } }

    protected virtual void Awake() {
        health = maxHealth; // 当前生命值等于最大生命值
    }

    // 更新眩晕时间
    [ServerCallback]
    private void Update() {
        if (stun > 0)
            stun -= Time.deltaTime;
    }

    // 受到攻击力为attack的攻击
    [Server]
    public void TakeAttack(int attack) {
        int damage = attack - defense; // 计算伤害
        if (damage <= 0) damage = 1; // 至少掉一滴血
        health -= damage; // 掉血
    }

    // 空血时调用
    [Server]
    protected abstract void OnEmptyHealth();

    // 生命值改变时客户端调整相应血管显示
    [Client]
    private void OnChangeHealth(int health) {
        m_Health = health;
        OnHealthChanged();
        OnChangeHp();
    }

    // 最大生命值改变时客户端调整相应血管显示
    [Client]
    private void OnChangeMaxHealth(int maxHealth) {
        m_MaxHealth = maxHealth;
        if (maxHealth >= health) OnHealthChanged();
        OnChangeHp();
    }

    [Client]
    private void OnHealthChanged() {
        if (healthBar != null)
            healthBar.rectTransform.sizeDelta = new Vector2((float)health / maxHealth, healthBar.rectTransform.sizeDelta.y);

        if (health > 0)
            spriteRenderer.color = new Color(1, 1, 1, spriteRenderer.color.a); // 1,1,1是白色
        else
            spriteRenderer.color = new Color(1, 0, 0, spriteRenderer.color.a); // 1,0,0是红色
    }

    // 防御力改变时调用
    [Client]
    private void OnChangeDefense(int defense) {
        m_Defense = defense;
        OnChangeDfs();
    }

    // 生命值或最大生命值改变时调用，此方法为了子类而存在
    [Client]
    protected virtual void OnChangeHp() { }

    // 防御力改变时调用，此方法为了子类而存在
    [Client]
    protected virtual void OnChangeDfs() { }

    // 受伤显示特效
    [Server]
    public void DamagedEffect(Vector2 effectPosition) {
        if (damagedEffectPrefab != null) {
            GameObject damagedEffect = Instantiate(damagedEffectPrefab, effectPosition, Quaternion.identity);
            NetworkServer.Spawn(damagedEffect);
        }
    }

    // 复活
    [Server]
    public void Resurrection() {
        health = maxHealth;
    }

    // 单位所属层改变时调用
    [Client]
    private void OnChangeLayer(int layer) {
        m_Layer = layer;
        OnLayerChanged(layer);
    }

    // 为新来的玩家显示之前改过的东西
    [Client]
    public override void OnStartClient() {
        OnHealthChanged(); // 正常显示之前死亡单位和单位血条
        OnLayerChanged(layer); // 正常显示对layer的更改
    }

    [Client]
    private void OnLayerChanged(int layer) {
        gameObject.layer = layer;

        string layerName = LayerMask.LayerToName(layer);
        if (layerName == "Air" || layerName == "EnemyAir")
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.9f);
        else
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        switch (layerName) {
            case "Player":
                spriteRenderer.sortingLayerName = "Overground";
                break;
            case "Float":
                spriteRenderer.sortingLayerName = "Float";
                break;
            case "Air":
                spriteRenderer.sortingLayerName = "Air";
                break;
        }
    }
}



/// <summary>
/// 单位组，用来区分不同单位所属的组别
/// </summary>
public enum Group { Player, Enemy, Neutral }
