// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Models;
using GameEngine.RenderEngine;
using GameEngine.Terrains;
using GameEngine.Textures;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using var displayManager = new DisplayManager(1024, 768, "OpenGL 3D Game Programming Tutorials");
IInputContext input = null!;
IKeyboard primaryKeyboard = null!;
IMouse primaryMouse = null!;
Loader loader = null!;

List<Entity> entities = new();
Light light = new Light(new Vector3D<float>(20000.0f, 40000.0f, 20000.0f), new Vector3D<float>(1.0f, 1.0f, 1.0f));

Terrain terrain = null!;
Terrain terrain2 = null!;

Camera camera = null!;
MasterRenderer renderer = null!;

Player player = null!;

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
    var blendMap = loader.LoadTerrainTexture("Assets/blendMap.png");
    // *****************************************

    var data = ObjFileLoader.LoadObj("Assets/tree.obj");
    var treeModel = loader.LoadToVAO(data.Vertices, data.Indices);

    var staticModel = new TexturedModel(treeModel, loader.LoadModelTexture("Assets/tree.png"));
    var grass = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/grassModel.obj", loader),
        loader.LoadModelTexture("Assets/grassTexture.png"));
    var flower = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/grassModel.obj", loader),
        loader.LoadModelTexture("Assets/flower.png"));
    var fern = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/fern.obj", loader),
        loader.LoadModelTexture("Assets/fern.png"));
    var bobble = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/lowPolyTree.obj", loader),
        loader.LoadModelTexture("Assets/lowPolyTree.png"));
    
    grass.ModelTexture.HasTransparency = true;
    grass.ModelTexture.UseFakeLighting = true;
    grass.ModelTexture.HasTransparency = true;
    grass.ModelTexture.UseFakeLighting = true;
    fern.ModelTexture.HasTransparency = true;

    Random random = new Random(676452);
    for(int i = 0; i < 400; i++){
        if (i % 7 == 0)
        {
            entities.Add(new Entity(grass, new Vector3D<float>(random.NextSingle() * 400 - 200,0,random.NextSingle() * -400),0,0,0,1.8f));
            entities.Add(new Entity(flower, new Vector3D<float>(random.NextSingle() * 400 - 200,0,random.NextSingle() * -400),0,0,0,2.3f));
        }

        if (i % 3 == 0)
        {
            entities.Add(new Entity(fern, new Vector3D<float>(random.NextSingle() * 400 - 200,0,random.NextSingle() * -400),0,random.NextSingle() * 360,0,0.9f));
            entities.Add(new Entity(bobble, new Vector3D<float>(random.NextSingle() * 800 - 400,0,random.NextSingle() * -600),0,random.NextSingle() * 360,0,random.NextSingle() * 0.1f + 0.6f));
            entities.Add(new Entity(staticModel, new Vector3D<float>(random.NextSingle() * 800 - 400,0,random.NextSingle() * -600),0,0,0,random.NextSingle() * 1 + 4));
        }
    }

    // originally in teh script: 0,0 / 1,0 but then the terrain was behind the camera 
    terrain = new Terrain(0, -1, loader, texturePack, blendMap, "Assets/heightmap.png");
    terrain2 = new Terrain(-1, -1, loader, texturePack, blendMap, "Assets/heightmap.png");
    
    renderer = new MasterRenderer(displayManager);

    var bunnyModel = ObjLoader.LoadObjModel("Assets/person.obj", loader);
    var standfordBunny = new TexturedModel(bunnyModel, loader.LoadModelTexture("Assets/playerTexture.png"));

    player = new(standfordBunny, new Vector3D<float>(100, 0, -50), 0, 180, 0, 0.6f);
    camera = new Camera(player);
};

displayManager.Window.Closing += () =>
{
    renderer.Dispose();
    loader.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    if (primaryKeyboard.IsKeyPressed(Key.Escape))
    {
        displayManager.Close();
    }

    camera.Move(primaryKeyboard, primaryMouse);
    player.Move((float)deltaTime, primaryKeyboard);
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.ProcessEntity(player);
    renderer.ProcessTerrain(terrain);
    renderer.ProcessTerrain(terrain2);
    foreach(var entity in entities)
    {
        renderer.ProcessEntity(entity);
    }
    renderer.Render(light, camera);
};

displayManager.Window
    .Run();