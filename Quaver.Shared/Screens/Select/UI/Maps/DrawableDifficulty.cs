/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Quaver.Shared.Assets;
using Quaver.Shared.Database.Maps;
using Quaver.Shared.Helpers;
using Quaver.Shared.Modifiers;
using Wobble;
using Wobble.Graphics;
using Wobble.Graphics.Animations;
using Wobble.Graphics.Sprites;
using Wobble.Graphics.UI.Buttons;
using Wobble.Input;
using ColorHelper = Quaver.Shared.Helpers.ColorHelper;

namespace Quaver.Shared.Screens.Select.UI.Maps
{
    public class DrawableDifficulty : Button
    {
        /// <summary>
        ///     Reference to the parent container.
        /// </summary>
        private DifficultyScrollContainer Container { get; }

        /// <summary>
        ///     The map this difficulty is for.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        ///     The name of the difficulty.
        /// </summary>
        private SpriteTextBitmap DifficultyName { get; }

        /// <summary>
        ///     Displays the actual difficulty rating.
        /// </summary>
        private SpriteTextBitmap TextDifficultyRating { get; }

        /// <summary>
        ///     Displays the creator of the map.
        /// </summary>
        private SpriteTextBitmap Creator { get; }

        /// <summary>
        ///     The height of the drawable mapset.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static int HEIGHT { get; } = 84;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public DrawableDifficulty(DifficultyScrollContainer container)
        {
            Container = container;

            Size = new ScalableVector2(410, HEIGHT);
            Image = UserInterface.SelectButtonBackground;

            DifficultyName = new SpriteTextBitmap(FontsBitmap.GothamRegular, " ")
            {
                Parent = this,
                Position = new ScalableVector2(15, 12),
                FontSize = 18
            };

            TextDifficultyRating = new SpriteTextBitmap(FontsBitmap.GothamRegular, " ")
            {
                Parent = this,
                Position = new ScalableVector2(DifficultyName.X, DifficultyName.Y + DifficultyName.Height + 4),
                Tint = ColorHelper.DifficultyToColor(19.12f),
                FontSize = 16
            };

            Creator = new SpriteTextBitmap(FontsBitmap.GothamRegular, " ")
            {
                Parent = this,
                Alignment = Alignment.TopRight,
                Position = new ScalableVector2(-5, TextDifficultyRating.Y + TextDifficultyRating.Height + 6),
                FontSize = 14
            };

            Clicked += OnClicked;
            ModManager.ModsChanged += OnModsChanged;
        }

        /// <summary />
        public override void Destroy()
        {
            DifficultyName.Destroy();
            TextDifficultyRating.Destroy();
            Creator.Destroy();

            ModManager.ModsChanged -= OnModsChanged;
            base.Destroy();
        }

        /// <summary>
        ///     Updates the information of the map with a new one.
        /// </summary>
        /// <param name="map"></param>
        public void UpdateWithNewMap(Map map)
        {
            Map = map;

            DifficultyName.Text = map.DifficultyName;

            var difficulty = map.DifficultyFromMods(ModManager.Mods).Average();

            TextDifficultyRating.Text = StringHelper.AccuracyToString(difficulty).Replace("%", "");
            TextDifficultyRating.Tint = ColorHelper.DifficultyToColor(difficulty);
            Creator.Text = $"By: {map.Creator}";
        }

        /// <summary>
        ///     Displays the difficulty when it is selected.
        /// </summary>
        public void DisplayAsSelected()
        {
            Animations.Clear();
            ChangeWidthTo(500, Easing.OutQuint, 400);
            Image = UserInterface.SelectedMapset;

            DifficultyName.Animations.Clear();
            TextDifficultyRating.Animations.Clear();
            Creator.ClearAnimations();

            DifficultyName.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, DifficultyName.Alpha, 1f, 400));
            TextDifficultyRating.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, TextDifficultyRating.Alpha, 1f, 400));
            Creator.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, Creator.Alpha, 1f, 400));
        }

        /// <summary>
        ///     Displays the difficulty when deselected.
        /// </summary>
        public void DisplayAsDeselected()
        {
            Animations.Clear();
            ChangeWidthTo(410, Easing.OutQuint, 400);
            Image = UserInterface.DeselectedMapset;

            DifficultyName.Animations.Clear();
            TextDifficultyRating.Animations.Clear();
            Creator.ClearAnimations();

            DifficultyName.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, DifficultyName.Alpha, 0.65f, 400));
            TextDifficultyRating.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, TextDifficultyRating.Alpha, 0.65f, 400));
            Creator.Animations.Add(new Animation(AnimationProperty.Alpha, Easing.OutQuint, Creator.Alpha, 0.65f, 400));
        }

        /// <summary>
        ///     Called when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClicked(object sender, EventArgs e)
        {
            if (Map == MapManager.Selected.Value)
            {
                var game = GameBase.Game as QuaverGame;
                var screen = game.CurrentScreen as SelectScreen;
                screen?.ExitToGameplay();
                return;
            }

            var view = Container.View;
            view.MapsetScrollContainer.SelectMap(view.MapsetScrollContainer.SelectedMapsetIndex, Map, true);
        }

        /// <inheritdoc />
        /// <summary>
        ///     In this case, we only want buttons to be clickable if they're in the bounds of the scroll container.
        /// </summary>
        /// <returns></returns>
        protected override bool IsMouseInClickArea()
        {
            var newRect = RectangleF.Intersect(ScreenRectangle, Container.ScreenRectangle);
            return GraphicsHelper.RectangleContains(newRect, MouseManager.CurrentState.Position);
        }

        /// <summary>
        ///     Called when the activated modifiers have changed.
        ///     Used for updating difficulty values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModsChanged(object sender, ModsChangedEventArgs e)
        {
            if (Map == null)
                return;

            var difficulty = (float) Map.DifficultyFromMods(ModManager.Mods).Average();

            var value = StringHelper.AccuracyToString(difficulty).Replace("%", "");

            // Don't bother updating the text if the difficulty value is the same as what's already there
            if (value == TextDifficultyRating.Text)
                return;

            TextDifficultyRating.Text = value;
            TextDifficultyRating.Tint = ColorHelper.DifficultyToColor(difficulty);
        }
    }
}
