namespace Market.Application.Modules.Sales.Orders.Commands.Status
{
    public class ChangeOrderStatusCommand : IRequest
    {
        public int Id { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
