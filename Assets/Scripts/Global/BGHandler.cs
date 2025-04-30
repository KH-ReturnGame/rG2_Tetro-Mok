using System.Collections;
using UnityEngine;

namespace Global
{
    public class BgHandler : MonoBehaviour
    {
        [SerializeField] private GameObject bg;
        [SerializeField] private Sprite[] bgSprites;
        [SerializeField] private float[] scales;
        [SerializeField] private float[] posYs;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private SpriteRenderer bgRenderer;
        private bool _isInTransition;
        private int _nextIndex;

        private void Start()
        {
            _nextIndex = 0;
            bg.transform.localScale = new Vector3(scales[_nextIndex], scales[_nextIndex], scales[_nextIndex]);
            bg.transform.position = new Vector3(0, posYs[_nextIndex], 0);
            _nextIndex++;
            _isInTransition = false;
        }

        private void Update()
        {
            if (!_isInTransition) StartCoroutine(ChangeBg());
        }

        private IEnumerator ChangeBg()
        {
            _isInTransition = true;
            yield return new WaitForSeconds(10f);

            var elapsedTime = 0f;
            var startColor = bgRenderer.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                bgRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            bgRenderer.sprite = bgSprites[_nextIndex];
            bg.transform.localScale = new Vector3(scales[_nextIndex], scales[_nextIndex], scales[_nextIndex]);
            bg.transform.position = new Vector3(0, posYs[_nextIndex], 0);

            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                bgRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            _isInTransition = false;

            _nextIndex++;
            if (_nextIndex >= bgSprites.Length) _nextIndex = 0;
        }
    }
}