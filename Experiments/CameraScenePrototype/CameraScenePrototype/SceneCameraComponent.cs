﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CameraScenePrototype
{
    public interface ISceneCameraComponent { }
    class SceneCameraComponent : GameComponent, ISceneCameraComponent
    {
        public Rectangle ViewableArea;
        public Vector2 Position;
        private Vector2 finalPosition; //used for smooth scrolling.
        public SceneNode Target;
        float accel = 0.0f;
        float mu = 0.0f;
        private InputHandler input;
        private bool userControlled;

        public void SetFinalPosition(Vector2 pos)
        {
            mu = 0.0f;
            accel = 0.15f;
            finalPosition = pos;
        }

        public SceneCameraComponent(Game game) : base(game)
        {
            //register self as a game service.
            Game.Services.AddService(typeof(ISceneCameraComponent), this);
            userControlled = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandlerComponent));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (userControlled)
                SetFinalPosition(finalPosition - input.MouseDisplacement());

            if (Target != null && mu > 0.15f)
                SetFinalPosition(Target.Position);

           // Position = Target.Position;

            if (Position != finalPosition)
            {
                Position = Vector2.SmoothStep(Position, finalPosition, mu);
                mu += accel;
            }

        }

        public void SetTarget(SceneNode node)
        {
            userControlled = false;
            
            //we had a previous target
            Target = node;

            if (Position != Target.Position)
            {
                SetFinalPosition(Target.Position);
            }
        }
    }
}
