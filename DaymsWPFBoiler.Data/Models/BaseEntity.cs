using System;
using System.Reflection;
using DaymsWPFBoiler.Data.Utilities;
using GalaSoft.MvvmLight;

namespace DaymsWPFBoiler.Data.Models
{
    public class BaseEntity : ObservableObject
    {
        private Guid _id;
        private DateTime _dateCreated;
        private DateTime _dateModified;

        public Guid Id { get => _id; set => Set(ref _id, value); }
        public DateTime DateCreated { get => _dateCreated; set => Set(ref _dateCreated, value); }
        public DateTime DateModified { get => _dateModified; set => Set(ref _dateModified, value); }

        public void Convert(object o)
        {
            Type t = o.GetType();

            if (t.GetProperty("Id") is PropertyInfo p_id)
            {
                Id = (Guid)p_id.GetValue(o);
            }

            if (t.GetProperty("DateCreated") is PropertyInfo p_dc &&
                p_dc.GetValue(o) is string strDC &&
                !string.IsNullOrEmpty(strDC))
            {
                try
                {
                    DateCreated = DateTime.Parse(strDC);
                }
                catch (Exception)
                {
                    DateCreated = DateTime.Now;
                }
            }

            if (t.GetProperty("DateModified") is PropertyInfo p_dm &&
                p_dm.GetValue(o) is string strDM &&
                !string.IsNullOrEmpty(strDM))
            {
                try
                {
                    DateModified = DateTime.Parse(strDM);
                }
                catch (Exception)
                {
                    DateModified = DateTime.Now;
                }
            }
        }

        public void ConvertBack(object o)
        {
            if (null != Id)
            {
                Type t = o.GetType();

                if (t.GetProperty("Id") is PropertyInfo p_id)
                {
                    p_id.SetValue(o, Id);
                }

                if (t.GetProperty("DateCreated") is PropertyInfo p_dc)
                {
                    p_dc.SetValue(o, DataFunctions.ToSQLiteDT(DateCreated));
                }

                if (t.GetProperty("DateModified") is PropertyInfo p_dm)
                {
                    p_dm.SetValue(o, ""); // Let database provide modification datetime.
                }
            }
        }
    }
}
