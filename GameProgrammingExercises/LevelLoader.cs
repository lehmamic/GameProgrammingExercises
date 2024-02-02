using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameProgrammingExercises;

public static class LevelLoader
{
    private static Dictionary<string, Func<Game, RawActor, Actor>> ActorFactoryMap =
        new()
        {
            { nameof(Actor), Actor.Create<Actor> },
            { nameof(BallActor), Actor.Create<Actor> },
            { nameof(FollowActor), Actor.Create<Actor> },
            { nameof(PlaneActor), Actor.Create<Actor> },
            { nameof(TargetActor), Actor.Create<Actor> },
        };

    private static Dictionary<string, Func<Actor, RawComponent, Component>> ComponentFactoyMap = new()
    {
        { nameof(AudioComponent), Component.Create<AudioComponent> },
        { nameof(BallMove), Component.Create<BallMove> },
        { nameof(BoxComponent), Component.Create<BoxComponent> },
        { nameof(CameraComponent), Component.Create<CameraComponent> },
        { nameof(FollowCamera), Component.Create<FollowCamera> },
        { nameof(MeshComponent), Component.Create<MeshComponent> },
        { nameof(MoveComponent), Component.Create<MoveComponent> },
        { nameof(SkeletalMeshComponent), Component.Create<SkeletalMeshComponent> },
        { nameof(SpriteComponent), Component.Create<SpriteComponent> },
        { nameof(MirrorCamera), Component.Create<MirrorCamera> },
        { nameof(PointLightComponent), Component.Create<PointLightComponent> },
        { nameof(TargetComponent), Component.Create<TargetComponent> },
    };

    public static void LoadLevel(Game game, string fileName)
    {
        var jsonString = File.ReadAllText(fileName);
        var raw = JsonSerializer.Deserialize<Level>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (raw?.Version != 1)
        {
            throw new LevelLoaderException($"Incorrect level  file version for {fileName}.");
        }

        LoadGlobalProperties(game, raw.GlobalProperties);
        LoadActors(game, raw.Actors);
    }

    private static void LoadActors(Game game, RawActor[] actors)
    {
        // Loop through array of actors
        for (int i = 0; i < actors.Length; i++)
        {
            var rawActor = actors[i];
            if (ActorFactoryMap.TryGetValue(rawActor.Type, out var actorFactory))
            {
                // Construct with function stored in map
                var actor = actorFactory(game, rawActor);
                LoadComponents(actor, rawActor.Components);
            }
        }
    }

    private static void LoadComponents(Actor actor, RawComponent[] components)
    {
        // Loop through array of components
        for (int i = 0; i < components.Length; i++)
        {
            var rawComponent = components[i];

            // Does the actor already have a component of this type?
            var component = actor.GetComponentOfType(rawComponent.Type);
            if (component is null)
            {
                // It's a new component, call function from map
                if (ComponentFactoyMap.TryGetValue(rawComponent.Type, out var componentFactory))
                {
                    // Construct with function stored in map
                    component = componentFactory(actor, rawComponent);
                }
            }
            else
            {
                // It already exists, just load properties
                component.LoadProperties(rawComponent.Properties);
            }
        }
    }

    private static void LoadGlobalProperties(Game game, GlobalProperties globals)
    {
        // Get ambient light
        if (JsonHelper.TryGetVector3D(globals.AmbientLight, out var ambient))
        {
            game.Renderer.AmbientLight = ambient;
        }

        // Set direction/color, if they exist
        JsonHelper.TryGetVector3D(globals.DirectionalLight.Direction, out var direction);
        JsonHelper.TryGetVector3D(globals.DirectionalLight.DiffuseColor, out var diffuseColor);
        JsonHelper.TryGetVector3D(globals.DirectionalLight.SpecularColor, out var specularColor);

        game.Renderer.DirectionalLight = new DirectionalLight(
                direction,
                diffuseColor,
                specularColor);
    }

    private class Level
    {
        public int Version { get; set; }

        public GlobalProperties GlobalProperties { get; set; } = new();

        public RawActor[] Actors { get; set; } = Array.Empty<RawActor>();
    }

    private class GlobalProperties
    {
        public float[] AmbientLight { get; set; } = Array.Empty<float>();

        public RawDirectionalLight DirectionalLight { get; set; } = new();
    }

    private class RawDirectionalLight
    {
        public float[] Direction { get; set; } = Array.Empty<float>();

        public float[] DiffuseColor { get; set; } = Array.Empty<float>();

        public float[] SpecularColor { get; set; } = Array.Empty<float>();
    }

    public class RawActor
    {
        public string Type { get; set; }

        public ActorProperties Properties { get; set; } = new();

        public RawComponent[] Components { get; set; } = Array.Empty<RawComponent>();
    }

    public class ActorProperties
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ActorState State { get; set; }

        public float[]? Position { get; set; }

        public float[]? Rotation { get; set; }

        public float? Scale { get; set; }            

        [JsonPropertyName("lifespan")]
        public float LifeSpan { get; set; }

        public bool Moving { get; set; }
    }

    public class RawComponent
    {
        public string Type { get; set; }

        public ComponentProperties Properties { get; set; } = new();
    }

    public class ComponentProperties
    {
        public int? UpdateOrder { get; set; }

        public string? MeshFile { get; set; }

        public int? TextureIndex { get; set; }

        public bool? Visible { get; set; }

        public bool? IsSkeletal { get; set; }

        public float[]? ObjectMin { get; set; }

        public float[]? WorldMax { get; set; }

        public float[]? Color { get; set; }

        public float? InnerRadius { get; set; }

        public float? OuterRadius { get; set; }
    }
}