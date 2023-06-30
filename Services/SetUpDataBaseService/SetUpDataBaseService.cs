using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goods.Data;
using Goods.Services;
using Microsoft.EntityFrameworkCore;

namespace Goods.Services.SetUpDataBaseService
{
    public class SetUpDataBaseService : ISetUpDataBaseService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public SetUpDataBaseService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ServiceResponse<string>> SetUpDataBase(CancellationToken token)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
        .UseSqlServer("Server=localhost; Database=goods; User Id=sa; password=reallyStrongPwd123; TrustServerCertificate=true;")
        .Options;
            using (var dbContext = new DataContext(options))
            {
                dbContext.Database.ExecuteSqlRaw(@"
                INSERT INTO Users (UserName, FullName,Password, Active)
                VALUES
                    (' U1', 'johndoe', 'P1', 1),
                    ('U2', 'janesmith', 'P2', 0),
                    ('Michael Johnson', 'mjohnson', 'password3', 0)");

                dbContext.Database.ExecuteSqlRaw(@"
                INSERT INTO BPType (TypeCode, TypeName)
                VALUES
                    ('C', 'Customer'),
                    ('V', 'Vendor')");

                dbContext.Database.ExecuteSqlRaw(@"
                INSERT INTO BusinessPartners (BPCode, BPName, BPType, Active)
                VALUES
                    ('C0001', 'Customer 1', 'C', 1),
                    ('C002', 'Customer 2', 'C', 0),
                    ('V001', 'Vendor 1', 'V', 1),
                    ('V002', 'Vendor 2', 'V', 0)");

                dbContext.Database.ExecuteSqlRaw(@"
                INSERT INTO Items (ItemCode, ItemName, Active)
                VALUES
                    ('Itm1', 'Item 1', 1),
                    ('Itm2', 'Item 2', 1),
                    ('Itm3', 'Item 3', 0)");
            }
            var serviceResponse = new ServiceResponse<string>();
            serviceResponse.Message = "DataBase is set up";
            return serviceResponse;
        }


    }
}