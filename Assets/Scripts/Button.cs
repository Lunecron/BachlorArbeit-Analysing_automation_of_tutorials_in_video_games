using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("References")]
    public GameObject pushable_button;
    public float button_pushInDistance = 0.1f;
    
    public GameObject connected_element;
    [SerializeField] Timer timer;


    [Header("OnButtonPress")]
    public bool changeMaterial = true;
    public Material changed_buttonMaterial;
    public bool activateConnected_element = false;
    public bool changeMaterialOfConnected_element = true;
    public Material materialToChangeTo;


    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!timer)
        {
            Debug.Log("No Timer found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive)
        {
            return;
        }

        if(other.tag == "Player")
        {
            if (!other.gameObject.GetComponentInParent<PlayerMovement>())
            {
                Debug.Log("Player Movement Script not Found" + other.gameObject.name);
                return;
            }
           if(other.gameObject.GetComponentInParent<PlayerMovement>().grounded)
            {
                isActive = true;
                if (!connected_element)
                {
                    Debug.Log("Missing Connected Element of :" + gameObject.name);
                    return;
                }

                if (!pushable_button)
                {
                    Debug.Log("Missing Pushable Buttonpart of :" + gameObject.name);
                    return;
                }
                
                PushButtonDown();
                timer.StopTimer();
                timer.SaveTimer();

                
                if (changeMaterial)
                {
                    ChangeMaterial(pushable_button,changed_buttonMaterial);
                }
               
                if (activateConnected_element)
                {
                    connected_element.gameObject.SetActive(true);
                }
                
                if (changeMaterialOfConnected_element)
                {
                    ChangeMaterial(connected_element, materialToChangeTo);
                }
               
            }
        }
    }

    private void PushButtonDown()
    {
        pushable_button.transform.localScale = new Vector3(pushable_button.transform.localScale.x, pushable_button.transform.localScale.y / 2, pushable_button.transform.localScale.z);
        //pushable_button.transform.localPosition = pushable_button.transform.localPosition - new Vector3(0, button_pushInDistance, 0);
    }

    private void ChangeMaterial(GameObject objectToChange, Material newMaterial)
    {
        if (!newMaterial)
        {
            Debug.Log("Material not assiened! Take Defaul Material");
            newMaterial = objectToChange.GetComponent<MeshRenderer>().material;
        }
        if (!objectToChange)
        {
            Debug.Log("Object not assiened!");
            return;
        }

        objectToChange.GetComponent<MeshRenderer>().material = newMaterial;
    }


}
