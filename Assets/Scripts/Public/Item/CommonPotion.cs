using UnityEngine;

public class CommonPotion<T> where T : MonoBehaviour, IPotion
{
    private T potion;

    public CommonPotion(T potion)
    {
        this.potion = potion;
    }
    
    public bool FindOwner(Collider2D collider)
    {
        if (!collider.CompareTag("Player") || collider.GetComponent<IPlayerController>().vehicle != null) return false;

        IState state = collider.GetComponent<IState>();
        if (state.health <= 0) return false;

        state.maxHealth += potion.maxHealth;
        state.health += potion.health;
        state.defense += potion.defense;

        return true;
    }
}