using MongoDB.Driver;
using MongoDB.Bson;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using AljasAuthApi.Models;

namespace AljasAuthApi.Services
{
    public class MealCountWebSocketService
    {
        private readonly IMongoCollection<Rawdata> _rawDataCollection;
        private readonly DashboardService _dashboardService;
        private readonly List<WebSocket> _clients = new();
        private readonly object _lock = new();

        public MealCountWebSocketService(IMongoDatabase database, DashboardService dashboardService)
        {
            _rawDataCollection = database.GetCollection<Rawdata>("rawdata");
            _dashboardService = dashboardService;
        }

     public async Task StartAsync()
{
    Console.WriteLine("🔁 Listening to rawdata changes...");

    var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Rawdata>>()
        .Match(change => change.OperationType == ChangeStreamOperationType.Insert);

    var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };

    var cursor = await _rawDataCollection.WatchAsync(pipeline, options);

    _ = Task.Run(async () =>
    {
        while (await cursor.MoveNextAsync())
        {
            foreach (var change in cursor.Current)
            {
                Console.WriteLine("📦 Detected change in rawdata collection");

                var mealCounts = await _dashboardService.GetDashboardMealCountsAsync();

                var response = mealCounts.Select(summary => new
                {
                    dashboardid = summary.Dashboardid.ToString(),
                    dashboardName = summary.DashboardName,
                    locationId = summary.LocationId,
                    canteenId = summary.CanteenId,
                    mealTypeCounts = new
                    {
                        today = summary.MealTypeCounts.Today,
                        thisWeek = summary.MealTypeCounts.ThisWeek,
                        thisMonth = summary.MealTypeCounts.ThisMonth
                    },
                    totalToday = summary.TotalToday,
                    totalThisWeek = summary.TotalThisWeek,
                    totalThisMonth = summary.TotalThisMonth
                }).ToList();

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
                var buffer = Encoding.UTF8.GetBytes(jsonResponse);
                var segment = new ArraySegment<byte>(buffer);

                lock (_lock)
                {
                    _clients.RemoveAll(c => c.State != WebSocketState.Open);
                }

                // Send the updated data to each connected client
                foreach (var client in _clients)
                {
                    try
                    {
                        if (client.State == WebSocketState.Open)
                        {
                            await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                            Console.WriteLine("📤 Sent meal count update to WebSocket clients");
                        }
                    }
                    catch
                    {
                        lock (_lock)
                        {
                            _clients.Remove(client);
                        }
                    }
                }
            }
        }
    });
}


public async Task HandleWebSocketAsync(WebSocket webSocket)
{
    Console.WriteLine("🧩 WebSocket client connected");

    lock (_lock)
    {
        _clients.Add(webSocket);
    }

    // Send initial data (current data) to the client
    var mealCounts = await _dashboardService.GetDashboardMealCountsAsync();
    var response = mealCounts.Select(summary => new
    {
        dashboardid = summary.Dashboardid.ToString(),
        dashboardName = summary.DashboardName,
        locationId = summary.LocationId,
        canteenId = summary.CanteenId,
        mealTypeCounts = new
        {
            today = summary.MealTypeCounts.Today,
            thisWeek = summary.MealTypeCounts.ThisWeek,
            thisMonth = summary.MealTypeCounts.ThisMonth
        },
        totalToday = summary.TotalToday,
        totalThisWeek = summary.TotalThisWeek,
        totalThisMonth = summary.TotalThisMonth
    }).ToList();

    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
    var buffer = Encoding.UTF8.GetBytes(jsonResponse);
    var segment = new ArraySegment<byte>(buffer);

    // Send the initial data to the WebSocket client
    await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
    Console.WriteLine("📤 Sent initial meal count data");

    // Receive WebSocket messages (keep connection alive)
    var receiveBuffer = new byte[1024 * 4];
    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
            lock (_lock)
            {
                _clients.Remove(webSocket);
            }
        }
    }
}

    }
}
