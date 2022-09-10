using GameEngine.Entities;
using GameEngine.RenderEngine;
using GameEngine.Terrains;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameEngine.Toolbox;

public class MousePicker
{
    private const int RECURSION_COUNT = 200;
    private const float RAY_RANGE = 600;
    
    private readonly DisplayManager _displayManager;
    private readonly Matrix4X4<float> _projectionMatrix;
    private readonly Terrain _terrain;
    private readonly Camera _camera;

    private Matrix4X4<float> _viewMatrix;
    private Vector3D<float> _currentRay;
    private Vector3D<float>? _currentTerrainPoint;

    public MousePicker(DisplayManager displayManager, Camera camera, Matrix4X4<float> projectionMatrix, Terrain terrain)
    {
        _displayManager = displayManager;
        _projectionMatrix = projectionMatrix;
        _terrain = terrain;
        _camera = camera;
        _viewMatrix = Maths.CreateViewMatrix(_camera);
    }

    public Vector3D<float> CurrentRay => _currentRay;

    public Vector3D<float>? CurrentTerrainPoint => _currentTerrainPoint;

    public void Update(IMouse mouse)
    {
        _viewMatrix = Maths.CreateViewMatrix(_camera);
        _currentRay = CalculateMouseRay(mouse);

        if (IntersectionInRange(0, RAY_RANGE, _currentRay)) {
            _currentTerrainPoint = BinarySearch(0, 0, RAY_RANGE, _currentRay);
        } else {
            _currentTerrainPoint = null;
        }
    }

    private Vector3D<float> CalculateMouseRay(IMouse mouse)
    {
        float mouseX = mouse.Position.X;
        float mouseY = mouse.Position.Y;
        var normalizedCoords = GetNormalizedDeviceCoords(mouseX, mouseY);
        var clipCoords = new Vector4D<float>(normalizedCoords.X, normalizedCoords.Y, -1.0f, 1.0f);
        var eyeCoords = ToEyeCoords(clipCoords);
        var worldRay = ToWorldCoords(eyeCoords);
        return worldRay;
    }

    private Vector2D<float> GetNormalizedDeviceCoords(float mouseX, float mouseY)
    {
        float x = (2.0f * mouseX) / _displayManager.Width - 1;
        float y = (2.0f * mouseY) / _displayManager.Height - 1;

        return new Vector2D<float>(x, -y);
    }

    private Vector4D<float> ToEyeCoords(Vector4D<float> clipCoords)
    {
        Matrix4X4.Invert(_projectionMatrix, out var invertedProjection);
        var eyeCoords = Vector4D.Transform(clipCoords, invertedProjection);
        return new Vector4D<float>(eyeCoords.X, eyeCoords.Y, -1.0f, 0.0f);
    }

    private Vector3D<float> ToWorldCoords(Vector4D<float> eyeCoords)
    {
        Matrix4X4.Invert(_viewMatrix, out var invertedViewMatrix);
        var rayWorld = Vector4D.Transform(eyeCoords, invertedViewMatrix);
        var mouseRay =  new Vector3D<float>(rayWorld.X, rayWorld.Y, rayWorld.Z);
        mouseRay = Vector3D.Normalize(mouseRay);

        return mouseRay;
    }
    
    //**********************************************************
    private Vector3D<float> GetPointOnRay(Vector3D<float> ray, float distance) {
        Vector3D<float> camPos = _camera.Position;
        Vector3D<float> start = new Vector3D<float>(camPos.X, camPos.Y, camPos.Z);
        Vector3D<float> scaledRay = new Vector3D<float>(ray.X * distance, ray.Y * distance, ray.Z * distance);
        return start + scaledRay;
    }

    private Vector3D<float>? BinarySearch(int count, float start, float finish, Vector3D<float> ray) {
        float half = start + ((finish - start) / 2f);
        if (count >= RECURSION_COUNT)
        {
            Vector3D<float> endPoint = GetPointOnRay(ray, half);
            Terrain? terrain = GetTerrain(endPoint.X, endPoint.Z);
            if (terrain != null)
            {
                return endPoint;
            }
            else
            {
                return null;
            }
        }

        if (IntersectionInRange(start, half, ray))
        {
            return BinarySearch(count + 1, start, half, ray);
        }
        else
        {
            return BinarySearch(count + 1, half, finish, ray);
        }
    }

    private bool IntersectionInRange(float start, float finish, Vector3D<float> ray) {
        Vector3D<float> startPoint = GetPointOnRay(ray, start);
        Vector3D<float> endPoint = GetPointOnRay(ray, finish);
        if (!IsUnderGround(startPoint) && IsUnderGround(endPoint))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsUnderGround(Vector3D<float> testPoint) {
        Terrain? terrain = GetTerrain(testPoint.X, testPoint.Z);
        float height = 0;

        if (terrain != null) {
            height = terrain.GetHeightOfTerrain(testPoint.X, testPoint.Z);
        }

        if (testPoint.Y < height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Terrain? GetTerrain(float worldX, float worldZ)
    {
        // for multiple terrains
        // int x = worldX / Terrain.Size;
        // int z = worldZ / Terrain.Size;
        // return _terrains[x][z];
        return _terrain;
    }
}