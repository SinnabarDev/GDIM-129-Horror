using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject HitBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void EnableHitBox()
    {
        HitBox.SetActive(true);
    }

    public void DisableHitBox()
    {
        HitBox.SetActive(false);
    }
}
