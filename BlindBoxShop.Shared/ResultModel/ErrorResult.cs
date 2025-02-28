namespace BlindBoxShop.Shared.ResultModel
{
    public class ErrorResult
    {
        public required string Code { get; set; }

        public required string Description { get; set; }

        public ErrorResult() { }

        public ErrorResult(string code, string description)
        {
            Code = code;
            Description = description;
        }

    }
}
