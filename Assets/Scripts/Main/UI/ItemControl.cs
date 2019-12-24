using UnityEngine;
using UnityEngine.UI;

public class ItemControl : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isSelected = false;
    public bool isUsed = false;
    public int type;
    public void Used(){
    	isUsed = true;
    	Destroy(gameObject);
    }
    public void Select(){
    	Debug.Log("touch isSelected");
    	isSelected = true;
    	gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
    }
    public void Unselect(){
    	isSelected = false;
    }
    public int FetchType(){
        return type;
    }
}
