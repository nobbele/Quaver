﻿using System;
using System.Collections.Generic;
using Quaver.Shared.Assets;
using Quaver.Shared.Config;
using Quaver.Shared.Database.Maps;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Scheduling;
using Wobble.Graphics;
using Wobble.Platform;

namespace Quaver.Shared.Screens.Editor.UI.Navigation
{
    public class MainEditorNavigationBar : EditorNavigationBar
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public MainEditorNavigationBar(EditorScreen screen) : base(new List<EditorControlButton>
        {
            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_home), "Back To Menu", -48, Alignment.BotLeft,
                (o, e) => screen.HandleKeyPressEscape()),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_text_file), "Edit Metadata", -48, Alignment.BotLeft,
                (o, e) => NotificationManager.Show(NotificationLevel.Warning, "Not implemented yet")),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_time), "Timing Setup", -48, Alignment.BotLeft,
                (o, e) => NotificationManager.Show(NotificationLevel.Warning, "Not implemented yet")),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_dashboard), "Edit Scroll Velocities", -48, Alignment.BotLeft,
                (o, e) => NotificationManager.Show(NotificationLevel.Warning, "Not implemented yet")),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_music_note_black_symbol), "Set Audio Preview Time", -48, Alignment.BotLeft,
                (o, e) => NotificationManager.Show(NotificationLevel.Warning, "Not implemented yet")),

        }, new List<EditorControlButton>
        {
            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_earth_globe), "Visit Mapset Page", -48, Alignment.BotRight,
                (o, e) => MapManager.Selected.Value.VisitMapsetPage()),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_cloud_storage_uploading_option), "Upload Mapset", -48, Alignment.BotRight,
                (o, e) => NotificationManager.Show(NotificationLevel.Warning, "Not implemented yet")),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_archive_black_box), "Export Mapset", -48, Alignment.BotRight,
                (o, e) => ExportToZip()),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_save_file_option), "Save File", -48, Alignment.BotRight,
                (o, e) => screen.Save()),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_file), "Edit .qua File", -48, Alignment.BotRight,
                (o, e) => MapManager.Selected.Value.OpenFile()),

            new EditorControlButton(FontAwesome.Get(FontAwesomeIcon.fa_open_folder), "Open Mapset Folder", -48, Alignment.BotRight,
                (o, e) => MapManager.Selected.Value.OpenFolder()),
        })
        {
        }

        /// <summary>
        /// </summary>
        private static void ExportToZip()
        {
            MapManager.Selected.Value.Mapset.ExportToZip();
            ThreadScheduler.RunAfter(() => NotificationManager.Show(NotificationLevel.Success, "Successfully exported mapset!"), 100);
        }
    }
}