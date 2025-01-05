using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableFurniture : ItemDescription
{
    public bool HasItemSavedHere;
    public bool IsBait;
    public UnityAction Hide;
    public UnityAction Seek;
    private Outline component;
    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        HasItemSavedHere = false;
        Hide += HandleHideItemSuccess;
        Seek += HandleSearchDone;
        component = this.gameObject.AddComponent<Outline>();
        component.OutlineMode = Outline.Mode.OutlineAll;
        component.OutlineColor = Color.red;
        component.OutlineWidth = 10;
        component.enabled = false;
        IsBait = false;
    }

    public void HandleActiveClosestItem()
    {
        component.enabled = true;
    }

    public void HandleDeactiveClosestItem()
    {
        component.enabled = false;
    }

    public void HandleSearchDone()
    {
        if(HasItemSavedHere)
        {
            if(!IsBait)
            {
                GameManager.Instance.HandlePetFindItem(GameManager.Instance.GetCurrentSeekingItemForPet(),this.gameObject);
                GameManager.Instance.HandleShowItemDescription("Grats! You just found something!", this.gameObject);
            }
            else
            {
                GameManager.Instance.HandlePetFindBait();
                GameManager.Instance.HandleShowItemDescription("It's a Bait!!!", this.gameObject);
            }
            HasItemSavedHere = false;
            IsBait = false;
        }
        else
        {
            GameManager.Instance.HandleShowItemDescription("Seems not here..", this.gameObject);
        }
        Player.GetComponent<PlayerControlPet>().HandleInteractionDone();
        Description[1] = "You Have Checked this place";
    }

    public void HandleHideItemSuccess()
    {
        HasItemSavedHere = true;
        IsBait = GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerControlHuman>().ItemJustPlacedBait;
        GameManager.Instance.HandleHumanPlacedItem(GameManager.Instance.GetCurrentHoldignItemForHuman(), this.gameObject, IsBait);
        if (IsBait)
        {
            GameManager.Instance.HandleShowItemDescription("Hide Bait Success!", this.gameObject);
        }
        else GameManager.Instance.HandleShowItemDescription("Hide Success!", this.gameObject);
        Description[0] = "You Already Hide something here..";
        Player.GetComponent<PlayerControlHuman>().HandleInteractionDone();
    }

    // Update is called once per frame
    void Update()
    {
        base.OnUpdate();
        if (IsCloseToCurrentPlayer&& GameManager.Instance.currentShowingDescriptionItem == this.gameObject && Input.GetKeyDown(KeyCode.E))
        {
            if(GameManager.Instance.CurrentStage == GameManager.STAGE.HUMAN_STAGE && !HasItemSavedHere)
            {
                Player.GetComponent<PlayerControlHuman>().HandleInteractWithFurniture(Hide);
            }
            else if (GameManager.Instance.CurrentStage == GameManager.STAGE.PET_STAGE)
            {
                Player.GetComponent<PlayerControlPet>().HandleInteractWithFurniture(Seek);
            }
        }

    }
}
