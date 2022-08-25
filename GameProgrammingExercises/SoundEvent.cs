using FMOD;
using FMOD.Studio;
using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class SoundEvent
{
    private readonly AudioSystem _system;
    private readonly uint _id;

    public SoundEvent(AudioSystem system, uint id)
    {
        _system = system;
        _id = id;
    }

    public bool IsValid()
    {
        return _system.GetEventInstance(_id) is not null;
    }

    public void Restart()
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.start();
        }
    }

    public void Stop(bool allowFadeOut /* true */)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            var mode = allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
            eventInstance.Value.stop(mode);
        }
    }

    public void SetPaused(bool pause)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.setPaused(pause);
        }
    }

    public void SetVolume(float value)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.setVolume(value);
        }
    }

    public void SetPitch(float value)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.setPitch(value);
        }
    }

    public void SetParameter(string name, float value)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.setParameterByName(name, value);
        }
    }

    public bool GetPaused()
    {
        bool retVal = false;

        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.getPaused(out retVal);
        }

        return retVal;
    }

    public float GetVolume()
    {
        float retVal = 0.0f;

        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.getVolume(out retVal);
        }

        return retVal;
    }

    public float GetPitch()
    {
        float retVal = 0.0f;

        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.getPitch(out retVal);
        }

        return retVal;
    }

    public float GetParameter(string name)
    {
        float retVal = 0.0f;

        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.getParameterByName(name, out retVal);
        }

        return retVal;
    }

    public bool Is3D()
    {
        bool retVal = false;

        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            eventInstance.Value.getDescription(out var description);
            if (description.isValid())
            {
                description.is3D(out retVal);
            }
        }

        return retVal;
    }

    public void Set3DAttributes(Matrix4X4<float> worldTrans)
    {
        var eventInstance = _system.GetEventInstance(_id);
        if (eventInstance is not null)
        {
            var attr = new ATTRIBUTES_3D
            {
                // Set position, forward, up
                position = worldTrans.GetTranslation().VecToFMOD(),

                // In world transform, first row is forward
                forward = worldTrans.GetXAxis().VecToFMOD(),

                // Third row is up
                up = worldTrans.GetZAxis().VecToFMOD(),

                // Set velocity to zero (fix is using Doppler effect)
                velocity = new VECTOR {x = 0.0f, y = 0.0f, z = 0.0f},
            };

            eventInstance.Value.set3DAttributes(attr);
        }
    }
}