using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AljasAuthApi.Models;

public class RawDataService
{
    private readonly IMongoCollection<Rawdata> _rawDataCollection;

    public RawDataService(IMongoDatabase database)
    {
        _rawDataCollection = database.GetCollection<Rawdata>("rawdata");
    }

    // Insert Data
    public async Task InsertRawDataAsync(Rawdata rawData)
    {
        await _rawDataCollection.InsertOneAsync(rawData);
    }
}