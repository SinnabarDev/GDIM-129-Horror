using UnityEngine;

public class UIUnflip : MonoBehaviour
{
    private Vector3 parentScale;

    void LateUpdate()
    {
        Transform p = transform.parent;
        if (!p)
            return;

        Vector3 ps = p.lossyScale;

        // cancel parent's X flip
        transform.localScale = new Vector3(1f / Mathf.Sign(ps.x), 1f, 1f);
    }
}
