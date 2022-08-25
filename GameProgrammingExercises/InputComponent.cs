using Silk.NET.Input;

namespace GameProgrammingExercises;

public class InputComponent : MoveComponent
{
    public InputComponent(Actor owner)
        : base(owner)
    {
    }
    
    public Key ForwardKey { get; set; }

    public Key BackKey { get; set; }

    public Key ClockwiseKey { get; set; }

    public Key CounterClockwiseKey { get; set; }

    public float MaxForwardSpeed { get; set; }

    public float MaxAngularSpeed { get; set; }

    public override void ProcessInput(InputState state)
    {
        // Calculate forward speed for MoveComponent
        float forwardSpeed = 0.0f;
    
        if (state.Keyboard.GetKeyValue(ForwardKey)) {
            forwardSpeed += MaxForwardSpeed;
        }
    
        if (state.Keyboard.GetKeyValue(BackKey)) {
            forwardSpeed -= MaxForwardSpeed;
        }

        ForwardSpeed = forwardSpeed;
    
        // Calculate angular speed for MoveComponent
        float angularSpeed = 0.0f;
    
        if (state.Keyboard.GetKeyValue(ClockwiseKey)) {
            angularSpeed += MaxAngularSpeed;
        }
    
        if (state.Keyboard.GetKeyValue(CounterClockwiseKey)) {
            angularSpeed -= MaxAngularSpeed;
        }
    
        AngularSpeed = angularSpeed;
    }
}