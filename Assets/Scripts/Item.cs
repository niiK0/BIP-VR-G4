using UnityEngine;

public class Item : MonoBehaviour
{
    public int points = 10; // Valor dos pontos

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BreakTest>() != null) // Se o jogador tocar no item
        {
            ScoreManager.Instance.AddScore(points);
            //Destroy(gameObject); // Remove o item
        }
    }
}
