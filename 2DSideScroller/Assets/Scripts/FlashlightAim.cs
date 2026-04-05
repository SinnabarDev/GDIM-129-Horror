using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    public bool isFacingRight = true;

    void Update()
    {
        AimFlashlight();
    }

    void AimFlashlight()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 direction = mousePos - transform.position;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //angle = Mathf.Clamp(angle, -180f, 0f);

        //transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 direction = (mousePos - transform.position).normalized;

        // 🚫 Block backwards aiming
        if (isFacingRight && direction.x < 0)
            direction.x = 0;

        if (!isFacingRight && direction.x > 0)
            direction.x = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}