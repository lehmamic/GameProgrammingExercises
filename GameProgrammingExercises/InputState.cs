namespace GameProgrammingExercises;

public class InputState
{
    public InputState(KeyboardState keyboard, MouseState mouse)
    {
        Keyboard = keyboard;
        Mouse = mouse;
    }

    public KeyboardState Keyboard { get; }
    
    public MouseState Mouse { get; }
}