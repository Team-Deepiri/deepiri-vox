using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public enum BackgroundType
    {
        Space,
        City,
        Desert,
        Forest
    }

    public class BackgroundManager : MonoBehaviour
    {
        [Header("Backgrounds")]
        public BackgroundType currentBackground;
        public SpriteRenderer bgRenderer;
        public SpriteRenderer midgroundRenderer;
        public SpriteRenderer foregroundRenderer;

        [Header("Parallax")]
        public float starSpeed = 2f;
        public float bgSpeed = 0.5f;
        public float fgSpeed = 1.5f;

        [Header("Colors")]
        public Color spaceColor1 = new Color(0.1f, 0.04f, 0.18f);
        public Color spaceColor2 = new Color(0.18f, 0.11f, 0.31f);
        public Color cityColor1 = new Color(0.04f, 0.09f, 0.16f);
        public Color cityColor2 = new Color(0.1f, 0.16f, 0.28f);
        public Color desertColor1 = new Color(0.18f, 0.1f, 0.04f);
        public Color desertColor2 = new Color(0.31f, 0.17f, 0.11f);
        public Color forestColor1 = new Color(0.04f, 0.18f, 0.1f);
        public Color forestColor2 = new Color(0.11f, 0.31f, 0.18f);

        private GameObject[] stars;
        private float[] starSpeeds;
        private int starCount = 100;
        private GameObject starPrefab;

        void Awake()
        {
            starPrefab = Resources.Load<GameObject>("Prefabs/Star");
            SetupStars();
        }

        void SetupStars()
        {
            stars = new GameObject[starCount];
            starSpeeds = new float[starCount];

            for (int i = 0; i < starCount; i++)
            {
                Vector2 pos = new Vector2(Random.Range(-10f, 10f), Random.Range(-6f, 8f));
                GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, transform);
                float size = Random.Range(0.02f, 0.08f);
                star.transform.localScale = new Vector3(size, size, 1);
                stars[i] = star;
                starSpeeds[i] = Random.Range(0.5f, 2f);
            }
        }

        void Update()
        {
            UpdateStars();
        }

        void UpdateStars()
        {
            if (stars == null) return;

            for (int i = 0; i < starCount; i++)
            {
                if (stars[i])
                {
                    stars[i].transform.position += Vector3.down * starSpeeds[i] * Time.deltaTime;
                    
                    if (stars[i].transform.position.y < -7f)
                    {
                        stars[i].transform.position = new Vector3(
                            Random.Range(-10f, 10f), 
                            8f, 
                            0
                        );
                    }
                }
            }
        }

        public void SetBackground(BackgroundType type)
        {
            currentBackground = type;
            
            Color c1, c2;
            
            switch (type)
            {
                case BackgroundType.Space:
                    c1 = spaceColor1;
                    c2 = spaceColor2;
                    break;
                case BackgroundType.City:
                    c1 = cityColor1;
                    c2 = cityColor2;
                    break;
                case BackgroundType.Desert:
                    c1 = desertColor1;
                    c2 = desertColor2;
                    break;
                case BackgroundType.Forest:
                    c1 = forestColor1;
                    c2 = forestColor2;
                    break;
                default:
                    c1 = spaceColor1;
                    c2 = spaceColor2;
                    break;
            }

            StartCoroutine(TransitionColors(c1, c2));
        }

        IEnumerator TransitionColors(Color target1, Color target2)
        {
            float t = 0;
            Color start1 = bgRenderer.color;
            Color start2 = midgroundRenderer ? midgroundRenderer.color : target2;

            while (t < 1)
            {
                t += Time.deltaTime * 2;
                bgRenderer.color = Color.Lerp(start1, target1, t);
                if (midgroundRenderer)
                    midgroundRenderer.color = Color.Lerp(start2, target2, t);
                yield return null;
            }
        }

        public BackgroundType GetNextBackground()
        {
            return (BackgroundType)(((int)currentBackground + 1) % 4);
        }

        public void Clear()
        {
            if (stars != null)
            {
                foreach (var star in stars)
                {
                    if (star) Destroy(star);
                }
            }
        }
    }
}