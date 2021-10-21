using UnityEngine;
namespace com.snake.framework
{
    namespace runtime
    {
        static public class RectTransformExtension
        {
            /// <summary>
            /// 设置默认RectTransform数值
            /// </summary>
            /// <param name="rectTransform"></param>
            static public void Identity(this RectTransform rectTransform)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;

                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.sizeDelta = Vector2.zero;

                rectTransform.localScale = Vector3.one;
            }
        }
    }
}