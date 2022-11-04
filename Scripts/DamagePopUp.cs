using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    
    private void Awake() 
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void HandePopUp(int damageAmount, string type)
    {
        var pos = transform.position;
        pos.x += Random.Range(-0.5f , 0.5f);
        pos.y += Random.Range(-0.5f , 0.5f);
        transform.position = pos;
        
        textMesh.SetText(damageAmount.ToString());
        switch (type)
        {
            case "normal":
            {
                textMesh.color = Color.cyan;
                break;
            }
            case "crit":
            {
                textMesh.color = Color.red;
                break;
            }
            case "heal":
            {
                textMesh.color = Color.green;
                break;
            }

        }
        Destroy(gameObject, 1f);
    }

    

}
