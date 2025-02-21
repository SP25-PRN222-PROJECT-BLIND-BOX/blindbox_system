namespace BlindBoxShop.Shared.Features
{
    public class VoucherParameter : RequestParameters
    {
        public string? SearchById { get; set; }

        public VoucherParameter()
        {
            OrderBy = "Id";
        }
    }
}
