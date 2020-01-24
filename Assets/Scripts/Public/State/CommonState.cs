using UnityEngine;

public class CommonState<T> where T : MonoBehaviour, IState
{
    private T state;

    public CommonState(T state)
    {
        this.state = state;
    }

    /// <summary>
    /// 当前生命值等于最大生命值。
    /// </summary>
    public void Awake()
    {
        state.health = state.maxHealth;
    }

    // 更新眩晕时间
    public void Update()
    {
        if (state.stun > 0)
            state.stun -= Time.deltaTime;
    }

    public void TakeAttack(int attack)
    {
        int damage = attack - state.defense; // 计算伤害

        if (damage <= 0) damage = 1; // 至少掉一滴血

        state.health -= damage; // 掉血
    }

    public void OnChangeHealth()
    {
        if (state.healthBar != null)
            state.healthBar.rectTransform.sizeDelta = new Vector2((float)state.health * 100 / state.maxHealth, state.healthBar.rectTransform.sizeDelta.y);

        if (state.health > 0)
            state.spriteRenderer.color = new Color(1, 1, 1, state.spriteRenderer.color.a); // 1,1,1是白色
        else
            state.spriteRenderer.color = new Color(1, 0, 0, state.spriteRenderer.color.a); // 1,0,0是红色
    }

    public GameObject DamagedEffect(Vector2 effectPosition)
    {
        if (state.damagedEffectPrefab == null)
            return null;
        
        return Object.Instantiate(state.damagedEffectPrefab, effectPosition, Quaternion.identity);
    }

    public void Resurrection()
    {
        state.health = state.maxHealth;
    }

    public void OnChangeLayer(int layer)
    {
        state.gameObject.layer = layer;

        string layerName = LayerMask.LayerToName(layer);
        if (layerName == "Air" || layerName == "EnemyAir")
            state.spriteRenderer.color = new Color(state.spriteRenderer.color.r, state.spriteRenderer.color.g, state.spriteRenderer.color.b, 0.9f);
        else
            state.spriteRenderer.color = new Color(state.spriteRenderer.color.r, state.spriteRenderer.color.g, state.spriteRenderer.color.b, 1f);

        switch (layerName)
        {
            case "Player":
                state.spriteRenderer.sortingLayerName = "Overground";
                break;
            case "Float":
                state.spriteRenderer.sortingLayerName = "Float";
                break;
            case "Air":
                state.spriteRenderer.sortingLayerName = "Air";
                break;
        }
    }
}



/// <summary>
/// 单位组，用来区分不同单位所属的组别
/// </summary>
public enum Group { Player, Enemy, Neutral }