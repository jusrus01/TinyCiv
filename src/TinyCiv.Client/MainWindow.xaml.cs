using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TinyCiv.Client.Code;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.Units;
using TinyCiv.Server.Client;
using TinyCiv.Shared;
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
        private GameGrid _gameGrid;
        private Player _currentPlayer;
        MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewModel;
            Loaded += PlayerConnection;
            Closed += PlayerDisconnect;
        }
        private void PlayerConnection(object sender, RoutedEventArgs e)
        {
            Thread playerConnectionThread = new Thread(() =>
            {
                Client = ServerClient.Create("http://localhost:5000");

                // Subscribe to events after creating the client
                Client.ListenForNewPlayerCreation(OnPlayerJoin);
                Client.ListenForGameStart(OnGameStart);
                Client.ListenForMapChange(OnMapChange);

                Client.SendAsync(new JoinLobbyClientEvent()).Wait();
            });

            playerConnectionThread.Start();
        }

        // Waiting for event implementation
        private async void PlayerDisconnect(object sender, EventArgs e)
        {

            // await Client.SendAsync(new PlayerDisconnectEvent(currentPlayer.Id));
        }

        private void OnPlayerJoin(JoinLobbyServerEvent response)
        {
            if (_currentPlayer == null)
            {
                _currentPlayer = response.Created;
                viewModel.PlayerColor = _currentPlayer.Color;
            }
            MessageBox.Show($"Player: {_currentPlayer.Id} has joined the game! They are in the {_currentPlayer.Color} team!");

            // If the party is full
            if (_currentPlayer == null)
            {
                MessageBox.Show("The game party is full! Try again later.");
            }
        }

        private void OnGameStart(GameStartServerEvent response)
        {
            _gameGrid = new GameGrid(UnitGrid, Constants.Game.HeightSquareCount, Constants.Game.WidthSquareCount);

            _gameGrid.GameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();

            _gameGrid.Client = Client;
            _gameGrid.CurrentPlayer = _currentPlayer;
            viewModel.PlayerColor = _currentPlayer.Color;

            InitializeMap();
            DrawGameObjects();
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            _gameGrid.GameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();

            DrawGameObjects();
        }

        private void InitializeMap()
        {
            Dispatcher.Invoke(() =>
            {
                for (int r = 0; r < Constants.Game.HeightSquareCount; r++)
                {
                    for (int c = 0; c < Constants.Game.WidthSquareCount; c++)
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(Constants.Assets.GameTile, UriKind.Relative));
                        MapGrid.Children.Add(image);
                    }
                }
            });
        }

        private void DrawGameObjects()
        {
            Dispatcher.Invoke(() =>
            {
                _gameGrid.Update();
            });
        }              
    }
}
