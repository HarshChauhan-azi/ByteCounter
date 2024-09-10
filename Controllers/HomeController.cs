using Microsoft.AspNetCore.Mvc;

namespace ByteCounter.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CountHexValues(string hexInput)
    {
        ViewBag.InputHex = hexInput;

        if (string.IsNullOrEmpty(hexInput))
        {
            ViewBag.Message = "Please provide valid input!";
            return View("Index");
        }

        string cleanedInput = hexInput.Replace("CC", "")
                                      .Replace("AA", "")
                                      .Replace("55", "")
                                      .Replace("33", "")
                                      .Replace(" ", ""); 

        if (cleanedInput.Length % 2 != 0)
        {
            ViewBag.Message = "Input contains an incomplete byte (odd number of characters).";
            return View("Index");
        }

        var hexPairs = Enumerable.Range(0, cleanedInput.Length / 2)
                                 .Select(i => cleanedInput.Substring(i * 2, 2))
                                 .ToList();

        string crc = hexPairs.LastOrDefault();

        int totalCount = hexPairs.Count(); 

        ViewBag.TotalCount = totalCount;
        ViewBag.Crc = crc;  
        
        ViewBag.PositionMessage = TempData["PositionMessage"];

        return View("Index");
    }

    [HttpPost]
    public IActionResult GetPositionOfHexValue(string hexInput, string searchValue)
    {
        ViewBag.InputHex = hexInput;  
        ViewBag.SearchValue = searchValue; 

        string cleanedInput = hexInput.Replace("CC", "")
                                      .Replace("AA", "")
                                      .Replace("55", "")
                                      .Replace("33", "")
                                      .Replace(" ", ""); 

        if (cleanedInput.Length % 2 != 0)
        {
            ViewBag.Message = "Input contains an incomplete byte (odd number of characters).";
            return View("Index");
        }

        var hexPairs = Enumerable.Range(0, cleanedInput.Length / 2)
                                 .Select(i => cleanedInput.Substring(i * 2, 2))
                                 .ToList();

        var positions = hexPairs
            .Select((value, index) => new { value, index })
            .Where(pair => pair.value == searchValue)
            .Select(pair => pair.index + 1)
            .ToList();

        if (positions.Count == 0)
        {
            ViewBag.PositionMessage = $"Value {searchValue} not found.";
        }
        else
        {
            ViewBag.PositionMessage = $"Value {searchValue} found at position(s): {string.Join(", ", positions)}.";
        }

        ViewBag.TotalCount = TempData["TotalCount"];
        ViewBag.Crc = TempData["Crc"];

        return View("Index");
    }
}
