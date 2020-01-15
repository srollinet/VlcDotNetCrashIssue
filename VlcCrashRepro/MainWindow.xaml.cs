using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VlcCrashRepro
{
    public partial class MainWindow : Window
    {
        private const int NbRows = 4;
        private const int NbColumns = 4;

        private static readonly Random Rnd = new Random(42);
        private static readonly TimeSpan DelayBetweenAffectations = TimeSpan.FromSeconds(4);

        private static readonly IReadOnlyList<Uri> StreamUris = new[]
            {
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerFun.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerJoyrides.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerMeltdowns.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/Sintel.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/SubaruOutbackOnStreetAndDirt.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/TearsOfSteel.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/VolkswagenGTIReview.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/WeAreGoingOnBullrun.mp4",
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/WhatCarCanYouGetForAGrand.mp4" ,
            }.Select(val => new Uri(val))
            .ToArray();

        public MainWindow()
        {
            InitializeComponent();
            LibVLCSharp.Shared.Core.Initialize();

            for (var col = 0; col < NbColumns; col++)
            {
                PlayerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var row = 0; row < NbRows; row++)
            {
                PlayerGrid.RowDefinitions.Add(new RowDefinition());
            }

            var players = new List<VideoPlayer>(NbRows * NbColumns);
            for (var row = 0; row < NbRows; row++)
            {
                for (var col = 0; col < NbColumns; col++)
                {
                    var player = new VideoPlayer();
                    players.Add(player);
                    PlayerGrid.Children.Add(player);
                    Grid.SetColumn(player, col);
                    Grid.SetRow(player, row);
                }
            }

            Task ChangeMatrixAffectations()
            {
                var tasks = players.Select(p =>
                {
                    var uri = StreamUris[Rnd.Next(StreamUris.Count)];
                    return Dispatcher.InvokeAsync(() => { p.Play(uri); }).Task;
                });

                return Task.WhenAll(tasks);
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    await ChangeMatrixAffectations();
                    await Task.Delay(DelayBetweenAffectations);
                }
            });
        }
    }
}