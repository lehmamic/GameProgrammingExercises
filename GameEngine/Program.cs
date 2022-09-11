// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Guis;
using GameEngine.Models;
using GameEngine.ObjConverter;
using GameEngine.RenderEngine;
using GameEngine.Terrains;
using GameEngine.Textures;
using GameEngine.Toolbox;
using GameEngine.Water;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");
IInputContext input = null!;
IKeyboard primaryKeyboard = null!;
IMouse primaryMouse = null!;
Loader loader = null!;

Entity entity = null!;
Entity entity2 = null!;
Entity entity3 = null!;
List<Entity> entities = new();
List<Entity> normalMapEntities = new ();
Light sun = null!;
List<Light> lights = new();
List<Terrain> terrains = new();
List<GuiTexture> guis = new();

WaterTile water = null!;
List<WaterTile> waters = new();

Terrain terrain = null!;
Player player = null!;
Camera camera = null!;

MasterRenderer renderer = null!;
GuiRenderer guiRenderer = null!;
WaterRenderer waterRenderer = null!;
WaterFrameBuffers fbos = null!;

MousePicker picker = null!;

displayManager.Window.Load += () =>
{
    input = displayManager.Window.CreateInput();
    primaryKeyboard = input.Keyboards.First();
    primaryMouse = input.Mice.First();

    loader = new Loader(displayManager.GL);

    // **********TERRAIN TEXTURE STUFF**********
    var backgroundTexture = loader.LoadTerrainTexture("Assets/grassy2.png");
    var rTexture = loader.LoadTerrainTexture("Assets/mud.png");
    var gTexture = loader.LoadTerrainTexture("Assets/grassFlowers.png");
    var bTexture = loader.LoadTerrainTexture("Assets/path.png");

    var texturePack = new TerrainTexturePack(backgroundTexture, rTexture, gTexture, bTexture);
    var blendMap = loader.LoadTerrainTexture("Assets/blendMap.png");

    terrain = new Terrain(0, -1, loader, texturePack, blendMap, "Assets/heightmap.png");
    terrains.Add(terrain);

    // *****************************************

    var rocks = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/rocks.obj", loader),
        loader.LoadModelTexture("Assets/rocks.png"));
    
    var fern = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/fern.obj", loader),
        loader.LoadModelTexture("Assets/fern.png"));
    fern.Texture.NumberOfRows = 2;

    var pine = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/pine.obj", loader),
        loader.LoadModelTexture("Assets/pine.png"));

    fern.Texture.HasTransparency = true;
    
    var lamp = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/lamp.obj", loader),
        loader.LoadModelTexture("Assets/lamp.png"));
    lamp.Texture.UseFakeLighting = true;

    //******************NORMAL MAP MODELS************************

    TexturedModel barrelModel = new TexturedModel(
        NormalMappedObjLoader.LoadObj("Assets/barrel.obj", loader),
        loader.LoadModelTexture("Assets/barrel.png"),
        loader.LoadModelTexture("Assets/barrelNormal.png"));
    barrelModel.Texture.ShineDamper = 10;
    barrelModel.Texture.Reflectivity = 0.5f;
    
    TexturedModel crateModel = new TexturedModel(
        NormalMappedObjLoader.LoadObj("Assets/crate.obj", loader),
        loader.LoadModelTexture("Assets/crate.png"),
        loader.LoadModelTexture("Assets/crateNormal.png"));
    crateModel.Texture.ShineDamper = 10;
    crateModel.Texture.Reflectivity = 0.5f;
    
    TexturedModel boulderModel = new TexturedModel(NormalMappedObjLoader.LoadObj("Assets/boulder.obj", loader),
        loader.LoadModelTexture("Assets/boulder.png"),
        loader.LoadModelTexture("Assets/boulderNormal.png"));
    boulderModel.Texture.ShineDamper = 10;
    boulderModel.Texture.Reflectivity = 0.5f;

    //************ENTITIES*******************

    entity = new Entity(barrelModel, new Vector3D<float>(75, 10, -75), 0, 0, 0, 1f);
    entity2 = new Entity(boulderModel, new Vector3D<float>(85, 10, -75), 0, 0, 0, 1f);
    entity3 = new Entity(crateModel, new Vector3D<float>(65, 10, -75), 0, 0, 0, 0.04f);

    normalMapEntities.Add(entity);
    normalMapEntities.Add(entity2);
    normalMapEntities.Add(entity3);

    Random random = new Random(676452);
    for(int i = 0; i < 100; i++)
    {
        if (i % 3 == 0)
        {
            float x = random.NextSingle() * 150;
            float z = random.NextSingle() * -150;
            if ((x > 50 && x < 100) || (z < -50 && z > -100))
            {
            }
            else
            {
                float y = terrain.GetHeightOfTerrain(x, z);

                entities.Add(new Entity(fern, 3, new Vector3D<float>(x, y, z), 0, random.NextSingle() * 360, 0, 0.9f));
            }
        }
        if (i % 2 == 0)
        {
            float x = random.NextSingle() * 150;
            float z = random.NextSingle() * -150;
            if ((x > 50 && x < 100) || (z < -50 && z > -100)) {

            }
            else
            {
                float y = terrain.GetHeightOfTerrain(x, z);
                entities.Add(new Entity(pine, 1, new Vector3D<float>(x, y, z), 0, random.NextSingle() * 360, 0, random.NextSingle() * 0.6f + 0.8f));
            }
        }
    }

    // Add the rock below the terrain
    entities.Add(new Entity(rocks, new Vector3D<float>(75,4.6f,-75), 0,0,0, 75.0f));

    //*******************OTHER SETUP***************

    sun = new Light(new Vector3D<float>(10000.0f, 10000.0f, -10000.0f), new Vector3D<float>(1.3f, 1.3f, 1.3f));
    lights.Add(sun);

    renderer = new MasterRenderer(displayManager, loader);

    var bunnyModel = ObjLoader.LoadObjModel("Assets/person.obj", loader);
    var standfordBunny = new TexturedModel(bunnyModel, loader.LoadModelTexture("Assets/playerTexture.png"));

    player = new(standfordBunny, new Vector3D<float>(75, 0, -50), 0, 100, 0, 0.6f);
    entities.Add(player);
    camera = new Camera(player);
    guiRenderer = new GuiRenderer(displayManager, renderer, loader, Matrix4X4<float>.Identity);
    picker = new MousePicker(displayManager, camera, renderer.ProjectionMatrix, terrains[0]);

    // **********Water Renderer Set-up**********
    fbos = new WaterFrameBuffers(displayManager);
    waterRenderer = new WaterRenderer(displayManager, loader, renderer.ProjectionMatrix, fbos);
    water = new WaterTile(75.0f, -75.0f, 0);
    waters.Add(water);
    // *****************************************
};

displayManager.Window.Closing += () =>
{
    renderer.Dispose();
    guiRenderer.Dispose();
    loader.Dispose();
    waterRenderer.Dispose();
    fbos.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    if (primaryKeyboard.IsKeyPressed(Key.Escape))
    {
        displayManager.Close();
    }

    player.Move((float)deltaTime, terrain, primaryKeyboard);
    camera.Move(primaryKeyboard, primaryMouse);
    picker.Update(primaryMouse);

    entity.IncreaseRotation(0, 1, 0);
    entity2.IncreaseRotation(0, 1, 0);
    entity3.IncreaseRotation(0, 1, 0);
};

displayManager.Window.Render += (deltaTime) =>
{
    displayManager.GL.Enable(EnableCap.ClipDistance0);

    // render reflection texture
    fbos.BindReflectionFrameBuffer();
    float distance = 2 * (camera.Position.Y - water.Height);
    camera.Position = new Vector3D<float>(camera.Position.X, camera.Position.Y - distance, camera.Position.Z);
    camera.InvertPitch();
    renderer.RenderScene((float) deltaTime, entities, normalMapEntities, terrains, lights, camera, new Vector4D<float>(0, 1, 0, -water.Height + 1.0f));
    camera.Position = new Vector3D<float>(camera.Position.X, camera.Position.Y + distance, camera.Position.Z);
    camera.InvertPitch();
    
    // render refraction texture
    fbos.BindRefractionFrameBuffer();
    renderer.RenderScene((float) deltaTime, entities, normalMapEntities, terrains, lights, camera, new Vector4D<float>(0, -1, 0, water.Height));
    
    // render to screen
    displayManager.GL.Disable(EnableCap.ClipDistance0);
    fbos.UnbindCurrentFrameBuffer();
    renderer.RenderScene((float) deltaTime, entities, normalMapEntities, terrains, lights, camera, new Vector4D<float>(0, -1, 0, 100000));
    waterRenderer.Render((float) deltaTime, waters, camera, sun);
    guiRenderer.Render(guis);
};

displayManager.Window
    .Run();