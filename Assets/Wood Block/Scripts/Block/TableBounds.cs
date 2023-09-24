using System;
using UnityEngine;

namespace WoodBlock
{
    public class TableBounds
    {
        private Camera _camera;

        public TableBounds(Camera camera)
        {
            _camera = camera;
            if (_camera == null) throw new NullReferenceException(nameof(_camera) + " is null!");
        }

        public Bounds CalculateBounds(float topOffsetPercent = 0,
            float rightOffsetPercent = 0,
            float bottomOffsetPercent = 0,
            float leftOffsetPercent = 0)
        {
            float cameraHeight = 2f * _camera.orthographicSize;
            float cameraWidth = cameraHeight * _camera.aspect;
            Vector3 cameraPos = _camera.transform.position;
            float xMin = cameraPos.x - cameraWidth / 2 * (1 + leftOffsetPercent / 100);
            float xMax = cameraPos.x + cameraWidth / 2 * (1 + rightOffsetPercent / 100);
            float yMin = cameraPos.y - cameraHeight / 2 * (1 + bottomOffsetPercent / 100);
            float yMax = cameraPos.y + cameraHeight / 2 * (1 + topOffsetPercent / 100);

            return new Bounds(xMin, xMax, yMin, yMax);
        }

        public Bounds CalculateCameraBoundsPixels(float topOffsetPixels,
            float rightOffsetPixels,
            float bottomOffsetPixels,
            float leftOffsetPixels)
        {
            Vector3 cameraPos = _camera.transform.position;
            float cameraHeight = 2f * _camera.orthographicSize;
            float cameraWidth = cameraHeight * _camera.aspect;
            float xMin = cameraPos.x - cameraWidth / 2 - leftOffsetPixels;
            float xMax = cameraPos.x + cameraWidth / 2 + rightOffsetPixels;
            float yMin = cameraPos.y - cameraHeight / 2 - bottomOffsetPixels;
            float yMax = cameraPos.y + cameraHeight / 2 + topOffsetPixels;

            return new Bounds(xMin, xMax, yMin, yMax);
        }
    }
}