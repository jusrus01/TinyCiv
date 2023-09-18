using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Client.Code;
using TinyCiv.Client.Code.units;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IServerClient Client;
        private GameGrid gameGrid;
        private Player currentPlayer;

        private bool isUnitSelected = false;
        private int unitIndex;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += PlayerConnection;
            Closed += PlayerDisconnect;
        }
        private async void PlayerConnection(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                Client = ServerClient.Create("http://localhost:5000");

                // Subscribe to events after creating the client
                Client.ListenForNewPlayerCreation(OnPlayerJoin);
                Client.ListenForGameStart(OnGameStart);
                Client.ListenForMapChange(OnMapChange);

                await Client.SendAsync(new JoinLobbyClientEvent());
            });
        }

        // Waiting for event implementation
        private async void PlayerDisconnect(object sender, EventArgs e)
        {
            // await Client.SendAsync(new PlayerDisconnectEvent(currentPlayer.Id));
        }

        private void OnPlayerJoin(JoinLobbyServerEvent response)
        {
            currentPlayer = response.Created;
            MessageBox.Show("Player: " + currentPlayer.Id + " has joined the game!");

            // If the party is full
            if (currentPlayer == null)
            {
                MessageBox.Show("The game party is full! Try again later.");
            }
        }

        private void OnGameStart(GameStartServerEvent response)
        {
            gameGrid = new GameGrid(UnitGrid, 20, 20);

            gameGrid.gameObjects = response.Map.Objects.Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();

            InitializeMap();
            DrawGameObjects();
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            gameGrid.gameObjects = response.Map.Objects.Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>(); ;            

            DrawGameObjects();
        }

        private void InitializeMap()
        {
            Dispatcher.Invoke(() =>
            {
                for (int r = 0; r < 20; r++)
                {
                    for (int c = 0; c < 20; c++)
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri("Assets/game_tile.png", UriKind.Relative));
                        MapGrid.Children.Add(image);
                    }
                }
            });
        }

        private void DrawGameObjects()
        {
            Dispatcher.Invoke(() =>
            {
                gameGrid.Update();
            });
        }              
    }
}
