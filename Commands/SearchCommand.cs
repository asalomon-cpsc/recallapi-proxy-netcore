
using System.ComponentModel.DataAnnotations;

namespace Opendata.Recalls.Commands{
public class SearchCommand{
    
    public Params Data { get; set; }
    public class Params {
    public string SearchFor { get; set; }
    public string ManufacturerName { get; set; }
    public string ProductName { get; set; }
    public string ProductType { get; set; }
    public string ProductModel { get; set; }
    public string  RecallDateEnd { get; set; }
    public string  RecallDateStart { get; set; }
    }
    
     
}


}