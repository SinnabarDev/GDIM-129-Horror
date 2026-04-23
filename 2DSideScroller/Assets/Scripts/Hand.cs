using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isFacingRight = true;
    [SerializeField] private SpriteRenderer handSprite;

    void Awake()
    {
        if (handSprite == null)
        handSprite=GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mousePos - transform.position).normalized;
        

        //Block aiming behind player
        if (isFacingRight && direction.x < 0)
            direction = Vector2.right;

        if (!isFacingRight && direction.x > 0)
            direction = Vector2.left;
        
        if (!isFacingRight)
        {
            direction.x *= -1;
            direction.y *= -1;
        }
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
 
    }

    public void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}