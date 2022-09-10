﻿// See https://aka.ms/new-console-template for more information

using GameEngine.Entities;
using GameEngine.Guis;
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
List<Light> lights = new();
List<GuiTexture> guis = new();

Terrain terrain = null!;

Camera camera = null!;
MasterRenderer renderer = null!;
GuiRenderer guiRenderer = null!;

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
    var pine = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/pine.obj", loader),
        loader.LoadModelTexture("Assets/pine.png"));

    var fernTextureAtlas = loader.LoadModelTexture("Assets/fern.png");
    fernTextureAtlas.NumberOfRows = 2;
    var fern = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/fern.obj", loader),
        fernTextureAtlas);

    var lowPolyTreeTextureAtlas = loader.LoadModelTexture("Assets/lowPolyTree.png");
    lowPolyTreeTextureAtlas.NumberOfRows = 2;
    var bobble = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/lowPolyTree.obj", loader),
        lowPolyTreeTextureAtlas);
    var box = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/box.obj", loader),
        loader.LoadModelTexture("Assets/box.png"));
    var box2 = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/box.obj", loader),
        loader.LoadModelTexture("Assets/box.png"));
    var lamp = new TexturedModel(
        ObjLoader.LoadObjModel("Assets/lamp.obj", loader),
        loader.LoadModelTexture("Assets/lamp.png"));
    
    grass.Texture.HasTransparency = true;
    grass.Texture.UseFakeLighting = true;
    grass.Texture.HasTransparency = true;
    grass.Texture.UseFakeLighting = true;
    fern.Texture.HasTransparency = true;
    lamp.Texture.UseFakeLighting = true;

    terrain = new Terrain(0, -1, loader, texturePack, blendMap, "Assets/heightmap.png");

    Random random = new Random(676452);
    for(int i = 0; i < 400; i++)
    {
        if (i % 2 == 0)
        {
            var x = random.NextSingle() * 800 - 400;
            var z = random.NextSingle() * -600;
            var y = terrain.GetHeightOfTerrain(x, z);
            entities.Add(new Entity(fern, random.Next(0, 3),new Vector3D<float>(x,y,z),0,random.NextSingle() * 360,0,0.9f));
        }
        if (i % 5 == 0)
        {
            // entities.Add(new Entity(grass, new Vector3D<float>(random.NextSingle() * 400 - 200,0,random.NextSingle() * -400),0,0,0,1.8f));
            // entities.Add(new Entity(flower, new Vector3D<float>(random.NextSingle() * 400 - 200,0,random.NextSingle() * -400),0,0,0,2.3f));
            var x = random.NextSingle() * 800 - 400;
            var z = random.NextSingle() * -600;
            var y = terrain.GetHeightOfTerrain(x, z);
            entities.Add(new Entity(pine, random.Next(0, 3), new Vector3D<float>(x,y,z),0,random.NextSingle() * 360,0,random.NextSingle() * 0.1f + 0.6f));

            x = random.NextSingle() * 800 - 400;
            z = random.NextSingle() * -600;
            y = terrain.GetHeightOfTerrain(x, z);
            entities.Add(new Entity(pine, new Vector3D<float>(x,y,z),0,0,0,random.NextSingle() * 1 + 4));
        }
    }

    lights.Add(new Light(new Vector3D<float>(0.0f, 10000.0f, -7000.0f), new Vector3D<float>(0.4f, 0.4f, 0.4f)));

    var lightX = 185.0f;
    var lightZ = -293.0f;
    var lightY = terrain.GetHeightOfTerrain(lightX, lightZ);
    lights.Add(new Light(new Vector3D<float>(lightX, lightY + 12.8f, lightZ), new Vector3D<float>(2.0f, 0.0f, 0.0f), new Vector3D<float>(1.0f, 0.01f, 0.002f)));
    entities.Add(new Entity(lamp, new Vector3D<float>(lightX, lightY, lightZ), 0, 0, 0,1));

    lightX = 370.0f;
    lightZ = -300.0f;
    lightY = terrain.GetHeightOfTerrain(lightX, lightZ);
    lights.Add(new Light(new Vector3D<float>(lightX, lightY + 12.8f, lightZ), new Vector3D<float>(0.0f, 2.0f, 2.0f), new Vector3D<float>(1.0f, 0.01f, 0.002f)));
    entities.Add(new Entity(lamp, new Vector3D<float>(lightX, lightY, lightZ), 0, 0, 0,1));
    
    lightX = 293.0f;
    lightZ = -305.0f;
    lightY = terrain.GetHeightOfTerrain(lightX, lightZ);
    lights.Add(new Light(new Vector3D<float>(lightX, lightY + 12.8f, lightZ), new Vector3D<float>(2.0f, 2.0f, 0.0f), new Vector3D<float>(1.0f, 0.01f, 0.002f)));
    entities.Add(new Entity(lamp, new Vector3D<float>(lightX, lightY, lightZ), 0, 0, 0,1));

    renderer = new MasterRenderer(displayManager, loader);
    guiRenderer = new GuiRenderer(displayManager, renderer, loader, Matrix4X4<float>.Identity);

    var bunnyModel = ObjLoader.LoadObjModel("Assets/person.obj", loader);
    var standfordBunny = new TexturedModel(bunnyModel, loader.LoadModelTexture("Assets/playerTexture.png"));

    player = new(standfordBunny, new Vector3D<float>(100, 0, -50), 0, 180, 0, 0.6f);
    camera = new Camera(player);

    var gui = loader.LoadGuiTexture("Assets/health.png", new Vector2D<float>(0.4f, -0.6f), new Vector2D<float>(0.25f, 0.25f));
    guis.Add(gui);
};

displayManager.Window.Closing += () =>
{
    renderer.Dispose();
    guiRenderer.Dispose();
    loader.Dispose();
};

displayManager.Window.Update += (deltaTime) =>
{
    if (primaryKeyboard.IsKeyPressed(Key.Escape))
    {
        displayManager.Close();
    }

    camera.Move(primaryKeyboard, primaryMouse);
    player.Move((float)deltaTime, terrain, primaryKeyboard);
};

displayManager.Window.Render += (deltaTime) =>
{
    // Game logic
    renderer.ProcessEntity(player);
    renderer.ProcessTerrain(terrain);
    // renderer.ProcessTerrain(terrain2);
    foreach(var entity in entities)
    {
        renderer.ProcessEntity(entity);
    }
    renderer.Render((float)deltaTime, lights, camera);
    guiRenderer.Render(guis);
};

displayManager.Window
    .Run();