using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] Players;
    public Text ItemDescription;
    public Text ProgressionText;
    public Text TimerText;
    public float[] TimeTuning;
    public GameObject InternalOverlay;
    public string[] InternalText;
    private float timer;
    private int internalIndex;
    public bool petStartGame;
    public bool GameEnd;
    public enum HIDE_ITEM
    {
        ITEM_1,
        ITEM_2,
        ITEM_3,
    }
    public Dictionary<HIDE_ITEM, bool> PlacedItems;
    public Dictionary<HIDE_ITEM, bool> FoundItems;
    public GameObject[] ItemPlacedGameObject;
    public GameObject currentShowingDescriptionItem;
    public enum STAGE
    {
        HUMAN_STAGE,
        PET_STAGE,
    }

    public STAGE CurrentStage;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void HandleStageStart(STAGE Stage)
    {
        CurrentStage = Stage;
        timer = TimeTuning[(int)Stage];
        switch (Stage)
        {
            case STAGE.HUMAN_STAGE:
                InternalOverlay.SetActive(false);
                Players[0].SetActive(true);
                Players[1].SetActive(false);
                for(int i = 0;i < 3;i++)
                {
                    PlacedItems[(HIDE_ITEM)i] = false;
                }
                ProgressionText.text = "0/3 Hided";
                break;
            case STAGE.PET_STAGE:
                internalIndex = 0;
                Players[0].GetComponent<PlayerControlHuman>().SkillBar.SetActive(false);
                Players[0].SetActive(false);
                Players[1].SetActive(true);
                GameObject[] treat = GameObject.FindGameObjectsWithTag("TREAT");
                for(int i = 0;i<treat.Length;i++)
                {
                    treat[i].GetComponent<PetTreat>().HandlePetRoundStart();
                }
                for (int i = 0; i < 3; i++)
                {
                    FoundItems[(HIDE_ITEM)i] = false;
                }
                ProgressionText.text = "0/"+ GetHumanPlacedItemNum().ToString() +  "Found";
                petStartGame = false;
                InternalOverlay.SetActive(true);
                InternalOverlay.GetComponentInChildren<Text>().text = InternalText[internalIndex];
                CatPlantManager.Instance.HandlePetRoundBegin();
                break;
        }
    }


    public void HandleHumanPlacedItem(HIDE_ITEM item, GameObject obj, bool isBait)
    {
        if(!isBait)
        {
            PlacedItems[item] = true;
            ItemPlacedGameObject[(int)item] = obj;
            ProgressionText.text = ((int)item + 1) + "/3 Hided";
            if (item == HIDE_ITEM.ITEM_3)
            {
                HandleNextStage();
            }
        }
        else
        {
            ItemPlacedGameObject[3] = obj;
        }
        
    }

    public int GetHumanPlacedItemNum()
    {
        int count = 0;
        for(int i = 0;i<3;i++)
        {
            if (PlacedItems[(HIDE_ITEM)i]) count++;
        }
        return count;
    }

    public int GetPetFoundItemNum()
    {
        int count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (FoundItems[(HIDE_ITEM)i]) count++;
        }
        return count;
    }


    public HIDE_ITEM GetCurrentHoldignItemForHuman()
    {
        for(int i = 0;i<3;i++)
        {
            if (!PlacedItems[(HIDE_ITEM)i]) return (HIDE_ITEM)i;
        }
        Debug.LogError("No Current Holdign Item");
        return HIDE_ITEM.ITEM_1;
    }

    public HIDE_ITEM GetCurrentSeekingItemForPet()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!FoundItems[(HIDE_ITEM)i]) return (HIDE_ITEM)i;
        }
        Debug.LogError("No Current Seeking Item");
        return HIDE_ITEM.ITEM_1;
    }



    public void HandlePetFindItem(HIDE_ITEM item, GameObject obj)
    {
        FoundItems[item] = true;
        ProgressionText.text = ((int)item + 1) + "/"+ GetHumanPlacedItemNum().ToString() + "Found";
        for(int i = 0;i<ItemPlacedGameObject.Length;i++)
        {
            if(ItemPlacedGameObject[i]==obj)
            {
                ItemPlacedGameObject[i] = null;
            }
        }
        if (item == (HIDE_ITEM)(GetHumanPlacedItemNum()-1) )
        {
            HandleNextStage();
        }
    }

    public void HandlePetFindBait()
    {
        ItemPlacedGameObject[3] = null;
    }

    public void HandleShowItemDescription(string desc,GameObject item)
    {
        ItemDescription.text = desc;
        if(currentShowingDescriptionItem != item && currentShowingDescriptionItem!=null)
        {
            if (currentShowingDescriptionItem.GetComponent<InteractableFurniture>() != null) currentShowingDescriptionItem.GetComponent<InteractableFurniture>().HandleDeactiveClosestItem();
        }
        currentShowingDescriptionItem = item;
        if(currentShowingDescriptionItem.GetComponent<InteractableFurniture>()!=null) currentShowingDescriptionItem.GetComponent<InteractableFurniture>().HandleActiveClosestItem();
    }

    public void HandleClearItemDescription(GameObject item)
    {
        if(currentShowingDescriptionItem == item)
        {
            ItemDescription.text = "";
        }
        if(item.GetComponent<InteractableFurniture>()!=null) item.GetComponent<InteractableFurniture>().HandleDeactiveClosestItem();
    }

    public GameObject GetCurrentPlayer()
    {
        return CurrentStage == STAGE.HUMAN_STAGE ? Players[0] : Players[1];
    }

    public void HandleNextStage()
    {
        if(CurrentStage == STAGE.HUMAN_STAGE)
        {
            HandleStageStart(STAGE.PET_STAGE);
        }
        else
        {
            GameEnd = true;
            petStartGame = false;
            InternalOverlay.SetActive(true);
            if (GetPetFoundItemNum() == GetHumanPlacedItemNum())
            {
                InternalOverlay.GetComponentInChildren<Text>().text = "You Have Found All of the hiden items!\n Contrats, You WIN!";
            }
            else
            {
                InternalOverlay.GetComponentInChildren<Text>().text = "You Failed to Found All of the hiden items,\n You LOSE!";
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PlacedItems = new Dictionary<HIDE_ITEM, bool>();
        FoundItems = new Dictionary<HIDE_ITEM, bool>();
        for (int i = 0;i<3;i++)
        {
            PlacedItems.Add((HIDE_ITEM)i, false);
            FoundItems.Add((HIDE_ITEM)i, false);
        }
        ItemPlacedGameObject = new GameObject[4];
        HandleStageStart(STAGE.HUMAN_STAGE);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            HandleNextStage();
        }
        if (CurrentStage == STAGE.PET_STAGE && !petStartGame)
        {
            if(GameEnd)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    internalIndex++;
                    if (internalIndex < InternalText.Length)
                    {
                        InternalOverlay.GetComponentInChildren<Text>().text = InternalText[internalIndex];
                    }
                    else
                    {
                        petStartGame = true;
                        InternalOverlay.SetActive(false);
                    }
                }
            }
           
        }
        else
        {
            timer -= Time.deltaTime;
            TimerText.text = "Time Left: " + Mathf.CeilToInt(timer).ToString();
            if (timer <= 0)
            {
                HandleNextStage();
            }
        }
    }
}
