using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerControlHuman : PlayerControllerBase
{
    public float PlacingItemLength;
    public bool ItemJustPlacedBait;
    public GameObject Choice;
    public GameObject TreatPrefab;
    public GameObject RunBuffIcon;
    private float timer;
    private float startTimerForRun;
    private bool isInSpeedUp;
    UnityAction action;
    public enum SKILLS
    {
        SPEEDUP,
        TREAT,
        BAIT,
    }
    public Image[] SkillsIcon;
    public GameObject SkillBar;
    private Dictionary<SKILLS, float> SkillsCount;

    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        SkillsCount = new Dictionary<SKILLS, float>();
        SkillsCount.Add(SKILLS.BAIT, 1);
        SkillsCount.Add(SKILLS.SPEEDUP, 1);
        SkillsCount.Add(SKILLS.TREAT, 3);
        SkillBar.SetActive(true);
        isInSpeedUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInSpeedUp)
        {
            RunBuffIcon.GetComponent<Slider>().value = (Time.time - startTimerForRun) / 10f;
        }
        if (IsInteractingWithFurniture)
        {
            this.GetComponentInChildren<Animator>().SetBool("RUN", false);
            this.GetComponentInChildren<Animator>().SetBool("RUN_FAST", false);
            return;
        }
        base.OnUpdate();
        CheckForSkills();
        UpdateSkillsUI();
    }

    void CheckForSkills()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && SkillsCount[SKILLS.SPEEDUP]>0)
        {
            isInSpeedUp = true;
            Speed *= 2;
            SkillsCount[SKILLS.SPEEDUP]--;
            Invoke("ResetSpeed", 10f);
            RunBuffIcon.SetActive(true);
            RunBuffIcon.GetComponent<Slider>().value = 0;
            startTimerForRun = Time.time;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && SkillsCount[SKILLS.TREAT] > 0)
        {
            SkillsCount[SKILLS.TREAT]--;
            //place a treat
            GameObject treat = Instantiate(TreatPrefab);
            treat.transform.position = this.transform.position;
        }
    }

    void UpdateSkillsUI()
    {
        for(int i = 0;i< SkillsIcon.Length;i++)
        {
            if(SkillsCount[(SKILLS)i]<=0)
            {
                SkillsIcon[i].color = Color.gray;
            }
            else
            {
                SkillsIcon[i].color = Color.white;
            }
            SkillsIcon[i].gameObject.GetComponentInChildren<Text>().text = SkillsCount[(SKILLS)i].ToString();
        }
        
    }

    private void ResetSpeed()
    {
        Speed /= 2;
        isInSpeedUp = false;
        RunBuffIcon.SetActive(false);
    }

    IEnumerator HideItem(UnityAction Action)
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            ProgressionBar.value = timer / PlacingItemLength;
            if (timer>=PlacingItemLength)
            {
                Action();
                ProgressionBar.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void HandlePlaceItem(bool isBait)
    {
        ItemJustPlacedBait = isBait;
        if (isBait) SkillsCount[SKILLS.BAIT]--;
        ProgressionBar.gameObject.SetActive(true);
        ProgressionBar.value = 0;
        timer = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(HideItem(action));
    }

    public override void HandleInteractWithFurniture(UnityAction Action)
    {
        base.HandleInteractWithFurniture(Action);
        action = Action;
        if(SkillsCount[SKILLS.BAIT] > 0)
        {
            Choice.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            HandlePlaceItem(false);
        }
    }
}
