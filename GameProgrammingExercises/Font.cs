using System.Runtime.InteropServices;
using FreeTypeSharp;
using FreeTypeSharp.Native;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using static FreeTypeSharp.Native.FT;

namespace GameProgrammingExercises;

public class Font
{
    private readonly IReadOnlyDictionary<char, Character> _characters;

    private Font(IReadOnlyDictionary<char,Character> characters)
    {
        _characters = characters;
    }
    // private readonly Game _game;
    //
    // public Font(Game game)
    // {
    //     _game = game;
    // }

    public static unsafe Font Load(string fileName, Game game)
    {
        using var lib = new FreeTypeLibrary();

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

        FontCollection collection = new();
        FontFamily family = collection.Add(fileName);
        var font = family.CreateFont(48, FontStyle.Regular);

        // disable byte-alignment restriction
        game.Renderer.GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        var characters = new Dictionary<char, Character>();
        for (int c = 0; c < 128; c++)
        {
            result = FT_Load_Char(face.Face, (char) c, FT_LOAD_RENDER);
            if (result != FT_Error.FT_Err_Ok)
            {
                throw new FreeTypeException(result);
            }

            var glyph = font.GetGlyphs(new CodePoint(c), ColorFontSupport.None).First();

            var texture = new Texture(game.Renderer.GL, face);
            var character = new Character(
                (char) c,
                texture,
                new Vector2D<float>(face.GlyphBitmap.width, face.GlyphBitmap.rows),
                new Vector2D<float>(face.FaceRec->glyph->bitmap_left, face.FaceRec->glyph->bitmap_top),
                face.FaceRec->glyph->metrics.horiAdvance.ToInt32()); //->advance.x));
            characters.Add(character.Char, character);
        }
        return new Font(characters);
    }

    public Character GetCharacter(char c)
    {
        return _characters[c];
    }
}