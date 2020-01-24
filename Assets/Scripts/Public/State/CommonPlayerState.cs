using UnityEngine;

public class CommonPlayerState<T> where T : MonoBehaviour, IPlayerState
{
    private T playerState;

	public CommonPlayerState(T playerState)
    {
        this.playerState = playerState;
    }

    public void OnChangeHealth()
    {
        PlayerStateDisplay.singleton.SetHealth(playerState.health, playerState.maxHealth);
    }

    public void OnChangeDefense()
    {
        PlayerStateDisplay.singleton.SetDefense(playerState.defense);
    }
}
