
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType
    {
        Normal,
        Red,
        Blue
    }

    [SerializeField] private CoinType coinType = CoinType.Normal;
    [SerializeField] private int value = 1;
    [SerializeField] private float spinSpeed = 90f;

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        switch (coinType)
        {
            case CoinType.Normal:
                GameManager.Instance.AddScore(value);
                Destroy(gameObject);
                break;

            case CoinType.Red:
                GameManager.Instance.CollectRedCoin();
                Destroy(gameObject);
                break;

            case CoinType.Blue:
                GameManager.Instance.CollectBlueCoin();
                Destroy(gameObject);
                break;
        }
    }
}
