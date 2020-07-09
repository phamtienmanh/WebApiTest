using System.Collections.Generic;
using WebApiTest.DataAccess.Entities;

namespace WebApiTest.DataAccess.Repositories
{
    public interface IAppDbRepository
    {
        bool Save();
    }
}
