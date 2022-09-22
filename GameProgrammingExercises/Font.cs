using FreeTypeSharp;
using FreeTypeSharp.Native;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using static FreeTypeSharp.Native.FT;

namespace GameProgrammingExercises;

public class Font : IDisposable
{
    private readonly GL _gl;
    private readonly FreeTypeLibrary _lib;
    private readonly FreeTypeFaceFacade _face;
    private readonly IDictionary<char, Character> _characters = new Dictionary<char, Character>();

    private Font(GL gl, FreeTypeLibrary lib, FreeTypeFaceFacade face)
    {
        _gl = gl;
        _lib = lib;
        _face = face;
    }

    ~Font()
    {
        Dispose(false);
    }

    public static Font Load(string fileName, Game game)
    {
        var lib = new FreeTypeLibrary();

        var result = FT_New_Face(lib.Native, fileName, 0, out var facePtr);
        if (result != FT_Error.FT_Err_Ok)
        {
            throw new FreeTypeException(result);
        }

        var face = new FreeTypeFaceFacade(lib, facePtr);

        // Setting the width to 0 lets the face dynamically calculate the width based on the given height
        result = FT_Set_Pixel_Sizes(face.Face, 0, 48);
        if (result != FT_Error.FT_Err_Ok)
        {
            throw new FreeTypeException(result);
        }

        result = FT_Select_Charmap(face.Face, FT_Encoding.FT_ENCODING_UNICODE);
        if (result != FT_Error.FT_Err_Ok)
        {
            throw new FreeTypeException(result);
        }

        return new Font(game.Renderer.GL, lib, face);
    }

    public Character GetCharacter(char c)
    {
        if (!_characters.ContainsKey(c))
        {
            var ch = RenderGlyph(c);
            _characters.Add(c, ch);
        }
        return _characters[c];
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private unsafe Character RenderGlyph(char c)
    {
        var result = FT_Load_Char(_face.Face, c, FT_LOAD_RENDER);
        if (result != FT_Error.FT_Err_Ok)
        {
            throw new FreeTypeException(result);
        }

        var texture = Texture.CreateFromGlyph(_gl, _face.GlyphBitmap);
        return new Character(
            c,
            texture,
            new Vector2D<float>(_face.GlyphBitmap.width, _face.GlyphBitmap.rows),
            new Vector2D<float>(_face.FaceRec->glyph->bitmap_left, _face.FaceRec->glyph->bitmap_top),
            _face.FaceRec->glyph->advance.x.ToInt32()); //->advance.x));
    }

    private void ReleaseUnmanagedResources()
    {
        _lib.Dispose();
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
        }
    }
}