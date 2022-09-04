using GameProgrammingExercises.Maths;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class UIScreen : IDisposable
{
    private readonly Game _game;
    private readonly Font _font;
    private readonly Texture _buttonOn;
    private readonly Texture _buttonOff;

    // State
    private UIScreenState _state;

    private Texture? _background;
    
    // Configure positions
    private Vector2D<float> _nextButtonPos = new(0.0f, 200.0f);
    private Vector2D<float> _bgPos = new(0.0f, 250.0f);

    public UIScreen(Game game)
    {
        _game = game;
        
        // Add to UI Stack
        Game.PushUI(this);
        _font = Game.GetFont("Assets/Antonio-Regular.ttf");
        _buttonOn = Game.Renderer.GetTexture("Assets/ButtonYellow.png");
        _buttonOff = Game.Renderer.GetTexture("Assets/ButtonBlue.png");
    }


    public Game Game => _game;

    public UIScreenState State => _state;

    public string? Title { get; set; }

    public Vector2D<float> TitlePos { get; set; } = new(0.0f, 300.0f);

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void Draw(Shader shader)
    {
        // Draw background (if exists)
        if (_background is not null)
        {
            Game.Renderer.DrawTexture(_background, _bgPos);
        }
        // Draw title (if exists)
        if (Title is not null)
        {
            Game.Renderer.DrawText(_font, Title, TitlePos, 1.0f, Color.White);
        }
        // Draw buttons
        // for (auto b : mButtons)
        // {
        //     // Draw background of button
        //     Texture* tex = b->GetHighlighted() ? mButtonOn : mButtonOff;
        //     DrawTexture(shader, tex, b->GetPosition());
        //     // Draw text of button
        //     DrawTexture(shader, b->GetNameTex(), b->GetPosition());
        // }
        // Override in subclasses to draw any textures
    }

    public virtual void ProcessInput(InputState state)
    {
        // // Do we have buttons?
        // if (!mButtons.empty())
        // {
        //     // Get position of mouse
        //     int x, y;
        //     SDL_GetMouseState(&x, &y);
        //     // Convert to (0,0) center coordinates
        //     Vector2 mousePos(static_cast<float>(x), static_cast<float>(y));
        //     mousePos.x -= mGame->GetRenderer()->GetScreenWidth() * 0.5f;
        //     mousePos.y = mGame->GetRenderer()->GetScreenHeight() * 0.5f - mousePos.y;
		      //
        //     // Highlight any buttons
        //     for (auto b : mButtons)
        //     {
        //         if (b->ContainsPoint(mousePos))
        //         {
        //             b->SetHighlighted(true);
        //         }
        //         else
        //         {
        //             b->SetHighlighted(false);
        //         }
        //     }
        // }
    }

    public void Close()
    {
        _state = UIScreenState.Closing;
    }

    public virtual void HandleKeyPress(InputState state)
    {
        
    }

    protected void SetRelativeMouseMode(bool relative)
    {
        Game.InputSystem.SetRelativeMouseMode(relative);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}