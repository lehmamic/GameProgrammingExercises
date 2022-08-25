using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class AudioComponent : Component
{
    private readonly List<SoundEvent> _events2D = new();
    private readonly List<SoundEvent> _events3D = new();

    public AudioComponent(Actor owner, int updateOrder = 200)
        : base(owner, updateOrder)
    {
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Remove invalid 2D events
        foreach(var e in _events2D.ToArray())
        {
            if (!e.IsValid())
            {
                _events2D.Remove(e);
            }
        }

        // Remove invalid 3D events
        foreach(var e in _events3D.ToArray())
        {
            if (!e.IsValid())
            {
                _events3D.Remove(e);
            }
        }
    }

    public override void OnUpdateWorldTransform()
    {
        Matrix4X4<float> world = Owner.WorldTransform;
        foreach(var e in _events3D)
        {
            if (e.IsValid())
            {
                e.Set3DAttributes(world);
            }
        }
    }

    public SoundEvent PlayEvent(string name)
    {
        SoundEvent e = Owner.Game.AudioSystem.PlayEvent(name);
    
        // Is this 2D or 3D?
        if (e.Is3D())
        {
            _events3D.Add(e);
        
            // Set initial 3D attributes
            e.Set3DAttributes(Owner.WorldTransform);
        }
        else
        {
            _events2D.Add(e);
        }
    
        return e;
    }

    public void StopAllEvents()
    {
        // Stop all sounds
        foreach (var e in _events2D)
        {
            e.Stop();
        }
    
        foreach (var e in _events3D)
        {
            e.Stop();
        }
    
        // Clear events
        _events2D.Clear();
        _events3D.Clear();
    }
}