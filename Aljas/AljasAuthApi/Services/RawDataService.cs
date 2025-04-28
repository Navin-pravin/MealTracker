using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RawDataService
{
    private readonly IMongoCollection<RawData> _rawDataCollection;

    public RawDataService(IMongoDatabase database)
    {
        _rawDataCollection = database.GetCollection<RawData>("rawdata");
    }

    // Insert Data
    public async Task InsertRawDataAsync(RawData rawData)
    {
        await _rawDataCollection.InsertOneAsync(rawData);
    }
}