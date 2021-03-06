using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace IKParasite_xna
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Parasite : Microsoft.Xna.Framework.DrawableGameComponent
    {

        // Parasite Part Variables
        private ParasiteHead head;

        private List<ParasiteBodyPart> bodyparts;

        private ParasiteTail tail;

        private RenderTarget2D mBackgroundRender;
        private RenderTarget2D mBackgroundRenderRotated;

        // Whole Parasite
        private List<ParasiteBodyPart> theParasite;

        // Movement vars
        private bool leftMovement = false;
        private bool rightMovement = false;

        private Vector2 gravity = new Vector2(0f, 0.9f);

        private bool rigid = false;
        private bool tailMoving = false;
        private float launchForce;

        private Texture2D theSprite;

        // System Stuff
        private SpriteBatch spriteBatch;

        public Parasite(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            /**
			 * Parasite made up of 4 parts.
			 * -Head
			 * -BPart01
			 * -BPart02
			 * -BPart03
			 * -Tail
			 */
            init();
            CreateParasite(6);
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Load any content

            base.LoadContent();
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\ParasiteBodyPart");

            mBackgroundRender = new RenderTarget2D(this.GraphicsDevice, 100, 100, 1, SurfaceFormat.Color);
            mBackgroundRenderRotated = new RenderTarget2D(this.GraphicsDevice, 100, 100, 1, SurfaceFormat.Color);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(theSprite, head.position, null, Color.White, 0, head.centre, 1, SpriteEffects.None, 1);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                //spriteBatch.Draw(theSprite, bodyparts[i].position, null, Color.White, 0f, bodyparts[i].centre, 1, SpriteEffects.None, 1);
                float scale = (bodyparts.Count - i+1);
                scale /= bodyparts.Count;
                spriteBatch.Draw(theSprite, bodyparts[i].position, null, Color.White, bodyparts[i].rotation, bodyparts[i].centre, new Vector2(scale, scale), SpriteEffects.None, 1);
            }

            //spriteBatch.Draw(theSprite, tail.position, null, Color.Chocolate, 0, tail.centre, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(theSprite, tail.position, null, Color.Chocolate, tail.rotation, tail.centre, new Vector2(0.8f, 0.8f), SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //head.position = mousePos;

            KeyboardState aKeyboard = Keyboard.GetState();
            MouseState aMouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(aMouse.X,aMouse.Y);

            Boolean applyGravity = true;

            if (!rigid)
            {
                if (aKeyboard.IsKeyDown(Keys.Left))
                {
                    head.position.X -= 2;
                }
                else if (aKeyboard.IsKeyDown(Keys.Right))
                {
                    head.position.X += 2;
                }

            }

            if ((tail.position - mousePos).Length() < 10 * bodyparts.Count)
            {
                tail.position = mousePos;
                //head.position = mousePos;

                applyGravity = false;
            }

            if (aMouse.LeftButton == ButtonState.Pressed && (tail.position - mousePos).Length() < 15)
            {
                // LOCK all angles
                if (rigid)
                {
                    float maxDistance = theParasite.Count * 10;
                    float theDistance = (head.position - tail.position).Length();

                    // max distance between head/tail = theParasite.Count * defaultDistance.
                    // in this case, it is : 8 * 10 = 80.
                    head.IKPoint.distance = (theDistance / theParasite.Count);
                    tail.IKPoint.distance = (theDistance / theParasite.Count);
                    for (int i = 0; i < bodyparts.Count; i++)
                    {
                        bodyparts[i].IKPoint.distance = (theDistance / theParasite.Count);
                    }

                    launchForce = maxDistance - theDistance;
                }
                else
                {
                    lockAngles();
                    rigid = true;
                }
            }
            else
            {
                if (rigid)
                {
                    unlockAngles();

                    head.IKPoint.distance = head.IKPoint.defaultdistance;
                    tail.IKPoint.distance = tail.IKPoint.defaultdistance;
                    for (int i = 0; i < bodyparts.Count; i++)
                    {
                        bodyparts[i].IKPoint.distance = bodyparts[i].IKPoint.defaultdistance;
                    }

                    rigid = false;

                    if (launchForce > 0)
                    {
                        // Shoot him off!
                        head.velocity.X = (float)Math.Cos(tail.IKPoint.currentAngle) * launchForce;
                        head.velocity.Y = (float)Math.Sin(tail.IKPoint.currentAngle) * launchForce;
                    }
                    
                }
            }

            tail.Init();
            tail.UpdatePoint();

            if (applyGravity)
                tail.ApplyForce(gravity);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                bodyparts[i].Init();
                bodyparts[i].UpdatePoint();

                if(applyGravity)
                    bodyparts[i].ApplyForce(gravity);
            }

            head.Init();
            head.UpdatePoint();
            head.ApplyForce(gravity);

            //head.IKPoint.moveTo(mousePos.X, mousePos.Y);

            //head.velocity.Y += 0.2f;

            base.Update(gameTime);
        }

        public void lockAngles()
        {
            tail.IKPoint.lockAngle(true);
            for (int i = 0; i < bodyparts.Count; i++)
            {
               bodyparts[i].IKPoint.lockAngle(true);
            }
        }

        public void unlockAngles()
        {
            tail.IKPoint.lockAngle(false);
            for (int i = 0; i < bodyparts.Count; i++)
            {
                bodyparts[i].IKPoint.lockAngle(false);
            }
        }

        public void init()
        {
            bodyparts = new List<ParasiteBodyPart>();
            theParasite = new List<ParasiteBodyPart>();
        }

        public void CreateParasite(int numParts)
        {
            // Create the Tail
            tail = new ParasiteTail(theSprite, 1.0f);
            tail.position = new Vector2(150 + (50 * numParts), 100);

            // Create Body Parts
            for (int i = 0; i < numParts; i++)
            {
                ParasiteBodyPart bodyPart = new ParasiteBodyPart(theSprite, 1f);
                bodyPart.position = new Vector2(150 + (50 * i), 100);
                bodyparts.Add(bodyPart);
            }
            
            // Create the Head
            head = new ParasiteHead(theSprite, 1);
            head.position = new Vector2(100, 100);

            // IKPoints

            theParasite.Add(head);

            IKMember headIK = new IKMember(head, 10);
            IKMember lastIK = headIK;

            head.AddIKPoint(headIK);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                ParasiteBodyPart bodyPart = bodyparts[i];

                theParasite.Add(bodyPart);

                IKMember ik = new IKMember(bodyPart, 10);
                  
                if (i != 0)
                {
                    ik.addNeighbour(lastIK);
                }

                lastIK.addNeighbour(ik);

                bodyPart.AddIKPoint(ik);

                lastIK = ik;
            }

            // Uncomment for Rad Wiggle Motion!
            // currentIK = new IKMember(tail, 1);

            //currentIK = new IKMember(tail, 10);
            theParasite.Add(tail);
            
            IKMember tailIK = new IKMember(tail, 10);

            tailIK.addNeighbour(lastIK);
            lastIK.addNeighbour(tailIK);

            tail.AddIKPoint(tailIK);

            tail.initTail();
        }
    }
}