using Silk.NET.Input;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class AudioActor : Actor
{
    private SoundEvent _musicEvent;
    private SoundEvent? _reverbSnap;

    public AudioActor(Game game)
        : base(game)
    {
        // Start music
        _musicEvent = Game.AudioSystem.PlayEvent("event:/Music");
    }

    protected override void ActorInput(InputState state)
    {
        if (state.Keyboard.GetKeyState(Key.Minus) == ButtonState.Pressed)
        {
            float volume = Game.AudioSystem.GetBusVolume("bus:/");
            volume = Scalar.Max(0.0f, volume - 0.1f);
            Game.AudioSystem.SetBusVolume("bus:/", volume);
        }

        if (state.Keyboard.GetKeyState(Key.Equal) == ButtonState.Pressed)
        {
            // Increase master volume
            float volume = Game.AudioSystem.GetBusVolume("bus:/");
            volume = Scalar.Min(1.0f, volume + 0.1f);
            Game.AudioSystem.SetBusVolume("bus:/", volume);
        }

        if (state.Keyboard.GetKeyState(Key.E) == ButtonState.Pressed)
        {
            // Play explosion
            Game.AudioSystem.PlayEvent("event:/Explosion2D");
        }

        if (state.Keyboard.GetKeyState(Key.M) == ButtonState.Pressed)
        {
            // Toggle music pause state
            _musicEvent.SetPaused(!_musicEvent.GetPaused());
        }
        
        if (state.Keyboard.GetKeyState(Key.R) == ButtonState.Pressed)
        {
            // Stop or start reverb snapshot
            if (_reverbSnap is null || !_reverbSnap.IsValid())
            {
                _reverbSnap = Game.AudioSystem.PlayEvent("snapshot:/WithReverb");
            }
            else
            {
                _reverbSnap.Stop();
            }
        }
        
        if (state.Keyboard.GetKeyState(Key.Number1) == ButtonState.Pressed)
        {
            // Set default footstep surface
            Game.Camera.SetFootstepSurface(0.0f);
        }
        
        if (state.Keyboard.GetKeyState(Key.Number2) == ButtonState.Pressed)
        {
            // Set grass footstep surface
            Game.Camera.SetFootstepSurface(0.5f);
        }
    }
}