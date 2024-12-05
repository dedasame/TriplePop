using TMPro;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public static TargetScript Instance;
    void Awake()
    {
        Instance = this;
    }

    public GameObject image;
    public TextMeshProUGUI text;
    public int targetColor;
    public int targetVal;
    public int temp = 0;

    public int moveCount;

    void Start()
    {
        SetText();
        image.GetComponent<UnityEngine.UI.Image>().color = GameScript.Instance.colors[targetColor];
    }
    
    void Update()
    {
        
    }

    public void SetText()
    {
        if(temp>=targetVal)
        {
            //hedef toplandi
            text.SetText("DONE");
            
            PlayerPrefs.SetInt("level",PlayerPrefs.GetInt("level")+1);
            if(PlayerPrefs.GetInt("level")>=4)
            {
                PlayerPrefs.SetInt("level",0);
            }
            GameScript.Instance.completed = true;
            GameScript.Instance.completedScreen.SetActive(true);
            GameScript.Instance.completedScreen.GetComponentInChildren<Animator>().SetTrigger("anim");
        }
        else
        {
            text.SetText(targetVal +" / "+ temp);
        }
       
        
    }
    
}
