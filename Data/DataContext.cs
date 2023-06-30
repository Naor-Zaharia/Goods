using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Goods.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }


        public DbSet<Users> Users { get; set; }
        public DbSet<BPType> BPType { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<BusinessPartners> BusinessPartners { get; set; }
        public DbSet<PurchaseOrders> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrdersLines> PurchaseOrdersLines { get; set; }
        public DbSet<SaleOrders> SaleOrders { get; set; }
        public DbSet<SaleOrdersLines> SaleOrdersLines { get; set; }
        public DbSet<SaleOrdersLinesComments> SaleOrdersLinesComments { get; set; }
    }
}