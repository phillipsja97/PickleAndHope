using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickleAndHope.Models;
using PickleAndHope.DataAccess;

namespace PickleAndHope.Controllers
{
    [Route("api/pickles")]
    [ApiController]
    public class PicklesController : ControllerBase
    {
        PickleRepository _repository;

        public PicklesController(PickleRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult AddPickle(Pickle pickleToAdd)
        {
            var existingPickle = _repository.GetByType(pickleToAdd.Type);
            if (existingPickle == null)
            {
                var newPickle = _repository.Add(pickleToAdd);
                return Created("", newPickle);
            }
            else
            {
                var updatedPickle = _repository.Update(pickleToAdd);
                return Ok(updatedPickle);
            }

        }
        
        [HttpGet("all")]
        public IActionResult GetAllPickles()
        {
            var allPickles = _repository.GetAll();
            return Ok(allPickles);
        }

        [HttpGet("{id}")]
        public IActionResult GetPickleById(int id)
        {
            var pickle = _repository.GetById(id);

            if (pickle == null) return NotFound("No pickle with that id could be found.");

            return Ok(pickle);
        }

        [HttpGet("type/{type}")]
        public IActionResult GetPickleByType(string type)
        {
            var pickle = _repository.GetByType(type);

            if (pickle == null) return NotFound("no pickle found");

            return Ok(pickle);
        }
    }
}