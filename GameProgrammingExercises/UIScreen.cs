namespace GameProgrammingExercises;

public class UIScreen : IDisposable
{
    private readonly Game _game;
    private UIScreenState _state;

    public UIScreen(Game game)
    {
        _game = game;
    }

    public UIScreenState State => _state;

    public virtual void Update(float deltaTime)
    {
        throw new NotImplementedException();
    }

    public virtual void Draw(Shader shader)
    {
        
    }

    public virtual void ProcessInput(InputState state)
    {
        
    }

    protected virtual void HandleKeyPress(InputState state)
    {
        
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