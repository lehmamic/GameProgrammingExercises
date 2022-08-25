using FMOD;
using FMOD.Studio;
using GameProgrammingExercises.Maths;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public sealed class AudioSystem : IDisposable
{
    private static uint NextID = 0;
    
    private readonly Game _game;
    
    // Map of loaded banks
    private readonly Dictionary<string, FMOD.Studio.Bank> _banks = new();
    
    // Map of event name to EventDescription
    private readonly Dictionary<string, FMOD.Studio.EventDescription> _events = new();
    
    // Map of event id to EventInstance
    private readonly Dictionary<uint, FMOD.Studio.EventInstance> _eventInstances = new();
    
    // Map of buses
    private readonly Dictionary<string, FMOD.Studio.Bus> _buses = new();

    // FMOD studio system
    private FMOD.Studio.System _system;

    // FMOD Low-level system (in case needed)
    private FMOD.System _lowLevelSystem;

    public AudioSystem(Game game)
    {
        _game = game;
    }

    public void Initialize()
    {
        // Set the native library location
        // Fmod.SetLibraryLocation("External/FMOD/FMOD Programmers API");

        // Initialize debug logging
        Debug.Initialize(
            DEBUG_FLAGS.ERROR, // Log only errors
            DEBUG_MODE.TTY); // Output to stdout

        // Create FMOD studio system object
        var result = FMOD.Studio.System.create(out _system);
        if (result != RESULT.OK)
        {
            throw new AudioSystemException($"Failed to create FMOD system: {result}");
        }

        // Initialize FMOD studio system
        _system.initialize(
            512,                         // Max number of concurrent sounds
            FMOD.Studio.INITFLAGS.NORMAL, // Use default settings
            FMOD.INITFLAGS.NORMAL,             // Use default settings
            default);          // Usually null

        // Save the low-level system pointer
        // _system.GetLowLevelSystem(&mLowLevelSystem);

        // Load the master banks (strings first)
        LoadBank("Assets/Master Bank.strings.bank");
        LoadBank("Assets/Master Bank.bank");
    }

    public void Update(float deltaTime)
    {
        // Find any stopped event instances
        var done = new List<uint>();
        foreach (var e in _eventInstances)
        {
            // Get the state of this PlayEvent
            e.Value.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.STOPPED)
            {
                // Release the event and add id to done
                e.Value.release();
                done.Add(e.Key);
            }
        }
    
        // Remove done event instances from map
        foreach (var id in done)
        {
            _eventInstances.Remove(id);
        }

        // Update FMOD
        _system.update();
    }

    public void LoadBank(string name)
    {
        // Prevent double-loading
        if (_banks.ContainsKey(name))
        {
            return;
        }

        // Try to load bank
        var result = _system.loadBankFile(
            name,                       // File name of bank
            LOAD_BANK_FLAGS.NORMAL,     // Normal loading
            out var bank);              // Save pointer to bank

        if (result == RESULT.OK)
        {
            // Add bank to map
            _banks.Add(name, bank);

            // Load all non-streaming sample data
            bank.loadSampleData();
            
            // Get the number of events in the this bank
            bank.getEventCount(out int numEvents);
            if (numEvents > 0)
            {
                // Get list of event descriptions in this bank
                bank.getEventList(out var events);
                
                for (int i = 0; i< numEvents; i++)
                {
                    var e = events[i];
                    
                    // Get the path of this event (like event:/Explosion2D)
                    e.getPath(out var eventName);
                    
                    // Add to event map
                    _events.Add(eventName, e);
                }
            }
            
            // Get the number of buses in this bank
            bank.getBusCount(out var numBuses);
            if (numBuses > 0)
            {
                // Get list of buses in this bank
                bank.getBusList(out var buses);
  
                for (int i = 0; i < numBuses; i++)
                {
                   var bus = buses[i];

                    // Get the path of this bus (like bus:/SFX)
                    bus.getPath(out var busName);
                    
                    // Add to buses map
                    _buses.Add(busName, bus);
                }
            }
        }
    }

    public void UnloadBank(string name)
    {
        // Ignore if not loaded
        if (!_banks.ContainsKey(name))
        {
            return;
        }

        // First we need to remove all events from this bank
        var bank = _banks[name];
        bank.getEventCount(out var numEvents);
        if (numEvents > 0)
        {
            // Get event descriptions for this bank
            bank.getEventList(out var events);

            // Get list of events
            for (int i = 0; i < numEvents; i++)
            {
                var e = events[i];
                
                // Get the path of this event
                e.getPath(out var eventName);
                
                // Remove this event
                if (_events.ContainsKey(eventName))
                {
                    _events.Remove(eventName);
                }
            }
        }

        // Get the number of buses in this bank
        bank.getBusCount(out var numBuses);
        if (numBuses > 0)
        {
            // Get list of buses in this bank
            bank.getBusList(out var buses);

            for (int i = 0; i < numBuses; i++)
            {
                var bus = buses[i];

                // Get the path of this bus (like bus:/SFX)
                bus.getPath(out var busName);

                // Remove this bus
                if (_buses.ContainsKey(busName))
                {
                    _banks.Remove(busName);
                }
            }
        }
        
        // Unload sample data and bank
        bank.unloadSampleData();
        bank.unload();

        // Remove from banks map
        _banks.Remove(name);
    }

    public void UnloadAllBanks()
    {
        foreach (var bank in _banks.Values)
        {
            bank.unloadSampleData();
            bank.unload();
        }

        _banks.Clear();

        // No banks means no events
        _events.Clear();

        _buses.Clear();
    }

    public SoundEvent PlayEvent(string name)
    {
        uint retID = 0;
        
        // Make sure event exists
        if (_events.TryGetValue(name, out var eventDescription))
        {
            // Create instance of event
            eventDescription.createInstance(out var eventInstance);
            if (eventInstance.isValid())
            {
                // Start the event instance
                eventInstance.start();
                
                // Get the next id, and ad to map
                NextID++;
                retID = NextID;
                _eventInstances.Add(retID, eventInstance);
            }
        }

        return new SoundEvent(this, retID);
    }
    
    public void SetListener(Matrix4X4<float> viewMatrix)
    {
        // Invert the view matrix to get the correct vectors
        Matrix4X4.Invert(viewMatrix, out var invView);

        var listener = new ATTRIBUTES_3D
        {
            // Set position, forward, up
            position = invView.GetTranslation().VecToFMOD(),
            forward = invView.GetZAxis().VecToFMOD(),
            up = invView.GetYAxis().VecToFMOD(),

            // Set velocity to zero (fix if using Doppler effect)
            velocity = new VECTOR { x = 0.0f, y = 0.0f, z = 0.0f },
        };

        // Send to FMOD (0 = only one listener)
        _system.setListenerAttributes(0, listener);
    }

    public float GetBusVolume(string name)
    {
        float retVal = 0.0f;
        if (_buses.TryGetValue(name, out var bus))
        {
            bus.getVolume(out retVal);
        }
    
        return retVal;
    }

    public bool GetBusPaused(string name)
    {
        bool retVal = false;
        if (_buses.TryGetValue(name, out var bus))
        {
            bus.getPaused(out retVal);
        }
    
        return retVal;
    }

    public void SetBusVolume(string name, float volume)
    {
        if (_buses.TryGetValue(name, out var bus))
        {
            bus.setVolume(volume);
        }
    }

    public void SetBusPaused(string name, bool pause)
    {
        if (_buses.TryGetValue(name, out var bus))
        {
            bus.setPaused(pause);
        }
    }

    public EventInstance? GetEventInstance(uint id)
    {
        _eventInstances.TryGetValue(id, out var eventInstance);
        return eventInstance;
    }

    public void Dispose()
    {
        UnloadAllBanks();
        _system.release();
    }
}