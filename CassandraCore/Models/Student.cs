using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassandraCore.Models
{
    public class Student
    {
        public Student(Guid id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }
}
