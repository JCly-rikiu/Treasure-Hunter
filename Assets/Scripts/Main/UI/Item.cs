using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public HexItemType itemname;
    public HexGameController hexcontroller;
    public void del(){
    	hexcontroller.useItem(itemname);
        Destroy(gameObject);
        if(itemname == HexItemType.Change)
        {
            UIInfo.changeisFull = false;
        }else if (itemname == HexItemType.FakeTreasureItem)
        {
            UIInfo.boxisFull = false;
        }else if (itemname == HexItemType.Poison)
        {
            UIInfo.poisonisFull = false;
        }
    }

}
