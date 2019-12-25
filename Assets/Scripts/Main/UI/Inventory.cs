using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    private Transform findchild;
    private Inventory inventory;

    public GameObject[] items;
    void Start(){

    }
    void Update(){
    	if(Input.GetKeyDown(KeyCode.F6))
    	{
    		for ( int i = 0; i < slots.Length;i++)
    		{
    			if(isFull[i] == false){
    				isFull[i] = true;
    				Instantiate(items[0], slots[i].transform, false);
    				break;
    			}

    		}
    	}

    	if(Input.GetKeyDown(KeyCode.F1))
    	{
    		for ( int i = 0; i < slots.Length;i++)
    		{
    			if(isFull[i] == false){
    				isFull[i] = true;
    				Instantiate(items[1], slots[i].transform, false);
    				break;
    			}

    		}
    	}
        if(Input.GetKeyDown(KeyCode.F2))
        {
            for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i] == false){
                    isFull[i] = true;
                    Instantiate(items[2], slots[i].transform, false);
                    break;
                }

            }
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i] == false){
                    isFull[i] = true;
                    Instantiate(items[3], slots[i].transform, false);
                    break;
                }

            }
        }
        if(Input.GetKeyDown(KeyCode.F4))
        {
            for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i] == false){
                    isFull[i] = true;
                    Instantiate(items[4], slots[i].transform, false);
                    break;
                }

            }
        }
        if(Input.GetKeyDown(KeyCode.F5))
        {
            for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i] == false){
                    isFull[i] = true;
                    Instantiate(items[5], slots[i].transform, false);
                    break;
                }

            }
        }
    	if(Input.GetKeyDown(KeyCode.F10))
    	{
            for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i]){
                    isFull[i] = false;
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                    break;
                }
            }
    	}
    	

    }

    public void GetItem(int k){
        for ( int i = 0; i < slots.Length;i++)
            {
                if(isFull[i] == false){
                    isFull[i] = true;
                    Instantiate(items[k], slots[i].transform, false);
                    break;
                }

            }
    }
}
