using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public HexItemType name;
    public HexGameController hexcontroller;
    public void del(){
    	hexcontroller.useItem(name);
        Destroy(gameObject);
        UIInfo.isFull = false;
    }

}
