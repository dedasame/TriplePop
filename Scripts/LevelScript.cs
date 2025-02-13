using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelScript : MonoBehaviour
{
    public static LevelScript Instance;
    void Awake()
    {
        Instance = this;
    }
    public List<TargetScript> targetList; //target listesi
    public int moveCount;
    public int[] values = {0,1,2}; //renk cikma olsasiligi icin
    public bool levelDone;
    public bool levelFailed;


    void Start()
    {
        foreach(TargetScript target in targetList)
        {
            //olusturduk
            target.blockControl = Instantiate(GameScript.Instance.controlBlock,GameScript.Instance.targetPanel.transform);
            if(target.itemIndex == 1) target.blockControl.GetComponent<UnityEngine.UI.Image>().sprite = GameScript.Instance.boxSprite.GetComponentInChildren<UnityEngine.UI.Image>().sprite;
            //??????????? HÜHÜHÜHÜHÜ
            
            //renk ve sayi ayari
            target.SetColor();
            target.SetText();
        }
    }


    //ITEM KONTROL KISMI
    public void CheckSelectedItems()
    {
        //box secilmis ise -> kontrole gonder
        if(GameScript.Instance.boxes.Count != 0) CheckBox();

        for(int i = 0; i<targetList.Count; i++)
        {
            //secili itemlerin rengini kontrol ediyoruz.
            if(targetList[i].color == GameScript.Instance.selectedItem.colorIndex)
            {
                //bir target ile eslesti o scriptte islemleri devam ettir ->
                targetList[i].AddItem(GameScript.Instance.targetPosList[i]);
                return;
            }
        }
        //herhangi bir target ile eslesmedi ve cikti
        DontHaveTargetItem();
    }

    public void DontHaveTargetItem()
    {
        foreach(ItemScript item in GameScript.Instance.selectedItems)
        {
            item.GetComponentInParent<GridScript>().item = null;
            item.anim.SetTrigger("pop");
        }
        GameScript.Instance.popSound.Play();
        LevelCompleteCheck();
    }


    //EKLEME - KONTROL BITTIKTEN SONRA ->> LEVEL KONTROL
    public void LevelCompleteCheck()
    {
        for(int i = 0; i<targetList.Count;i++)
        {
            if(!targetList[i].done) 
            {
                MoveCheck(); //target bitmedi move check
                return;
            }
        }
        if(!levelDone)
        {
            //level completed
            levelDone = true;
            GameScript.Instance.completed = true;
            PlayerPrefs.SetInt("level",PlayerPrefs.GetInt("level")+1);
            GameScript.Instance.completedScreen.SetActive(true);
            GameScript.Instance.completedScreen.GetComponent<Animator>().SetTrigger("anim");
        }
    }

    //HAMLE KONTROLU
    public void MoveCheck() 
    {
        if(GameScript.Instance.tempMoveCount == 0)//move kalmadi
        {
            //failed
            if(!levelFailed)
            {
                levelFailed = true;
                GameScript.Instance.failed = true;
                GameScript.Instance.failedScreen.SetActive(true);
                GameScript.Instance.failedScreen.GetComponent<Animator>().SetTrigger("anim");
            }
            
        }
        else //hala move var
        {
            GameScript.Instance.boxes.Clear();
            GameScript.Instance.selectedItems.Clear();
            GameScript.Instance.selectedItem = null;
        }
    }



    //BOX KONTROL KISMI
    public void CheckBox()
    {
        for(int i = 0; i<targetList.Count; i++)
        {
            //target ve rengi uyuyor ise
            if(targetList[i].itemIndex == 1 && targetList[i].color == GameScript.Instance.boxes[0].colorIndex)
            {
                //targeta gonder
                targetList[i].AddBox();
                return;
            }
        }
        //herhangi bir targetda yok
        DontHaveTargetBox();
    }

    public void DontHaveTargetBox()
    {
        foreach(BoxScript box in GameScript.Instance.boxes)
        {
            box.GetComponentInParent<GridScript>().item = null; //gridi temizliyoruz
            box.gameObject.SetActive(false); //boxi kapat
            box.anim.SetTrigger("break");
        }
        GameScript.Instance.glassSound.Play();
        GameScript.Instance.boxes.Clear(); //box listesini temizle
    }

}
