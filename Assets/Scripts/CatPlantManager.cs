using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPlantManager : MonoBehaviour
{
    public static CatPlantManager Instance;
    public GameObject[] CatPlants;
    private List<GameObject> shuffledList;
    // Start is called before the first frame update
    void Start()
    {
        shuffledList = new List<GameObject>();
        foreach(GameObject plant in CatPlants)
        {
            shuffledList.Add(plant);
            plant.SetActive(false);
        }
        //shuffle the list
        for(int i = 0;i<shuffledList.Count;i++)
        {
            GameObject temp = shuffledList[i];
            int randomIndex = Random.Range(0, shuffledList.Count);
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void HandlePetRoundBegin()
    {
        shuffledList[0].SetActive(true);
    }

    public void HandleActivePlant()
    {
        GameObject temp = shuffledList[0];
        shuffledList.RemoveAt(0);
        Destroy(temp);
        if(shuffledList.Count>1)
        {
            shuffledList[0].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
