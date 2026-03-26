using UnityEngine;

namespace KimicuUtility
{
    public static class KiCameraExtension
    {
        private static Camera _camera;
        private static bool _isCameraDestroyed = false;

        private static Camera Camera
        {
            get
            {
                // Проверяем, не уничтожена ли камера
                if (_isCameraDestroyed || !IsCameraValid(_camera))
                {
                    _camera = null;
                    _isCameraDestroyed = false;
                }

                TryInitializeCamera();
                return _camera;
            }
            set
            {
                _camera = value;
                _isCameraDestroyed = false;
            }
        }

        private static bool IsCameraValid(Camera cam)
        {
            // Проверяем, что камера существует и не уничтожена
            return cam != null && cam.gameObject != null;
        }

        private static void TryInitializeCamera()
        {
            // Если камера уже установлена и валидна, ничего не делаем
            if (_camera != null && IsCameraValid(_camera))
                return;

            // Ищем главную камеру
            var mainCamera = Camera.main;

            // Если главная камера существует и валидна, используем её
            if (mainCamera != null && IsCameraValid(mainCamera))
            {
                _camera = mainCamera;
                return;
            }

            // Ищем любую активную камеру в сцене
            var allCameras = Camera.allCameras;
            foreach (var camera in allCameras)
            {
                if (camera != null && camera.enabled && IsCameraValid(camera))
                {
                    _camera = camera;
                    return;
                }
            }

            // Если камера не найдена, пробуем создать её
            _camera = CreateFallbackCamera();
        }

        private static Camera CreateFallbackCamera()
        {
            // Создаем простую камеру как запасной вариант
            var cameraObject = new GameObject("FallbackCamera")
            {
                tag = "MainCamera"
            };

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;

            // Отмечаем, что это временная камера
            Object.DontDestroyOnLoad(cameraObject);

            return camera;
        }

        /// <summary> Sets the position of the transform in world space based on a screen position. </summary>
        public static void SetWorldSpace<T>(this T component, Vector2 screenPosition, float z = -1) where T : Component
        {
            if (component == null)
            {
                Debug.LogWarning("Component is null in SetWorldSpace");
                return;
            }

            var camera = Camera;
            if (camera == null)
            {
                Debug.LogWarning("Camera is null in SetWorldSpace");
                return;
            }

            if (z == -1) z = camera.nearClipPlane;
            Vector3 position = new(screenPosition.x, screenPosition.y, z);

            // Дополнительная проверка перед использованием камеры
            if (!IsCameraValid(camera))
            {
                _camera = null;
                _isCameraDestroyed = true;
                return;
            }

            component.transform.position = camera.ScreenToWorldPoint(position);
        }

        /// <summary> Converts a 2D screen position to a 3D world position. </summary>
        public static Vector3 GetWorldSpace(this Vector2 screenPosition, float z = -1)
        {
            var camera = Camera;
            if (camera == null || !IsCameraValid(camera))
            {
                Debug.LogWarning("Camera is null or invalid in GetWorldSpace");
                return Vector3.zero;
            }

            if (z == -1) z = camera.nearClipPlane;

            // Дополнительная проверка
            if (!IsCameraValid(camera))
            {
                _camera = null;
                _isCameraDestroyed = true;
                return Vector3.zero;
            }

            return camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, z));
        }

        /// <summary> Converts a screen position to a world space position using the camera. </summary>
        public static Vector3 GetWorldSpace(this Vector3 screenPosition)
        {
            var camera = Camera;
            if (camera == null || !IsCameraValid(camera))
            {
                Debug.LogWarning("Camera is null or invalid in GetWorldSpace");
                return Vector3.zero;
            }

            // Дополнительная проверка
            if (!IsCameraValid(camera))
            {
                _camera = null;
                _isCameraDestroyed = true;
                return Vector3.zero;
            }

            return camera.ScreenToWorldPoint(screenPosition);
        }

        /// <summary> Returns a ray from a screen point. </summary>
        public static Ray GetScreenPointToRay(this Vector2 screenPosition)
        {
            var camera = Camera;
            if (camera == null || !IsCameraValid(camera))
            {
                Debug.LogWarning("Camera is null or invalid in GetScreenPointToRay");
                return new Ray(Vector3.zero, Vector3.forward);
            }

            // Дополнительная проверка
            if (!IsCameraValid(camera))
            {
                _camera = null;
                _isCameraDestroyed = true;
                return new Ray(Vector3.zero, Vector3.forward);
            }
            return camera.ScreenPointToRay(screenPosition);
        }

        /// <summary> Returns a ray going from camera through a screen point. </summary>
        public static Ray GetScreenPointToRay(this Vector3 screenPosition)
            => GetScreenPointToRay((Vector2)screenPosition);

        // Остальные методы остаются без изменений
        public static Bounds GetBoundsPercent(this Camera camera, float top, float right, float bottom, float left)
            => KiMath.GetBoundsPercent(camera, top, right, bottom, left);

        public static Bounds GetBoundsUnit(this Camera camera, float top, float right, float bottom, float left)
            => KiMath.GetBoundsUnit(camera, top, right, bottom, left);

        /// <summary>
        /// Метод для явной очистки кэшированной камеры
        /// Вызывайте этот метод при смене сцены или уничтожении камеры
        /// </summary>
        public static void ClearCameraCache()
        {
            _camera = null;
            _isCameraDestroyed = false;
        }
    }
}