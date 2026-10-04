using UnityEngine;

namespace PinkTaxGame
{
    public class ProductIdleMotion : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 35f;

        [Header("Hover")]
        [Min(0f)]
        [SerializeField] private float hoverSpeed = 0.1f;

        [Min(0f)]
        [SerializeField] private float hoverHeight = 0.1f;

        [Header("Randomization")]
        [Range(0f, 0.1f)]
        [SerializeField] private float maxSpeedOffset = 0.1f;

        private float baseLocalY;
        private float hoverTime;

        private float actualRotationSpeed;
        private float actualHoverSpeed;

        private void Awake()
        {
            baseLocalY = transform.localPosition.y;

            actualRotationSpeed =
                rotationSpeed * Random.Range(1f, 1f + maxSpeedOffset);

            actualHoverSpeed =
                hoverSpeed * Random.Range(1f, 1f + maxSpeedOffset);

            hoverTime = Random.Range(0f, 1f);
        }

        private void Update()
        {
            Rotate();
            Hover();
        }

        private void Rotate()
        {
            transform.Rotate(
                Vector3.up,
                actualRotationSpeed * Time.deltaTime,
                Space.Self
            );
        }

        private void Hover()
        {
            hoverTime += Time.deltaTime * actualHoverSpeed;

            float normalizedHeight =
                (1f - Mathf.Cos(hoverTime * Mathf.PI * 2f)) * 0.5f;

            float hoverOffset =
                normalizedHeight * hoverHeight;

            Vector3 position = transform.localPosition;

            position.y =
                baseLocalY + hoverOffset;

            transform.localPosition = position;
        }

        public void ResetBaseHeight()
        {
            baseLocalY = transform.localPosition.y;
            hoverTime = Random.Range(0f, 1f);
        }
    }
}