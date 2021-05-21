﻿namespace Entities.RequestFeatures {
    public class Metadata {
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
    }
}