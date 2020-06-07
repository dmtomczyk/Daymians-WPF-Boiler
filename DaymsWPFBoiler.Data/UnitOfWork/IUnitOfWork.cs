using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models.DTOs;
using DaymsWPFBoiler.Data.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DaymsWPFBoiler.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRepository<Roles> Roles { get; }

        int Complete();

        IDbContextTransaction BeginTransaction();
    }
}
