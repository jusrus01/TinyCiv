using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private GameGrid gameGrid;
        private Player currentPlayer;

        private static IServerClient Client = ClientManager.Instance.Client;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += PlayerConnection;           
  
            Client.ListenForNewPlayerCreation(OnPlayerJoin);
            Client.ListenForGameStart(OnGameStart);
            Client.ListenForMapChange(OnMapChange);
        }
        private async void PlayerConnection(object sender, RoutedEventArgs e)
        {
            await Client.SendAsync(new JoinLobbyClientEvent());                    
        }

        private void OnPlayerJoin(JoinLobbyServerEvent response)
        {
            currentPlayer = response.Created;

            // If the party is full
            if (currentPlayer == null)
            {
                MessageBox.Show("The game party is full! Try again later.");
            }
        }

        private void OnGameStart(GameStartServerEvent response)
        {
            Console.WriteLine("Game started since two players already joined");
            Console.WriteLine($"Map to render: {response.Map}");
            
            gameGrid = new GameGrid(UnitGrid, 20, 20);

            gameGrid.gameObjects = response.Map.Objects.Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();

            InitializeMap();
            gameGrid.Update();
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            //gameGrid.gameObjects = response;
            Console.WriteLine("Mp changes received");

            gameGrid.Update();
        }

        private void InitializeMap()
        {
            for (int r = 0; r < 20; r++)
            {
                for(int c = 0; c < 20; c++) {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri("Assets/game_tile.png", UriKind.Relative));
                    MapGrid.Children.Add(image);
                }
            }            
        }
    }
}
