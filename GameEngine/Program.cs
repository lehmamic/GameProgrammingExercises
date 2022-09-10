// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Guis;
using GameEngine.Models;
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

List<Entity> entities = new();
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
    var backgroundTexture = loader.LoadTerrainTexture("Assets/grassy.png");
    var rTexture = loader.LoadTerrainTexture("Assets/dirt.png");
    var gTexture = loader.LoadTerrainTexture("Assets/pinkFlowers.png");
    var bTexture = loader.LoadTerrainTexture("Assets/path.png");

    var texturePack = new TerrainTexturePack(backgroundTexture, rTexture, gTexture, bTexture);
    var blendMap = loader.LoadTerrainTexture("Assets/blendmap_water.png");
    
    terrain = new Terrain(0, -1, loader, texturePack, blendMap, "Assets/heightmap_water.png");
    terrains.Add(terrain);
    // *****************************************

    // var data = ObjFileLoader.LoadObj("Assets/tree.obj");
    // var treeModel = loader.LoadToVAO(data.Vertices, data.Indices);

    // var staticModel = new TexturedModel(treeModel, loader.LoadModelTexture("Assets/tree.png"));
    var grass = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/grassModel.obj", loader),
        loader.LoadModelTexture("Assets/grassTexture.png"));
    // var flower = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/grassModel.obj", loader),
    //     loader.LoadModelTexture("Assets/flower.png"));
    var pine = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/pine.obj", loader),
        loader.LoadModelTexture("Assets/pine.png"));

    // var fernTextureAtlas = loader.LoadModelTexture("Assets/fern.png");
    // fernTextureAtlas.NumberOfRows = 2;
    // var fern = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/fern.obj", loader),
    //     fernTextureAtlas);

    // var lowPolyTreeTextureAtlas = loader.LoadModelTexture("Assets/lowPolyTree.png");
    // lowPolyTreeTextureAtlas.NumberOfRows = 2;
    // var bobble = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/lowPolyTree.obj", loader),
    //     lowPolyTreeTextureAtlas);
    // var box = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/box.obj", loader),
    //     loader.LoadModelTexture("Assets/box.png"));
    // var box2 = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/box.obj", loader),
    //     loader.LoadModelTexture("Assets/box.png"));
    // var lamp = new TexturedModel(
    //     ObjLoader.LoadObjModel("Assets/lamp.obj", loader),
    //     loader.LoadModelTexture("Assets/lamp.png"));
    
    grass.Texture.HasTransparency = true;
    grass.Texture.UseFakeLighting = true;
    grass.Texture.HasTransparency = true;
    grass.Texture.UseFakeLighting = true;
    // fern.Texture.HasTransparency = true;
    // lamp.Texture.UseFakeLighting = true;
    
    var x = 20;
    var z = -80;
    var y = terrain.GetHeightOfTerrain(x, z);
    entities.Add(new Entity(pine, 0, new Vector3D<float>(x,y,z),0,0,0,1.4f));
    x = 140;
    z = -80;
    y = terrain.GetHeightOfTerrain(x, z);
    entities.Add(new Entity(pine, 0, new Vector3D<float>(x,y,z),0,0,0,1.4f));

    sun = new Light(new Vector3D<float>(0.0f, 10000.0f, -7000.0f), new Vector3D<float>(0.4f, 0.4f, 0.4f));
    lights.Add(sun);

    renderer = new MasterRenderer(displayManager, loader);
    guiRenderer = new GuiRenderer(displayManager, renderer, loader, Matrix4X4<float>.Identity);
    
    // **********Water Renderer Set-up**********
    fbos = new WaterFrameBuffers(displayManager);
    waterRenderer = new WaterRenderer(displayManager, loader, renderer.ProjectionMatrix, fbos);
    water = new WaterTile(75.0f, -75.0f, 0);
    waters.Add(water);
    // *****************************************

    var bunnyModel = ObjLoader.LoadObjModel("Assets/person.obj", loader);
    var standfordBunny = new TexturedModel(bunnyModel, loader.LoadModelTexture("Assets/playerTexture.png"));

    player = new(standfordBunny, new Vector3D<float>(100, 0, -50), 0, 180, 0, 0.6f);
    camera = new Camera(player);
    picker = new MousePicker(displayManager, camera, renderer.ProjectionMatrix, terrains[0]);
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
};

displayManager.Window.Render += (deltaTime) =>
{
    displayManager.GL.Enable(EnableCap.ClipDistance0);

    // render reflection texture
    fbos.BindReflectionFrameBuffer();
    float distance = 2 * (camera.Position.Y - water.Height);
    camera.Position = new Vector3D<float>(camera.Position.X, camera.Position.Y - distance, camera.Position.Z);
    camera.InvertPitch();
    renderer.RenderScene((float) deltaTime, entities, terrains, lights, camera, new Vector4D<float>(0, 1, 0, -water.Height));
    camera.Position = new Vector3D<float>(camera.Position.X, camera.Position.Y + distance, camera.Position.Z);
    camera.InvertPitch();
    
    // render refraction texture
    fbos.BindRefractionFrameBuffer();
    renderer.RenderScene((float) deltaTime, entities, terrains, lights, camera, new Vector4D<float>(0, -1, 0, water.Height));
    
    // render to screen
    displayManager.GL.Disable(EnableCap.ClipDistance0);
    fbos.UnbindCurrentFrameBuffer();
    renderer.RenderScene((float) deltaTime, entities, terrains, lights, camera, new Vector4D<float>(0, -1, 0, 100000));
    waterRenderer.Render((float) deltaTime, waters, camera, sun);
    guiRenderer.Render(guis);
};

displayManager.Window
    .Run();