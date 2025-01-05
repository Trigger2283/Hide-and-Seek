using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCatPlant : ItemDescription
{
    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        base.OnUpdate();
        if (IsCloseToCurrentPlayer && GameManager.Instance.currentShowingDescriptionItem == this.gameObject && Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.CurrentStage == GameManager.STAGE.PET_STAGE)
            {
                Player.GetComponent<PlayerControlPet>().HandleActiveCatnip();
            }
        }
    }
}
