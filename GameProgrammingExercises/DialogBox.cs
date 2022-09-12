using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class DialogBox : UIScreen
{
    public DialogBox(Game game, string text, Action onOK) : base(game)
    {
        BgPos = Vector2D<float>.Zero;
        NextButtonPos = Vector2D<float>.Zero;
        Background = Game.Renderer.GetTexture("Assets/DialogBG.png");
        Title = text;
        TitlePos = new Vector2D<float>(0.0f, 100.0f);
        TitleScale = 0.625f;
        TitleColor = Color.Black;
        AddButton(Game.GetText("OKButton"), onOK);
        AddButton(Game.GetText("CancelButton"), Close);
    }
}