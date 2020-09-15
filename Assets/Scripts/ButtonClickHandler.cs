using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        CheckCollision(gameObject.transform.position, 0.2f);
    }

    void CheckCollision(Vector2 center, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        Color32[] oldColors = new Color32[3];
        int i = 0;
        foreach (var hitCollider in hitColliders)
        {
            print("Collider" + i + ": " + hitCollider.gameObject.name);
            oldColors[i] = hitCollider.gameObject.GetComponent<SpriteRenderer>().color;
            ++i;
            hitCollider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        StartCoroutine(waiter(oldColors, hitColliders));

    }

    IEnumerator waiter(Color32[] tempColors, Collider2D[] hitColliders)
    {
        yield return new WaitForSeconds(2);
        int i = 0;
        foreach(var hitCollider in hitColliders)
        {
            hitCollider.gameObject.GetComponent<SpriteRenderer>().color = tempColors[i];
            ++i;
        }

    }
}
