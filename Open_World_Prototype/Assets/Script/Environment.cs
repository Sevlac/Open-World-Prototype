using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    // Ce script est attaché à tout gameobject composant l'environement


    public int health = 3;
   public IEnumerator Attacked(bool axeInHand, bool pickaxeInHand) // Fonction appelé quand le gameobject est frappé
    {
        // Si le le gameobject est un arbre et que la hache est dans la main
        if (gameObject.tag == "Pine" && axeInHand)
        {
            yield return new WaitForSeconds(1);
            health -= 1;
            if (health <= 0)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(1,1,1);
                yield return new WaitForSeconds(5);
                Destroy(gameObject);
            }

        }

        // Si le le gameobject est un rocher et que la pioche est dans la main
        if (gameObject.tag == "Rock" && pickaxeInHand)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("rock");
            health -= 1;
            if (health <= 0)
            {
                foreach (Transform child in transform)
                {
                    child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                }
                yield return new WaitForSeconds(5);
                Destroy(gameObject);
            }

        }
    }
}
