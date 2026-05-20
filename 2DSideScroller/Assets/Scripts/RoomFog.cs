using System.Collections;
using UnityEngine;

public class RoomFog : MonoBehaviour
{
    public SpriteRenderer fogCover;
    public float fadeSpeed = 4f;

    Coroutine fadeRoutine;

    void FadeTo(float target)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(target));
    }

    IEnumerator Fade(float target)
    {
        Color c = fogCover.color;

        while (Mathf.Abs(c.a - target) > 0.01f)
        {
            c.a = Mathf.Lerp(c.a, target, Time.deltaTime * fadeSpeed);
            fogCover.color = c;
            yield return null;
        }

        c.a = target;
        fogCover.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            FadeTo(0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            FadeTo(1f);
    }
}
