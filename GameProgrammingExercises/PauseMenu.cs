using Silk.NET.Input;

namespace GameProgrammingExercises;

public class PauseMenu : UIScreen
{
    public PauseMenu(Game game)
        : base(game)
    {
        Game.State = GameState.Paused;
        Title = "PauseTitle";
        SetRelativeMouseMode(false);
    }

    public override void HandleKeyPress(InputState state)
    {
        base.HandleKeyPress(state);

        if (state.Keyboard.GetKeyState(Key.Escape) == ButtonState.Pressed)
        {
            Close();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SetRelativeMouseMode(true);
            Game.State = GameState.GamePlay;
        }

        base.Dispose(disposing);
    }
}