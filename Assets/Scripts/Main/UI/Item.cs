using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public HexItemType itemname;
    public HexGameController hexcontroller;
    public void del(){
    	Debug.Log("pushbutton");
    	Debug.Log(name);
    	hexcontroller.useItem(itemname);
        Destroy(gameObject);
        UIInfo.isFull = false;
    }

}
