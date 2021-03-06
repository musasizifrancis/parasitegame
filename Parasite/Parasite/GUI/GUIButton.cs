using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Parasite
{

    class OnMouseClickEventArgs : EventArgs
    {
        public OnMouseClickEventArgs(string command, string argument)
            : base()
        {
            Command = command;
            Argument = argument;
        }
        public string Command;
        public string Argument;
    };

    /// <summary>
    /// Simple button that has a MouseClick event.
    /// It will grow to fill the size of the caption.
    /// </summary>
    class GUIButton : GUIComponent
    {
        //Event handler for when we're clicked
        public delegate void MouseClickHandler(GUIComponent sender, OnMouseClickEventArgs args);
        public event MouseClickHandler OnMouseClick;
        public event MouseClickHandler OnMouseHold;

        //drws the button geometry
        PrimitiveBatch batch;
        
        //draws the button text
        SpriteBatch textBatch;
        SpriteFont font;

        // width and height of the button
        Vector2 Dimensions = Vector2.Zero;

        public bool AllowHold = false;

        //width and height of the font
        Vector2 fontDimensions = Vector2.Zero;
        //screen position of the caption on the button
        Vector2 textPosition;

        private string caption;
        public string Caption
        {
            set
            {
                caption = value;
                fontDimensions = Vector2.Zero;
                if (font == null) return;
                //measure the caption so we know how big to make the button               

                //set the bounds of the button to incorporate the caption + any padding
                //Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + (textPaddingSide * 2)), (int)(fontDimensions.Y + (textPaddingTopAndBottom * 2)));

                if (Dimensions == Vector2.Zero)
                {
                    fontDimensions = font.MeasureString(caption);
                    Dimensions = new Vector2((fontDimensions.X + (textPaddingSide * 2)), (fontDimensions.Y + (textPaddingTopAndBottom * 2)));
                }

                Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);

                //set the position of the caption.
                textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            }
            get
            {
                return caption;
            }
        }

                
        public GUIButton(Game game, Vector2 location, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
            Dimensions = Vector2.Zero;
        }

        public GUIButton(Game game, Vector2 location, Vector2 dimensions, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
            Dimensions = dimensions;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization code here
            batch = new PrimitiveBatch(Game.GraphicsDevice);
            textBatch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();

            //if we're clicked on
            if (input.IsMouseButtonClicked("left") && Bounds.Contains(mState.X, mState.Y))
            {
                if (OnMouseClick != null)
                {
                    //fire off any mouseclick event handlers that are listening
                    OnMouseClick(this, null);
                }
            }

            if (input.IsMouseButtonPressed("left") && Bounds.Contains(mState.X, mState.Y) && AllowHold)
            {
                if (OnMouseHold != null)
                {
                    //fire off any mouseclick event handlers that are listening
                    OnMouseHold(this, null);
                }
            }
            
            //if the font dimensions arent set yet then we can;t draw the button
            //so set the caption again (see caption "set" property).
            if (fontDimensions == Vector2.Zero)
                Caption = Caption;

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposed)
            {
                if (disposing)
                {
                    if (batch != null)
                    {
                        batch.Dispose();
                        batch = null;
                    }

                    if (textBatch != null)
                    {
                        textBatch.Dispose();
                        textBatch = null;
                    }
                }
            }
        }

        public override void UpdateLocation(Vector2 newLocation)
        {
            Location = newLocation;
            //Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + (textPaddingSide * 2)), (int)(fontDimensions.Y + (textPaddingTopAndBottom * 2)));

            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
            textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
        }

        public override void Draw(GameTime gameTime)
        {
            //draw top left triangle of the button
            batch.Begin(PrimitiveType.TriangleList);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);

            //draw bottom right tri of the button
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);
            batch.End();
            
            //rad dropshadow
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), Color.Black);
            batch.End();

            //draw caption
            textBatch.Begin();
            textBatch.DrawString(font, caption, textPosition, ForegroundColor);
            textBatch.End();
           // batch.AddVertex(
            base.Draw(gameTime);
        }
    }
}
