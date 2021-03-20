using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;

namespace CassandraCore.Models.Repos
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ISession _session;

        public StudentRepository(ISession session)
        {
            _session = session;
        }

        public void Add(Student student)
        {
            Execute(
                "insert into student (id, address, name) values (?,?,?);"
                , student.Id, student.Address, student.Name);
        }

        public IEnumerable<Student> All()
        {
            RowSet rows = _session.Execute("select id, name, address from student;");

            foreach (Row row in rows)
                yield return Create(row);
        }

        public IEnumerable<Student> GetStudentById(string id)
        {
            RowSet rows = _session.Execute("select * from student where id ="+ id);

            foreach (Row row in rows)
                yield return Create(row);         
        }

        public void Remove(string id)
        {
            Execute("delete from student where id =" + id);
        }

        public string Replace(Student student)
        {
            var prepairQuery = "UPDATE student SET address = '"+ student.Address + "', name = '"+ student.Name + "' WHERE id =" + student.Id;
            Execute(prepairQuery);
            return "";
        }

        #region Private Method
        private Student Create(Row row)
        {
            var student= new Student((Guid)row["id"],(string)row["name"],(string)row["address"]);
            return student;
        }

        private RowSet Execute(string sql, params object[] values)
        {
            var prepare = _session.Prepare(cqlQuery: sql);

            var statement = prepare.Bind(values: values);

            return _session.Execute(statement: statement);
        }
        #endregion
    }
}
