using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    public string[] Description;
    public float InteractionDistance = 3;
    private bool hasShown;
    protected GameObject Player;
    public bool IsCloseToCurrentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    public void OnStart()
    {
        hasShown = false;
        if (Description.Length == 0)
        {
            Description = new string[2];
            Description[0] = "Maybe I could hide here...\nPress E to Hide here";
            Description[1] = "Could it be hiden here?";
        }
    }

    public bool IsCloseToPlayer()
    {
        Player = GameManager.Instance.GetCurrentPlayer();
        if (Vector3.Distance(this.transform.position, Player.transform.position) <= InteractionDistance) return true;
        return false;
    }

    public void OnUpdate()
    {
        if (GameManager.Instance == null) return;
        IsCloseToCurrentPlayer = IsCloseToPlayer();
        if (IsCloseToCurrentPlayer && !hasShown)
        {
            hasShown = true;
            GameManager.Instance.HandleShowItemDescription(Description[(int)GameManager.Instance.CurrentStage],this.gameObject);
        }
        if (!IsCloseToCurrentPlayer && hasShown)
        {
            hasShown = false;
            GameManager.Instance.HandleClearItemDescription(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
