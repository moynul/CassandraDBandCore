using CassandraCore.Models;
using CassandraCore.Models.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassandraCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;

        public StudentController( IStudentRepository StudentRepo)
        {
            _studentRepo = StudentRepo;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Student>> GetApplicant()
        {
            try
            {
                var student = _studentRepo.All();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Student>> GetApplicant(Guid id)
        {
            try
            {
                var student = _studentRepo.GetStudentById(id.ToString());
                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [Route("CreateApplicant")]
        public ActionResult<Student> CreateApplicant([FromBody] Student studentobject)
        {
            try
            {
                var id = Guid.NewGuid(); 
                var result = new Student(id, studentobject.Name, studentobject.Address);
                _studentRepo.Add(result);
                return Ok(StatusCode(201));
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateApplicant(Guid id, Student Studentobject)
        {
            try
            {
                var student = _studentRepo.GetStudentById(id.ToString()).ToList();
                if (student.Count() > 0)
                {
                   var studentobj = student.FirstOrDefault();
                    studentobj.Address = Studentobject.Address;
                    studentobj.Name = Studentobject.Name;
                    _studentRepo.Replace(studentobj);
                    return StatusCode(StatusCodes.Status200OK, "User updated");
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User Not found");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteApplicant(Guid id)
        {
            try
            {

                var student = _studentRepo.GetStudentById(id.ToString());
                if (student.Count()>0)
                {
                    _studentRepo.Remove(id.ToString());
                    return StatusCode(StatusCodes.Status200OK, "User deleted");
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User Not found");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
