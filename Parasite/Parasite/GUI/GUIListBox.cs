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
    /// <summary>
    /// Simple text label.
    /// </summary>
    class GUIListBox : GUIComponent
    {
        SpriteBatch batch;
        SpriteFont font;
        PrimitiveBatch primBatch;

        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;

        GUIButton DownButton;
        GUIButton UpButton;
        GUIButton PosBar;

        string selectedItem = "";

        List<GUIListBoxItem> Items;
        List<GUIComponent> components;

        int topItem = 0;
        private static int numItems = 10;
        private float lastMousePos;

        // Sizes
        float barIntervals;
                
        public GUIListBox(Game game, Vector2 location, string name, List<GUIListBoxItem> items)
            : base(game)
        {
            Location = location;
            Name = name;
            Items = items;
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
                    if (primBatch != null)
                    {
                        primBatch.Dispose();
                        primBatch = null;
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization code here
            batch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");
            primBatch = new PrimitiveBatch(Game.GraphicsDevice);

            // Set up visible items
            if (numItems > Items.Count)
            {
                numItems = Items.Count;
            }

            // Initialise the buttons
            foreach (GUIListBoxItem lbi in Items)
            {
                lbi.Initialize();
                lbi.OnMouseClick += new GUIListBoxItem.MouseClickHandler(SelectItem);
            }

            // And set the initial locations
            UpdateList();

            components = new List<GUIComponent>();
      
            float boxHeight = (numItems - 1) * Items[0].Bounds.Height;

            // Display Down button
            DownButton = new GUIButton(Game, new Vector2(100, boxHeight), new Vector2(20, 20), "down", "+");
            DownButton.Initialize();
            DownButton.OnMouseClick += new GUIButton.MouseClickHandler(ScrollDown);
            DownButton.UpdateLocation(DownButton.Location + new Vector2(Location.X, Location.Y));
            components.Add(DownButton);

            // Display Up button
            UpButton = new GUIButton(Game, new Vector2(100, 0), new Vector2(20, 20), "up", "-");
            UpButton.Initialize();
            UpButton.OnMouseClick += new GUIButton.MouseClickHandler(ScrollUp);
            UpButton.UpdateLocation(UpButton.Location + new Vector2(Location.X, Location.Y));
            components.Add(UpButton);

            // Calculate bar intervals
            barIntervals = ((boxHeight-20) / Items.Count);

            PosBar = new GUIButton(Game, new Vector2(100, 20), new Vector2(20, barIntervals * numItems), "bar", "");
            PosBar.BackgroundColor = HighlightColor;
            PosBar.AllowHold = true;
            PosBar.Initialize();
            PosBar.OnMouseHold += new GUIButton.MouseClickHandler(DragScroll);
            PosBar.UpdateLocation(PosBar.Location + new Vector2(Location.X, Location.Y));
            components.Add(PosBar);
        }

        /// <summary>
        /// Ensures that only one value is selected at a time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void SelectItem(GUIComponent sender, OnMouseClickEventArgs args)
        {
            GUIListBoxItem listBox = (GUIListBoxItem)sender;

            if (selectedItem == "")
            {
                // If nothing is currently selected, select.
                listBox.SelectItem(true);
                selectedItem = listBox.Name;
            }
            else if (selectedItem == listBox.Name)
            {
                // If this item is already selected, deselect it.
                listBox.SelectItem(false);
                selectedItem = "";
            }
            else
            {
                // If there's something else selected, remove it, then add the new one
                foreach (GUIListBoxItem lbi in Items)
                {
                    if (lbi.Name == selectedItem)
                    {
                        lbi.SelectItem(false);
                    }
                }

                listBox.SelectItem(true);
                selectedItem = listBox.Name;
            }
        }

        /// <summary>
        /// Rad Dragging of the Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void DragScroll(GUIComponent sender, OnMouseClickEventArgs args)
        {
            float mousePos = input.MousePosition.Y;
            if (lastMousePos != null)
            {
                if (mousePos > lastMousePos + 2.5)
                {
                    ScrollDown(sender, args);
                }
                else if (mousePos < lastMousePos - 2.5)
                {
                    ScrollUp(sender, args);
                }
            }

            this.lastMousePos = input.MousePosition.Y;
        }

        /// <summary>
        /// Down Scroll Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ScrollDown(GUIComponent sender, OnMouseClickEventArgs args)
        {
            if (topItem < Items.Count - numItems)
            {
                topItem++;
                UpdateList();
                PosBar.UpdateLocation(PosBar.Location + new Vector2(0, barIntervals));
            }
        }

        /// <summary>
        /// Up Scroll Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ScrollUp(GUIComponent sender, OnMouseClickEventArgs args)
        {
            if (topItem > 0)
            {
                topItem--;
                UpdateList();
                PosBar.UpdateLocation(PosBar.Location + new Vector2(0, 0 - barIntervals));
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                GUIComponent c = Items[i];
                if (c != null)
                    c.Update(gameTime);
                if (disposed) return;
            }

            for (int i = 0; i < components.Count; i++)
            {
                GUIComponent c = components[i];
                if (c != null)
                    c.Update(gameTime);
                if (disposed) return;
            }

            base.Update(gameTime);
        }

        public override void UpdateLocation(Vector2 newLocation)
        {
            Location = newLocation;
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(fontDimensions.X + (textPaddingSide * 2)), Convert.ToInt32(fontDimensions.Y + (textPaddingTopAndBottom * 2)));
            textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);

            UpdateList();

            foreach (GUIComponent c in components)
            {
                c.Location = c.Location + new Vector2(Location.X, Location.Y);
            }
        }

        private void UpdateList()
        {
            GUIListBoxItem CurrentItem;
            for (int i = topItem; i < topItem + numItems; i++)
            {
                CurrentItem = (GUIListBoxItem)Items[i];
                CurrentItem.UpdateLocation(this.Location + new Vector2(0, (i - topItem) * CurrentItem.Bounds.Height));
            }
        }

        public string getSelectedItem()
        {
            string returnString = "";

            foreach (GUIListBoxItem lbi in Items)
            {
                if (lbi.Selected)
                {
                    returnString = lbi.Name;
                }
            }

            return returnString;
        }

        public override void Draw(GameTime gameTime)
        {
            primBatch.Begin(PrimitiveType.TriangleList);
            //white text area
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);

            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);

            primBatch.End();

            GUIListBoxItem CurrentItem;
            for (int i = topItem; i < topItem + numItems; i++)
            {
                CurrentItem = (GUIListBoxItem)Items[i];
                CurrentItem.Draw(gameTime);
            }

            foreach (GUIComponent c in components)
            {
                c.Draw(gameTime);
            }
        }
    }
}