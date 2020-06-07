using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Services.Interfaces
{
    /// <summary>
    /// Defines method signatures for methods that should be implemented for each object that interacts with the DB.
    /// </summary>
    public interface IDataService
    {
        /// Example methods to define per DTO
        /// 1: void Upsert(IUnitOfWork DaymsWork, DTO model); [Create / Update Item]
        /// 2: bool Delete(IUnitOfWork DaymsWork, DTO model); [Delete Item]
        /// 3: DTO GetItem(IUnitOfWork DaymsWork, Guid itemID); [Get Item matching this ID]
        /// 4: ObservableCollection<DTO> GetItems(IUnitOfWork DaymsWork); [Get All Items of this type]

    }
}
