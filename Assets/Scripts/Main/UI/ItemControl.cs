using UnityEngine;
using UnityEngine.UI;

public class ItemControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void del(){
        Destroy(gameObject);
        UIInfo.isFull = false;
    }
}
