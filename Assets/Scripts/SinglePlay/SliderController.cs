using UnityEngine;
using UnityEngine.UI;

namespace SinglePlay
{
    public class SliderController : MonoBehaviour
    {
        private static Scrollbar _staticScrollbar;
        private static GameObject _staticScrollbarGameObject;
        [SerializeField] private GameObject scrollbarGameObject;
        [SerializeField] private Scrollbar scrollbar;

        private void Start()
        {
            _staticScrollbar = scrollbar;
            _staticScrollbarGameObject = scrollbarGameObject;

            SetScrollbarVisible(false);
        }

        public static void SetScrollbarValue(float value)
        {
            _staticScrollbar.value = Mathf.Clamp01(value);
            Debug.Log("진행 상황: " + Mathf.Round(_staticScrollbar.value * 100) + " %");
        }

        public static void SetScrollbarVisible(bool isVisible)
        {
            _staticScrollbarGameObject.SetActive(isVisible);
        }
    }
}