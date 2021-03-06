using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Blob_P2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public enum GameState { gsEdit, gsSimulate };
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public BlobManager theBlob;                    // Blob Manager - Handles Particles
        public StaticBodyManager staticBodyManager;    // Static Manager - Handles Static Bodies
        public RigidBodyManager rigidBodyManager;      // Rigid Manager - Handles Rigid Bodies      **Better way to do this ? We could end up with a million managers...?
        public StaticBodyEditor staticBodyEditor;
        FrameRateCounter frc;
        public GameState state;

        public Parasite theParasite;

        SceneCameraComponent camera;
        SceneManager manager;

        InputHandler input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            state = GameState.gsEdit;

            theBlob = new BlobManager(this);
            this.Components.Add(theBlob);

            frc = new FrameRateCounter(this);
            Components.Add(frc);

            staticBodyManager = new StaticBodyManager(this);
            this.Components.Add(staticBodyManager);

            rigidBodyManager = new RigidBodyManager(this);
            this.Components.Add(rigidBodyManager);

            staticBodyEditor = new StaticBodyEditor(this);
            this.Components.Add(staticBodyEditor);

            camera = new SceneCameraComponent(this);
            this.Components.Add(camera);

            input = new InputHandler(this);
            this.Components.Add(input);

            GuiManager manager = new GuiManager(this);
            this.Components.Add(manager);
            manager.AddButton(new Vector2(250, 250), "Button1", "THIS IS A LARGE CAPTION");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            manager = new SceneManager(this);

            base.Initialize();      
        }

        private void createParasite()
        {
            theParasite = new Parasite(this);
            this.Components.Add(theParasite);
            //theParasite.Initialize();
            camera.SetTarget(theParasite.head);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        ///
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                staticBodyManager.Save();

            if (Keyboard.GetState().IsKeyDown(Keys.L))
                staticBodyManager.Load();

            if (Keyboard.GetState().IsKeyDown(Keys.P) && theParasite == null)
                createParasite();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);
            

            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("DebugFont"), "MOUSE: " + Mouse.GetState().ToString(), new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(Content.Load<SpriteFont>("DebugFont"), "MODE: " + ((state == GameState.gsEdit) ? "Edit" : "Simulate"), new Vector2(10, 30), (state == GameState.gsEdit) ? Color.Red : Color.Green);
            spriteBatch.DrawString(Content.Load<SpriteFont>("DebugFont"), "MOUSE WORLD: " + camera.MouseToWorld(), new Vector2(10, 50), Color.Red);
            spriteBatch.DrawString(Content.Load<SpriteFont>("DebugFont"), "CAMERA WORLD: " + camera.Position, new Vector2(10, 90), Color.Red);

            spriteBatch.End();
            // TODO: Add your drawing code here
        }

        public static RenderTarget2D CloneRenderTarget(GraphicsDevice device, int numberLevels)
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                numberLevels,
                device.DisplayMode.Format,
                device.PresentationParameters.MultiSampleType,
                device.PresentationParameters.MultiSampleQuality
            );
        }
    }
}
