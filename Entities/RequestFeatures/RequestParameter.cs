namespace Entities.RequestFeatures {
    public abstract class RequestParameter {
        const int MaxSize = 50;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 5;
        public int PageSize {
            get => pageSize;
            set => pageSize = value > MaxSize ? MaxSize : value;
        }

        public string Query { get; set; }
        public string OrderBy { get; set; }
        public string Fields { get; set; }
    }
}