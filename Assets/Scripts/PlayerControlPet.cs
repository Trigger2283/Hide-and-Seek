using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerControlPet : PlayerControllerBase
{
    public float SeekingItemLength;
    public Image SomethingCloseIcon;
    //index 0: farest detectable distance, //1: closet distance
    public float[] CloseTuning;
    public GameObject[] BuffIcons;
    public float[] SkillDuration;
    private bool[] isInSkills;
    private float[] buffStartTime;
    public enum SKILLS
    {
        LICK_NOSE,
        SHARPEN_CLAWS,
    }
    public Image[] SkillsIcon;
    public GameObject SkillBar;
    private Dictionary<SKILLS, float> SkillsCount;
    private bool ignoreBait;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        SomethingCloseIcon.enabled = false;
        SkillsCount = new Dictionary<SKILLS, float>();
        SkillsCount.Add(SKILLS.LICK_NOSE, 1);
        SkillsCount.Add(SKILLS.SHARPEN_CLAWS, 1);
        SkillBar.SetActive(true);
        ignoreBait = false;
        isInSkills = new bool[3];
        buffStartTime = new float[3];
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.petStartGame) return;
        base.OnUpdate();
        for (int i = 0; i < isInSkills.Length; i++)
        {
            if (isInSkills[i])
            {
                BuffIcons[i].GetComponent<Slider>().value = (Time.time - buffStartTime[i]) / SkillDuration[i];
            }
        }
        if (IsInteractingWithFurniture) return;
        //For showing "something is close" Icon
        float cloestDis = float.MaxValue;
        
        for(int i = 0;i<GameManager.Instance.ItemPlacedGameObject.Length;i++)
        {
            if (GameManager.Instance.ItemPlacedGameObject[i] == null) continue;
            if (ignoreBait && i == 3) continue;
            float temp = Vector3.Distance(this.transform.position, GameManager.Instance.ItemPlacedGameObject[i].transform.position);
            if (temp < cloestDis) cloestDis = temp;
        }
        if(cloestDis<=CloseTuning[0])
        {
            SomethingCloseIcon.enabled = true;
            float alpha = 0;
            if (cloestDis <= CloseTuning[1]) alpha = 1;
            else
            {
                alpha = Mathf.Lerp(1, 0, (cloestDis - CloseTuning[1]) / (CloseTuning[0] - CloseTuning[1]));
            }
            SomethingCloseIcon.color = new Color(0, 0, 0, alpha);
        }
        else
        {
            SomethingCloseIcon.enabled = false;
        }
        CheckForSkills();
        UpdateSkillsUI();
    }

    void CheckForSkills()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && SkillsCount[SKILLS.LICK_NOSE] > 0)
        {
            SkillsCount[SKILLS.LICK_NOSE]--;
            ignoreBait = true;
            BuffIcons[0].SetActive(true);
            BuffIcons[0].GetComponent<Slider>().value = 0;
            isInSkills[0] = true;
            buffStartTime[0] = Time.time;
            Invoke("ResetLickNose", SkillDuration[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && SkillsCount[SKILLS.SHARPEN_CLAWS] > 0)
        {
            SkillsCount[SKILLS.SHARPEN_CLAWS]--;
            StartCoroutine(SharpenClaw());
        }
    }
     void ResetLickNose()
    {
        ignoreBait = false;
        BuffIcons[0].SetActive(false);
        isInSkills[0] = false;
    }

    IEnumerator SeekingItem(UnityAction Action)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            ProgressionBar.value = timer / SeekingItemLength;
            if (timer >= SeekingItemLength)
            {
                Action();
                ProgressionBar.gameObject.SetActive(false);
                break;
            }
        }
    }

    IEnumerator Treat(GameObject treat)
    {
        ProgressionBar.gameObject.SetActive(true);
        ProgressionBar.value = 0;
        timer = 0;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            ProgressionBar.value = timer / 5f;
            if (timer >= 5f)
            {
                IsInteractingWithFurniture = false;
                ProgressionBar.gameObject.SetActive(false);
                Destroy(treat);
                break;
            }
        }
    }

    void ResetSharpenClaw()
    {
        BuffIcons[1].SetActive(false);
        isInSkills[1] = false;
        SeekingItemLength = SeekingItemLength * 3;
    }

    IEnumerator SharpenClaw()
    {
        ProgressionBar.gameObject.SetActive(true);
        ProgressionBar.value = 0;
        timer = 0;
        IsInteractingWithFurniture = true;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            ProgressionBar.value = timer / 3f;
            if (timer >= 3f)
            {
                IsInteractingWithFurniture = false;
                ProgressionBar.gameObject.SetActive(false);
                SeekingItemLength = SeekingItemLength / 3;
                BuffIcons[1].SetActive(true);
                BuffIcons[1].GetComponent<Slider>().value = 0;
                isInSkills[1] = true;
                buffStartTime[1] = Time.time;
                Invoke("ResetSharpenClaw", SkillDuration[1]);
                break;
            }
        }
    }

    void UpdateSkillsUI()
    {
        for (int i = 0; i < SkillsIcon.Length; i++)
        {
            if (SkillsCount[(SKILLS)i] <= 0)
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

    public void HandleActiveCatnip()
    {
        CatPlantManager.Instance.HandleActivePlant();
        Speed *= 2;
        isInSkills[2] = true;
        buffStartTime[2] = Time.time;
        BuffIcons[2].SetActive(true);
        BuffIcons[2].GetComponent<Slider>().value = 0;
        StartCoroutine(Catnip());
    }

    IEnumerator Catnip()
    {
        yield return new WaitForSeconds(SkillDuration[2]);
        float temp = Speed / 2;
        Speed = 0;
        BuffIcons[2].SetActive(false);
        isInSkills[2] = false;
        yield return new WaitForSeconds(3f);
        Speed = temp; 
    }

    public override void HandleInteractWithFurniture(UnityAction Action)
    {
        base.HandleInteractWithFurniture(Action);
        ProgressionBar.gameObject.SetActive(true);
        ProgressionBar.value = 0;
        timer = 0;
        StartCoroutine(SeekingItem(Action));
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TREAT")
        {
            IsInteractingWithFurniture = true;
            other.GetComponent<PetTreat>().HandlePetFoundThis();
            StartCoroutine(Treat(other.gameObject));
        }
    }
}
