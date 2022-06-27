using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MessageFeedManager : MonoBehaviour
{
    
    private static MessageFeedManager instance;

    [SerializeField] private GameObject messagePrefab;

    public static MessageFeedManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MessageFeedManager>();
            }

            return instance;
        }
    }

    public void WriteMessage(string messgae)
    {
       GameObject go= Instantiate(messagePrefab, transform);

       go.GetComponent<Text>().text = messgae;
       
       go.transform.SetAsFirstSibling();
       
       Destroy(go,2);
    }
    
    
}
