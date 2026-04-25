using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] float knockbackForce = 7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float dir = Mathf.Sign(
                other.transform.position.x -
                transform.position.x
            );

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(
                new Vector2(dir * knockbackForce, 2f),
                ForceMode2D.Impulse
            );
        }
        }
    }
}
