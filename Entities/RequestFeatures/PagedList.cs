using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities.RequestFeatures {
    public class PagedList<T> : List<T> {
        public Metadata Metadata { get; set; }

        public PagedList(IEnumerable<T> items,int count,int pageSize,int pageNum) {
            Metadata = new Metadata {
                PageSize = pageSize,
                Total = count,
                TotalPages = (int) Math.Ceiling(count / (double)pageSize ),
                CurrentPage = pageNum
            };
            AddRange(items);
        }

        public static async Task<PagedList<T>> ToPageList(IQueryable<T> source, int pageNum, int pageSize) {
            var pageList = await source.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(pageList, source.Count(),pageSize,pageNum);
        }
    }
}