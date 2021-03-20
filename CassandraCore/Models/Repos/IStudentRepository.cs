using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassandraCore.Models.Repos
{
   public interface IStudentRepository
    {
        IEnumerable<Student> GetStudentById(string id);
        IEnumerable<Student> All();
        void Remove(string id);
        string Replace(Student student);
        void Add(Student student);
    }
}
