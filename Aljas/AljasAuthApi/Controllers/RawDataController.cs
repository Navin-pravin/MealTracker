using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AljasAuthApi.Models;


[ApiController]
[Route("api/[controller]")]
public class RawDataController : ControllerBase
{
    private readonly RawDataService _rawDataService;

    public RawDataController(RawDataService rawDataService)
    {
        _rawDataService = rawDataService;
    }

    // Insert Data Endpoint
    [HttpPost]
    [Route("insert")]
    public async Task<IActionResult> InsertRawData([FromBody] Rawdata rawData)
    {
        if (rawData == null)
        {
            return BadRequest("Invalid data.");
        }

        await _rawDataService.InsertRawDataAsync(rawData);
        return Ok("Data inserted successfully.");
    }
}