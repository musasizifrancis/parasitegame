﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Controllers;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;

namespace Parasite
{
    /// <summary>
    /// This is a base class for all game objects that will be drawn on the screen.
    /// Contains methods that decide where on the screen the node should be drawn based
    /// on the camera position.
    /// </summary>
    public class SceneNode
    {
        /// <summary>
        /// The position of the node in the world.
        /// </summary>
        public Vector2 WorldPosition;

        /// <summary>
        /// The Starting position of the node in the world
        /// </summary>
        public Vector2 StartingPosition;
        public float StartingRotation;

        public float Rotation = 0f;
        public Vector2 Origin;

        public Body PhysicsBody = null;
        public Geom PhysicsGeometry = null;

        /// <summary>
        /// Whether or not this scene node is currently being followed by the Camera
        /// </summary>
        public bool IsCameraTarget;
        
        /// <summary>
        /// Depth on the screen. 0.0 from right in front of the camera,
        /// 1.0 for static background
        /// </summary>
        public float ScreenDepth;

        protected Camera camera;
        protected Game game;
        protected PhysicsManager physicsManager;
        protected DeveloperConsole console;

        private Vector2 screenCentre;

        public SceneNode(Game game, Vector2 startingPosition)
        {
            Initialise(game, startingPosition);
        }

        public SceneNode(Game game)
        {
            Initialise(game, new Vector2(0,0));
        }

        public virtual void Initialise(Game game, Vector2 startingPosition)
        {
            camera = (Camera)game.Services.GetService(typeof(ICamera));
            physicsManager = (PhysicsManager)game.Services.GetService(typeof(IPhysicsManager));
            console = (DeveloperConsole)game.Services.GetService(typeof(IDeveloperConsole));
            
            this.game = game;
            screenCentre.X = game.GraphicsDevice.Viewport.Width / 2;
            screenCentre.Y = game.GraphicsDevice.Viewport.Height / 2;
            ScreenDepth = 0.0f;
            WorldPosition = startingPosition;
        }

        /// <summary>
        /// Calculates where on the screen this scene node should be displayed relative to the camera.
        /// </summary>
        /// <returns>Vector2 representing the screen coords where this scenenode should be drawn.</returns>
        public Vector2 GetScreenPosition()
        {
            return (screenCentre - ((camera.Position - WorldPosition) * camera.ZoomLevel) * (1 - ScreenDepth));
        }

        public void Update()
        {
            //update the position of the scenenode based on the physics body position
            if (PhysicsBody != null)
            {
                WorldPosition = PhysicsBody.Position;
                Rotation = PhysicsBody.Rotation;
            }
        }

    }
}
