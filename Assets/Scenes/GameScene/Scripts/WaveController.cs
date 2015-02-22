﻿//#define TESTMODE

using System;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * HOW TO USE: The prefab with this component attached defines all of the enemies 
 * that will be spawned, and creates pools for them on initialization. 
 */
[RequireComponent( typeof( AudioSource ) )]
public class WaveController : MonoBehaviour
{
    public const int MAX_OBJECT_PER_WAVEDEF = 20;

    [Serializable]
    public class WaveDefinition
    {
        public string Name;                 // Textual description of enemy
        public GameObject GameObjectPrefab; // Prefab associated with enemy 
        public float[] LevelFrequencyLow;   // Lower bound of random % chance of spawning per level
        public float[] LevelFrequencyHigh;  // Upper bound of random % chance of spawning per level
        public ObjectPool Pool;             // not exposed in editor //TODO:Should this be private?
        public bool Disabled;               // Will disable this enemy for debugging purposes
    }

    public WaveDefinition[] Waves;

    public bool Paused { get; set; }

    private AudioSource _audio;

    private float _cur_spawn_timeout;
    private float _next_spawn_time;

    /*****************************/
    public void Awake()
    {
        _audio = GetComponent<AudioSource>();
        CreatePools();
        Reset();
    }

    /*****************************/
    public void Reset()
    {
        _cur_spawn_timeout = GameConstants.MULT_TIMEOUT_RAMP[ 0 ];
        _next_spawn_time = Time.time + _cur_spawn_timeout;
        for ( int i = 0; i < Waves.Length; i++ ) {
            Waves[ i ].Pool.Reset();
        }
    }

    /*****************************/
    void CreatePools()
    {
        for ( int i = 0; i < Waves.Length; i++ ) {
            Waves[ i ].Pool = new ObjectPool( Waves[ i ].GameObjectPrefab, MAX_OBJECT_PER_WAVEDEF );
        }
    }

    /*****************************/
    public ObjectPool GetPoolForName( string pool_name )
    {
        if ( pool_name == null )
            throw new ArgumentNullException( "pool_name" );

        for ( int i = 0; i < Waves.Length; i++ ) {
            if ( Waves[ i ].Name.Equals( pool_name ) ) {
                return Waves[ i ].Pool;
            }
        }
        throw new UnityException( "Could not provide object pool for " + pool_name );
    }

    /*****************************/
    public void Update()
    {
        if ( Paused )
            return;

        _cur_spawn_timeout = GameConstants.MULT_TIMEOUT_RAMP[ GameController.instance.State.Multiplier - 1 ];

        if ( !GameController.instance.DebugMode ) {

            if ( Input.GetKeyDown( KeyCode.Alpha1 ) ) {
                //GameController.instance.State.Multiplier = 1;
                GameController.instance.State.Score = GameState.SCORE_THRESH_1X;
            }
            if ( Input.GetKeyDown( KeyCode.Alpha2 ) ) {
                //GameController.instance.State.Multiplier = 2;
                GameController.instance.State.Score = GameState.SCORE_THRESH_2X;
            }
            if ( Input.GetKeyDown( KeyCode.Alpha3 ) ) {
                //GameController.instance.State.Multiplier = 3;
                GameController.instance.State.Score = GameState.SCORE_THRESH_3X;
            }
            if ( Input.GetKeyDown( KeyCode.Alpha4 ) ) {
                //GameController.instance.State.Multiplier = 4;
                GameController.instance.State.Score = GameState.SCORE_THRESH_4X;
            }
            if ( Input.GetKeyDown( KeyCode.Alpha5 ) ) {
                //GameController.instance.State.Multiplier = 5;
                GameController.instance.State.Score = GameState.SCORE_THRESH_5X;
            }
            if ( Input.GetKeyDown( KeyCode.Alpha6 ) ) {
                //GameController.instance.State.Multiplier = 6;
                GameController.instance.State.Score = GameState.SCORE_THRESH_6X;
            }
        }

        if ( Time.time >= _next_spawn_time ) {
            // Heartbeat thob
            _audio.Play();
            _next_spawn_time = Time.time + _cur_spawn_timeout;
            SpawnTick();
        }
    }

    private void SpawnTick()
    {
        for ( int i = 0; i < Waves.Length; i++ ) {
            if ( Waves[ i ].Disabled )
                continue;
            float chance = Random.Range( Waves[ i ].LevelFrequencyLow[ 0 ], Waves[ i ].LevelFrequencyHigh[ 0 ] );

            if ( Random.Range( 0, 100 ) <= chance ) {
                GameObject g = Waves[ i ].Pool.GetInstance();
                if ( g != null )
                    return;
            }

            // NOTED: There is a slim chance nothing will spawn.
        }
    }
}