using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameEngine.Entities;

public class Camera
{
    private readonly Player _player;

    private float _distanceFromPlayer = 50.0f;
    private float _angleAroundPlayer = 0.0f;

    private Vector3D<float> _position;

    private Vector2D<float> _lastMousePosition;
    private Vector2D<float> _deltaPosition;

    public Camera(Player player)
    {
        _player = player;
    }

    public Vector3D<float> Position
    {
        get => _position;
        set => _position = value;
    }

    public float Pitch { get; set; } = 20.0f;

    public float Yaw { get; set; }

    public float Roll { get; set; }

    public void Move(IKeyboard keyboard, IMouse mouse)
    {
        _deltaPosition = mouse.Position.ToGeneric() - _lastMousePosition;

        CalculateZoom(mouse);
        CalculatePitch(mouse);
        CalculateAngleAroundPlayer(mouse);

        float horizontalDistance = CalculateHorizontalDistance();
        float verticalDistance = CalculateVerticalDistance();
        CalculateCameraPosition(horizontalDistance, verticalDistance);
        Yaw = 180 - (_player.RotY + _angleAroundPlayer);

        _lastMousePosition = mouse.Position.ToGeneric();
    }

    private void CalculateCameraPosition(float horizDistance, float verticDistance)
    {
        var theta = _player.RotY + _angleAroundPlayer;
        var offsetX = horizDistance * Scalar.Sin(Scalar.DegreesToRadians(theta));
        var offsetZ = horizDistance * Scalar.Cos(Scalar.DegreesToRadians(theta));

        Position = new Vector3D<float>(
            _player.Position.X - offsetX,
            _player.Position.Y + verticDistance,
            _player.Position.Z - offsetZ);
    }

    private float CalculateHorizontalDistance()
    {
        return _distanceFromPlayer * Scalar.Cos(Scalar.DegreesToRadians(Pitch));
    }

    private float CalculateVerticalDistance()
    {
        return _distanceFromPlayer * Scalar.Sin(Scalar.DegreesToRadians(Pitch));
    }

    private void CalculateZoom(IMouse mouse)
    {
        float zoomLevel = mouse.ScrollWheels.First().Y * 0.1f;
        _distanceFromPlayer -= zoomLevel;
    }

    private void CalculatePitch(IMouse mouse)
    {
        if (mouse.IsButtonPressed(MouseButton.Right))
        {
            float pitchChange =_deltaPosition.Y * 0.1f;
            Pitch -= pitchChange;
        }
    }

    private void CalculateAngleAroundPlayer(IMouse mouse)
    {
        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            float angleChange = _deltaPosition.X * 0.3f;
            _angleAroundPlayer -= angleChange;
        }
    }
}