using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card_ShowcaseGen : MonoBehaviour
{

    public bool AllCard;
    
    public List<GameObject> CardList;
    private int RoW, Column = 0;
    public int[] basePos = new int[2];

    public float GenAll_SizeScale;
    public int GenAll_ColumnLimit;
    public int GenAll_ColumnSpace, GenAll_RowSpace;
    
    public float GenFinished_SizeScale;
    public int GenFinished_ColumnLimit;
    public int GenFinished_ColumnSpace, GenFinished_RowSpace;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] cardArray = Resources.LoadAll<GameObject>("Prefabs/Cards");
        CardList = cardArray.ToList();

        //get rid of Card Sample
        for (int i = 0; i < CardList.Count; i++)
        {
            if (CardList[i].name == "Card")
                CardList.Remove(CardList[i]);
        }
        
        //gen card
        if (AllCard == true) GenAllCard();
        else GenFinishedCard();
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenAllCard()
    {
        //start instantiate card
        for (int i = 0; i < CardList.Count; i++)
        {
            int H_pos = Column * GenAll_ColumnSpace;
            int V_pos = RoW * GenAll_RowSpace;
            GameObject card = Instantiate(CardList[i],transform);
            card.transform.localScale *= GenAll_SizeScale;
            card.transform.position = new Vector3(H_pos + basePos[0], -V_pos + basePos[1], 0 );
            card.GetComponent<Card>().ShowFullSize = true;
            
            if (Column < GenAll_ColumnLimit)
            {
                Column++;
            }
            else
            {
                Column = 0;
                RoW++;
            }
        }
    }

    void GenFinishedCard()
    {
        //get rid of Unfinished Card
        for (int i = 0; i < CardList.Count; i++)
        {
            if (CardList[i].GetComponent<Card>().CardImage == null || CardList[i].GetComponent<Card>().CardImage.name == "Blank")
            {
                CardList.Remove(CardList[i]);
                i = i - 1;
            }
        }
        
        //start instantiate card
        for (int i = 0; i < CardList.Count; i++)
        {
            int H_pos = Column * GenFinished_ColumnSpace;
            int V_pos = RoW * GenFinished_RowSpace;
            GameObject card = Instantiate(CardList[i],transform);
            card.transform.localScale *= GenFinished_SizeScale;
            card.transform.position = new Vector3(H_pos + basePos[0], -V_pos + basePos[1], 0 );
            card.GetComponent<Card>().ShowFullSize = true;
            
            if (Column < GenFinished_ColumnLimit)
            {
                Column++;
            }
            else
            {
                Column = 0;
                RoW++;
            }
        }
    }
}
