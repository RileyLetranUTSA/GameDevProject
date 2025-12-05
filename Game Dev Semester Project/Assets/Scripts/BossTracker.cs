using UnityEngine;

public class BossTracker : MonoBehaviour
{
    public RectTransform indicatorRect;
    public Camera gameCamera;

    private BossController currentBoss;

    private void Update()
    {
        if (currentBoss == null)
        {
            currentBoss = FindAnyObjectByType<BossController>();
            if (currentBoss == null) return;
        }

        Transform bossTransform = currentBoss.transform;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 screenPos = gameCamera.WorldToScreenPoint(bossTransform.position);
        Vector2 fromCenter = new Vector2(screenPos.x - screenCenter.x, screenPos.y - screenCenter.y);

        if (screenPos.z < 0f)
            fromCenter = -fromCenter;

        if (fromCenter == Vector2.zero)
            fromCenter = Vector2.up * 0.01f;

        Vector2 dir = fromCenter.normalized;

        float padding = 40f;
        float maxX = (Screen.width / 2f) - padding;
        float maxY = (Screen.height / 2f) - padding;

        float scaleX = maxX / Mathf.Abs(dir.x);
        float scaleY = maxY / Mathf.Abs(dir.y);
        float scale = Mathf.Min(scaleX, scaleY);

        Vector2 indicatorPos = dir * scale + (Vector2)screenCenter;

        indicatorPos.x = Mathf.Clamp(indicatorPos.x, padding, Screen.width - padding);
        indicatorPos.y = Mathf.Clamp(indicatorPos.y, padding, Screen.height - padding);

        indicatorRect.position = indicatorPos;

        if (!indicatorRect.gameObject.activeSelf)
            indicatorRect.gameObject.SetActive(true);
    }
}
