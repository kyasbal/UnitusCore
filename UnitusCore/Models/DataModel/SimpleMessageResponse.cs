namespace UnitusCore.Models.DataModel
{
    public class SimpleMessageResponse
    {
        public SimpleMessageResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}