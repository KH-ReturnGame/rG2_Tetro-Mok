using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Global
{
    public class BgHandler : MonoBehaviour
    {
        public Sprite[] bgSprites; // 새 이미지
        public float fadeDuration = 1f; // 페이드 지속 시간
        public Image uiImage;
        private bool _isInTransition;
        private int _nextIndex;

        private void Start()
        {
            _nextIndex = 1;
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
            var startColor = uiImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                uiImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            // 새 이미지로 교체
            uiImage.sprite = bgSprites[_nextIndex];

            // 새 이미지 페이드 인
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                uiImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            _isInTransition = false;

            _nextIndex++;
            if (_nextIndex >= bgSprites.Length) _nextIndex = 0;
        }
    }
}