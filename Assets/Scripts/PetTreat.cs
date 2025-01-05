using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetTreat : MonoBehaviour
{
    public void HandlePetRoundStart()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<BoxCollider>().isTrigger = true;
    }

    public void HandlePetFoundThis()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        GameManager.Instance.HandleShowItemDescription("What a nice treat!! I couldn't help myself...", this.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.HandleClearItemDescription(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
