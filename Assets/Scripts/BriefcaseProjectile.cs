using UnityEngine;

namespace TSEFatal
{
    public class BriefcaseProjectile : MonoBehaviour
    {
        private FighterController target;
        private int direction;
        private float lifetime = 1.2f;

        public static void Launch(Vector3 position, FighterController target, int direction)
        {
            var projectile = new GameObject("Maleta Meteoro").AddComponent<BriefcaseProjectile>();
            projectile.transform.position = position;
            projectile.target = target;
            projectile.direction = direction;

            var renderer = projectile.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            renderer.color = new Color(.14f, .12f, .10f);
            renderer.sortingOrder = 5;
            projectile.transform.localScale = new Vector3(1.15f, .7f, 1f);
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;
            transform.position += new Vector3(direction * 9f * Time.deltaTime, Mathf.Sin(Time.time * 14f) * .025f, 0f);
            transform.Rotate(0f, 0f, -direction * 720f * Time.deltaTime);

            Vector3 targetPoint = target == null ? Vector3.zero : target.transform.position + Vector3.up;
            if (target != null && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetPoint.x, targetPoint.y)) < 1.25f)
            {
                target.TakeDamage(25f, direction);
                Destroy(gameObject);
            }
            else if (lifetime <= 0f) Destroy(gameObject);
        }
    }
}
