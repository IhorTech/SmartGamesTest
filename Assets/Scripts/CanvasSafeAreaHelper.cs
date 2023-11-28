using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//source: https://forum.unity.com/threads/canvashelper-resizes-a-recttransform-to-iphone-xs-safe-area.521107

namespace Tools.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasSafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;

        [SerializeField] private RectTransform _safeAreaTransform;
        
        private ScreenOrientation _lastOrientation;
        private Vector2Int _lastResolution;
        private Rect _lastSafeArea;

        void Awake()
        {
            _lastOrientation = Screen.orientation;
            _lastResolution.x = Screen.width;
            _lastResolution.y = Screen.height;
            _lastSafeArea = Screen.safeArea;

            ApplySafeArea();
        }

        void Update()
        {
            if (Application.isMobilePlatform && Screen.orientation != _lastOrientation)
                OrientationChanged();

            if (Screen.safeArea != _lastSafeArea)
                SafeAreaChanged();

            if (Screen.width != _lastResolution.x || Screen.height != _lastResolution.y)
                ResolutionChanged();
        }

        void ApplySafeArea()
        {
            if (_safeAreaTransform == null)
                return;
            
            Rect safeArea = Screen.safeArea;
            
            if (safeArea.width == 0f || safeArea.height == 0f)
                return;
            
            float canvasScale = _canvasScaler.referenceResolution.y / safeArea.height;
            float canvasWidth = _canvas.pixelRect.width;

            float left = canvasScale * safeArea.position.x;
            float right = canvasScale * canvasWidth * (1 - (safeArea.position.x + safeArea.size.x) / canvasWidth);

            float symmetricalOffset = Mathf.Max(left, right);
            if (symmetricalOffset < 1)
            {
                symmetricalOffset = canvasScale < 1 ? Screen.dpi * 0.2f * canvasScale : Screen.dpi * 0.2f;
            }

            _safeAreaTransform.offsetMin = new Vector2(symmetricalOffset, 0);
            _safeAreaTransform.offsetMax = new Vector2(-symmetricalOffset, 0);
        }

        private void OrientationChanged()
        {
            _lastOrientation = Screen.orientation;
            _lastResolution.x = Screen.width;
            _lastResolution.y = Screen.height;
        }

        private void ResolutionChanged()
        {
            _lastResolution.x = Screen.width;
            _lastResolution.y = Screen.height;
        }

        private void SafeAreaChanged()
        {
            _lastSafeArea = Screen.safeArea;

            ApplySafeArea();
        }
    }
}