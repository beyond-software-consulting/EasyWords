using System;
using System.Collections.Generic;

namespace Questions.Interfaces
{
    public interface IDatabaseRepository<TI> where TI: class
    {



        TI Add(TI item);
        void AddRange(IList<TI> Items);

        TI Update(TI item);
        void Delete(TI item);

        TI GetById(int id);
        IList<TI> GetAll();
        IList<TI> GetByFilter<TFilter>(TFilter filter);
    }
}
