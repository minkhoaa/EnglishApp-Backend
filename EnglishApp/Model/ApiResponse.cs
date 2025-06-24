namespace EnglishApp.Model
{
    public class ApiResponse
    {
        public bool Success { get; set; }  
        public string Message {  get; set; } = string.Empty;
        public object? Data { get; set; } = null!;


        // Constructor cho response chỉ với success & message
        
    }
}
