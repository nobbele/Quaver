/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Quaver.Server.Client;
using Quaver.Shared.Config;
using Quaver.Shared.Database.Scores;
using Quaver.Shared.Graphics.Notifications;
using Quaver.Shared.Helpers;
using SQLite;
using Wobble.Bindables;
using Wobble.Platform;

namespace Quaver.Shared.Database.Maps
{
    public class Map
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     The MD5 Of the file.
        /// </summary>
        [Unique]
        public string Md5Checksum { get; set; }

        /// <summary>
        ///     The directory of the map
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     The absolute path of the .qua file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     The map set id of the map.
        /// </summary>
        public int MapSetId { get; set; }

        /// <summary>
        ///     The id of the map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        ///     The song's artist
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     The song's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The difficulty name of the map
        /// </summary>
        public string DifficultyName { get; set; }

        /// <summary>
        ///     The highest rank that the player has gotten on the map.
        /// </summary>
        public Grade HighestRank { get; set; }

        /// <summary>
        ///     The ranked status of the map.
        /// </summary>
        public RankedStatus RankedStatus { get; set; }

        /// <summary>
        ///     The last time the user has played the map.
        /// </summary>
        public string LastPlayed { get; set; } = new DateTime(0001, 1, 1, 00, 00, 00).ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        ///     The creator of the map.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     The absolute path of the map's background.
        /// </summary>
        public string BackgroundPath { get; set; }

        /// <summary>
        ///     The absolute path of the map's audio.
        /// </summary>
        public string AudioPath { get; set; }

        /// <summary>
        ///     The audio preview time of the map
        /// </summary>
        public int AudioPreviewTime { get; set; }

        /// <summary>
        ///     The description of the map
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The source (album/mixtape/etc) of the map
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Tags for the map
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        ///     The most common bpm for the map
        /// </summary>
        public double Bpm { get; set; }

        /// <summary>
        ///     The map's length (Time of the last hit object)
        /// </summary>
        public int SongLength { get; set; }

        /// <summary>
        ///     The Game Mode of the map
        /// </summary>
        public GameMode Mode { get; set; }

        /// <summary>
        ///     The local offset for this map
        /// </summary>
        public int LocalOffset { get; set; }

        /// <summary>
        ///     The version of the difficulty calculator that was used in this cache
        /// </summary>
        public string DifficultyProcessorVersion { get; set; }

        /// <summary>
        ///     The last time the file was modified
        /// </summary>
        public DateTime LastFileWrite { get; set; }

        /// <summary>
        ///     The date the map was added.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        ///     The count of regular notes.
        /// </summary>
        public int RegularNoteCount { get; set; }

        /// <summary>
        ///     The count of long notes.
        /// </summary>
        public int LongNoteCount { get; set; }

        /// <summary>
        ///     The percentage of long notes among the map's hit objects.
        /// </summary>
        public float LNPercentage
        {
            get
            {
                var hitObjectCount = RegularNoteCount + LongNoteCount;
                return hitObjectCount == 0 ? 0 : ((float) LongNoteCount / hitObjectCount * 100);
            }
        }

#region DIFFICULTY_RATINGS
        [Ignore]
        public float[] Difficulty05X { get; set; }
        [Ignore]
        public float[] Difficulty055X { get; set; }
        [Ignore]
        public float[] Difficulty06X { get; set; }
        [Ignore]
        public float[] Difficulty065X { get; set; }
        [Ignore]
        public float[] Difficulty07X { get; set; }
        [Ignore]
        public float[] Difficulty075X { get; set; }
        [Ignore]
        public float[] Difficulty08X { get; set; }
        [Ignore]
        public float[] Difficulty085X { get; set; }
        [Ignore]
        public float[] Difficulty09X { get; set; }
        [Ignore]
        public float[] Difficulty095X { get; set; }
        [Ignore]
        public float[] Difficulty10X { get; set; }
        [Ignore]
        public float[] Difficulty11X { get; set; }
        [Ignore]
        public float[] Difficulty12X { get; set; }
        [Ignore]
        public float[] Difficulty13X { get; set; }
        [Ignore]
        public float[] Difficulty14X { get; set; }
        [Ignore]
        public float[] Difficulty15X { get; set; }
        [Ignore]
        public float[] Difficulty16X { get; set; }
        [Ignore]
        public float[] Difficulty17X { get; set; }
        [Ignore]
        public float[] Difficulty18X { get; set; }
        [Ignore]
        public float[] Difficulty19X { get; set; }
        [Ignore]
        public float[] Difficulty20X { get; set; }

        public string StrDifficulty05X { get => string.Join(",", Difficulty05X); set => Difficulty05X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty055X { get => string.Join(",", Difficulty055X); set => Difficulty055X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty06X { get => string.Join(",", Difficulty06X); set => Difficulty06X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty065X { get => string.Join(",", Difficulty065X); set => Difficulty065X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty07X { get => string.Join(",", Difficulty07X); set => Difficulty07X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty075X { get => string.Join(",", Difficulty075X); set => Difficulty075X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty08X { get => string.Join(",", Difficulty08X); set => Difficulty08X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty085X { get => string.Join(",", Difficulty085X); set => Difficulty085X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty09X { get => string.Join(",", Difficulty09X); set => Difficulty09X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty095X { get => string.Join(",", Difficulty095X); set => Difficulty095X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty10X { get => string.Join(",", Difficulty10X); set => Difficulty10X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty11X { get => string.Join(",", Difficulty11X); set => Difficulty11X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty12X { get => string.Join(",", Difficulty12X); set => Difficulty12X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty13X { get => string.Join(",", Difficulty13X); set => Difficulty13X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty14X { get => string.Join(",", Difficulty14X); set => Difficulty14X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty15X { get => string.Join(",", Difficulty15X); set => Difficulty15X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty16X { get => string.Join(",", Difficulty16X); set => Difficulty16X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty17X { get => string.Join(",", Difficulty17X); set => Difficulty17X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty18X { get => string.Join(",", Difficulty18X); set => Difficulty18X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty19X { get => string.Join(",", Difficulty19X); set => Difficulty19X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        public string StrDifficulty20X { get => string.Join(",", Difficulty20X); set => Difficulty20X = value?.Split(',')?.Select(str => float.Parse(str))?.ToArray() ; }
        #endregion

        /// <summary>
        ///     Determines if this map is an osu! map.
        /// </summary>
        [Ignore]
        public MapGame Game { get; set; } = MapGame.Quaver;

        /// <summary>
        ///     The actual parsed qua file for the map.
        /// </summary>
        [Ignore]
        public Qua Qua { get; set; }

        /// <summary>
        ///     The mapset the map belongs to.
        /// </summary>
        [Ignore]
        public Mapset Mapset { get; set; }

        /// <summary>
        ///     The scores for this map.
        /// </summary>
        [Ignore]
        public Bindable<List<Score>> Scores { get; } = new Bindable<List<Score>>(null);

        /// <summary>
        ///     Determines if this map is outdated and needs an update.
        /// </summary>
        [Ignore]
        public bool NeedsOnlineUpdate { get; set; }

        /// <summary>
        ///     Determines if the map is newly created and should open the metadata screen in the editor.
        /// </summary>
        [Ignore]
        public bool NewlyCreated { get; set; }

        /// <summary>
        ///     If set to true, when the user loads the map up in the editor, it
        ///     will prompt the user if they would like to remove all HitObjects from the map.
        /// </summary>
        [Ignore]
        public bool AskToRemoveHitObjects { get; set; }

        /// <summary>
        ///     Responsible for converting a Qua object, to a Map object
        ///     a Map object is one that is stored in the db.
        /// </summary>
        /// <param name="qua"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Map FromQua(Qua qua, string path, bool skipPathSetting = false)
        {
            var map = new Map
            {
                Artist = qua.Artist,
                Title = qua.Title,
                HighestRank = Grade.None,
                AudioPath = qua.AudioFile,
                AudioPreviewTime = qua.SongPreviewTime,
                BackgroundPath = qua.BackgroundFile,
                Description = qua.Description,
                MapId = qua.MapId,
                MapSetId = qua.MapSetId,
                Creator = qua.Creator,
                DifficultyName = qua.DifficultyName,
                Source = qua.Source,
                Tags = qua.Tags,
                SongLength =  qua.Length,
                Mode = qua.Mode,
                RegularNoteCount = qua.HitObjects.Count(x => !x.IsLongNote),
                LongNoteCount = qua.HitObjects.Count(x => x.IsLongNote),
            };

            if (!skipPathSetting)
            {
                try
                {
                    map.Md5Checksum = MapsetHelper.GetMd5Checksum(path);
                    map.Directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? throw new InvalidOperationException()).Name.Replace("\\", "/");
                    map.Path = System.IO.Path.GetFileName(path)?.Replace("\\", "/");
                    map.LastFileWrite = File.GetLastWriteTimeUtc(map.Path);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {

                }
            }

            try
            {
                map.Bpm = qua.GetCommonBpm();
            }
            catch (Exception)
            {
                map.Bpm = 0;
            }

            map.DateAdded = DateTime.Now;
            return map;
        }

        /// <summary>
        ///     Loads the .qua, .osu or .sm file for a map.
        ///
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Qua LoadQua(bool checkValidity = true)
        {
            // Reference to the parsed .qua file
            Qua qua;

            // Handle osu! maps as well
            switch (Game)
            {
                case MapGame.Quaver:
                    var quaPath = $"{ConfigManager.SongDirectory}/{Directory}/{Path}";
                    qua = Qua.Parse(quaPath, checkValidity);
                    break;
                case MapGame.Osu:
                    var osu = new OsuBeatmap(MapManager.OsuSongsFolder + Directory + "/" + Path);
                    qua = osu.ToQua();
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            return qua;
        }

        /// <summary>
        ///     Changes selected map
        /// </summary>
        public void ChangeSelected()
        {
            MapManager.Selected.Value = this;

            Task.Run(async () =>
            {
                using (var writer = File.CreateText(ConfigManager.DataDirectory + "/temp/Now Playing/map.txt"))
                {
                    await writer.WriteAsync($"{Artist} - {Title} [{DifficultyName}]");
                }
            });
        }

        /// <summary>
        ///     Unhooks all of the event handlers and clears the scores.
        /// </summary>
        public void ClearScores()
        {
            Scores.UnHookEventHandlers();
            Scores.Value?.Clear();
        }

        /// <summary>
        ///     Calculates the difficulty of the entire map
        /// </summary>
        public void CalculateDifficulties()
        {
            var qua = LoadQua(false);

            Difficulty05X = qua.SolveDifficulty(ModIdentifier.Speed05X).Difficulty;
            Difficulty055X = qua.SolveDifficulty(ModIdentifier.Speed055X).Difficulty;
            Difficulty06X = qua.SolveDifficulty(ModIdentifier.Speed06X).Difficulty;
            Difficulty065X = qua.SolveDifficulty(ModIdentifier.Speed065X).Difficulty;
            Difficulty07X = qua.SolveDifficulty(ModIdentifier.Speed07X).Difficulty;
            Difficulty075X = qua.SolveDifficulty(ModIdentifier.Speed075X).Difficulty;
            Difficulty08X = qua.SolveDifficulty(ModIdentifier.Speed08X).Difficulty;
            Difficulty085X = qua.SolveDifficulty(ModIdentifier.Speed085X).Difficulty;
            Difficulty09X = qua.SolveDifficulty(ModIdentifier.Speed09X).Difficulty;
            Difficulty095X = qua.SolveDifficulty(ModIdentifier.Speed095X).Difficulty;
            Difficulty10X = qua.SolveDifficulty().Difficulty;
            Difficulty11X = qua.SolveDifficulty(ModIdentifier.Speed11X).Difficulty;
            Difficulty12X = qua.SolveDifficulty(ModIdentifier.Speed12X).Difficulty;
            Difficulty13X = qua.SolveDifficulty(ModIdentifier.Speed13X).Difficulty;
            Difficulty14X = qua.SolveDifficulty(ModIdentifier.Speed14X).Difficulty;
            Difficulty15X = qua.SolveDifficulty(ModIdentifier.Speed15X).Difficulty;
            Difficulty16X = qua.SolveDifficulty(ModIdentifier.Speed16X).Difficulty;
            Difficulty17X = qua.SolveDifficulty(ModIdentifier.Speed17X).Difficulty;
            Difficulty18X = qua.SolveDifficulty(ModIdentifier.Speed18X).Difficulty;
            Difficulty19X = qua.SolveDifficulty(ModIdentifier.Speed19X).Difficulty;
            Difficulty20X = qua.SolveDifficulty(ModIdentifier.Speed20X).Difficulty;
        }

        /// <summary>
        ///     Retrieve the map's difficulty rating from given mods
        /// </summary>
        /// <returns></returns>
        public float[] DifficultyFromMods(ModIdentifier mods)
        {
            if (mods.HasFlag(ModIdentifier.Speed05X))
                return Difficulty05X;
            if (mods.HasFlag(ModIdentifier.Speed055X))
                return Difficulty055X;
            if (mods.HasFlag(ModIdentifier.Speed06X))
                return Difficulty06X;
            if (mods.HasFlag(ModIdentifier.Speed065X))
                return Difficulty065X;
            if (mods.HasFlag(ModIdentifier.Speed07X))
                return Difficulty07X;
            if (mods.HasFlag(ModIdentifier.Speed075X))
                return Difficulty075X;
            if (mods.HasFlag(ModIdentifier.Speed08X))
                return Difficulty08X;
            if (mods.HasFlag(ModIdentifier.Speed085X))
                return Difficulty085X;
            if (mods.HasFlag(ModIdentifier.Speed09X))
                return Difficulty09X;
            if (mods.HasFlag(ModIdentifier.Speed095X))
                return Difficulty095X;
            if (mods.HasFlag(ModIdentifier.Speed11X))
                return Difficulty11X;
            if (mods.HasFlag(ModIdentifier.Speed12X))
                return Difficulty12X;
            if (mods.HasFlag(ModIdentifier.Speed13X))
                return Difficulty13X;
            if (mods.HasFlag(ModIdentifier.Speed14X))
                return Difficulty14X;
            if (mods.HasFlag(ModIdentifier.Speed15X))
                return Difficulty15X;
            if (mods.HasFlag(ModIdentifier.Speed16X))
                return Difficulty16X;
            if (mods.HasFlag(ModIdentifier.Speed17X))
                return Difficulty17X;
            if (mods.HasFlag(ModIdentifier.Speed18X))
                return Difficulty18X;
            if (mods.HasFlag(ModIdentifier.Speed19X))
                return Difficulty19X;
            if (mods.HasFlag(ModIdentifier.Speed20X))
                return Difficulty20X;

            return Difficulty10X;
        }

        /// <summary>
        ///     Opens the browser to visit the mapset's page.
        /// </summary>
        public void VisitMapsetPage()
        {
            if (MapSetId == -1)
            {
                NotificationManager.Show(NotificationLevel.Error, "This mapset is not uploaded to the Quaver servers.");
                return;
            }

            BrowserHelper.OpenURL(MapId != -1 ? $"{OnlineClient.WEBSITE_URL}/mapsets/map/{MapId}" : $"{OnlineClient.WEBSITE_URL}/mapsets/{MapSetId}");
        }

        /// <summary>
        ///     Opens the folder to the mapset while highlighting the file.
        /// </summary>
        public void OpenFolder()
        {
            if (MapManager.Selected.Value.Game != MapGame.Quaver)
            {
                NotificationManager.Show(NotificationLevel.Error, "You cannot open a folder for a map loaded from another game.");
                return;
            }

            var path = $"{ConfigManager.SongDirectory.Value}/{Directory}/{Path}";
            Utils.NativeUtils.HighlightInFileManager(path);
        }

        /// <summary>
        ///     Opens the .qua file
        /// </summary>
        public void OpenFile()
        {
            if (MapManager.Selected.Value.Game != MapGame.Quaver)
            {
                NotificationManager.Show(NotificationLevel.Error, "You cannot open a .qua loaded from another game.");
                return;
            }

            try
            {
                Process.Start("notepad.exe", "\"" + $"{ConfigManager.SongDirectory.Value}/{Directory}/{Path}".Replace("/", "\\") + "\"");
            }
            catch (Exception e)
            {
                NotificationManager.Show(NotificationLevel.Error, "An error occurred while opening the file.");
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public List<double> GetDifficultyRatings()
        {
            var ratings = new List<double>
            {
                Difficulty05X.Average(),
                Difficulty055X.Average(),
                Difficulty06X.Average(),
                Difficulty065X.Average(),
                Difficulty07X.Average(),
                Difficulty075X.Average(),
                Difficulty08X.Average(),
                Difficulty085X.Average(),
                Difficulty09X.Average(),
                Difficulty095X.Average(),
                Difficulty10X.Average(),
                Difficulty11X.Average(),
                Difficulty12X.Average(),
                Difficulty13X.Average(),
                Difficulty14X.Average(),
                Difficulty15X.Average(),
                Difficulty16X.Average(),
                Difficulty17X.Average(),
                Difficulty18X.Average(),
                Difficulty19X.Average(),
                Difficulty20X.Average()
            };

            for (var i = 0; i < ratings.Count; i++)
                ratings[i] = Math.Round(ratings[i], 2);

            return ratings;
        }

        public int GetJudgementCount() => LongNoteCount * 2 + RegularNoteCount;

        /// <summary>
        ///     If the map is non-Quaver, then the map needs to be converted
        /// </summary>
        /// <returns></returns>
        public string GetAlternativeMd5() => Game == MapGame.Quaver ? Md5Checksum : CryptoHelper.StringToMd5(LoadQua().Serialize());

        public override string ToString() => $"{Artist} - {Title} [{DifficultyName}]";
    }

    /// <summary>
    ///     The game in which the map belongs to
    /// </summary>
    public enum MapGame
    {
        Quaver,
        Osu,
    }
}
