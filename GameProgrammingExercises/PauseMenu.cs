using Silk.NET.Input;

namespace GameProgrammingExercises;

public class PauseMenu : UIScreen
{
    public PauseMenu(Game game)
        : base(game)
    {
        Game.State = GameState.Paused;
        SetRelativeMouseMode(false);
        Title = Game.GetText("PauseTitle");
        AddButton(Game.GetText("ResumeButton"), Close);
        AddButton(Game.GetText("QuitButton"), () =>
        {
            _ = new DialogBox(Game, Game.GetText("QuitText"), () => { Game.State = GameState.Quit; });
        });
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