using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Contexts;
using DaymsWPFBoiler.Data.Models.DTOs;
using DaymsWPFBoiler.Data.Respositories;
using DaymsWPFBoiler.Data.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DaymsWPFBoiler.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DaymsContext _context;

        public UnitOfWork()
        {
            _context = new DaymsContext();

            Users = new UserRepository(_context);
            Roles = new Repository<Roles>(_context);
        }

        public IUserRepository Users { get; private set; }
        public IRepository<Roles> Roles { get; private set; }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
