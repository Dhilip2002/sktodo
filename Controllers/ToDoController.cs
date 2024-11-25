using sktodo.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sktodo.Models;
using sktodo.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Sprache;

namespace sktodo.Controllers
{
    [Route("[controller]/[action]")]
    public class ToDoController : ControllerBase
    {
        private readonly NLPContext _nlpContext;
        private readonly ToDoPlugin _toDoPlugin;

        // Inject the NLPContext and ToDoPlugin dependencies
        public ToDoController(NLPContext context, ToDoPlugin toDoPlugin)
        {
            _nlpContext = context;
            _toDoPlugin = toDoPlugin;
        }

        // Add a new ToDo (using database and plugin)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToDo input)
        {
            try
            {
                // Add task to the database
                _nlpContext.ToDos.Add(input);
                await _nlpContext.SaveChangesAsync();
                // Optionally, you could integrate with ToDoPlugin to add task
                //await _toDoPlugin.AddTaskAsync(input.Name, input.IsComplete);

                return Ok("ToDo added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Get all ToDos from the database
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _nlpContext.ToDos.ToListAsync();
            return Ok(result);
        }

        // Get all completed ToDos from the database
        [HttpGet]
        public async Task<IActionResult> GetAllCompleteStatus()
        {
            var result = await _nlpContext.ToDos./*Where(x => x.IsComplete).*/ToListAsync();
            return Ok(result);
        }

        // Get a single ToDo by ID from the database
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var result = await _nlpContext.ToDos.FindAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // Update an existing ToDo in the database
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ToDo input)
        {
            var result = await _nlpContext.ToDos.FindAsync(input.Id);
            if (result == null)
            {
                return NotFound();
            }

            //result.Name = input.Name;
            //result.IsComplete = input.IsComplete;

            await _nlpContext.SaveChangesAsync();
            return Ok("Updated successfully");
        }

        // Delete a ToDo by ID from the database
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _nlpContext.ToDos.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            _nlpContext.ToDos.Remove(result);
            await _nlpContext.SaveChangesAsync();
            return Ok("Removed successfully");
        }

        // Get all tasks from the ToDoPlugin (NLP Plugin)
        [HttpGet("plugin/tasks")]
        public async Task<IActionResult> GetTasksFromPlugin()
        {
            var tasks = await _toDoPlugin.GetTasksAsync();
            return Ok(tasks);
        }

        // Add a task via the ToDoPlugin (NLP Plugin)
        [HttpPost("plugin/tasks")]
        public async Task<IActionResult> AddTaskViaPlugin([FromBody] string description)
        {
            var newTask = await _toDoPlugin.AddTaskAsync(description, false);
            return CreatedAtAction(nameof(GetTasksFromPlugin), new { id = newTask.Id }, newTask);
        }

        // Change the state of a task via the ToDoPlugin
        [HttpPost("plugin/tasks/{id}/state")]
        public async Task<IActionResult> ChangeTaskStateViaPlugin(int id, [FromQuery] bool isComplete)
        {
            var updatedTask = await _toDoPlugin.ChangeTaskStateAsync(id, isComplete);
            return updatedTask == null ? NotFound() : Ok(updatedTask);
        }
    }
}










/*
using CrudWithNLP.Context;
//using CrudWithNLP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using sktodo.Models;

namespace sktodo.Controllers
{
    [Route("[controller]/[action]")]
    public class ToDoController : ControllerBase
    {
        private NLPContext _nlpContext; 

        public ToDoController(NLPContext context)
        {
            _nlpContext = context;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToDo input)
        {
            try
            {
                //ToDo todo = JsonConvert.DeserializeObject<ToDo>(input);
                var result = _nlpContext.ToDos.Add(input);
                await _nlpContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            return Ok("ToDo added successfully");
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _nlpContext.ToDos.ToListAsync();
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCompleteStatus()
        {
            var result = await _nlpContext.ToDos.Where(x => x.IsComplete).ToListAsync();
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var result = await _nlpContext.ToDos.FindAsync(id);
            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Update(ToDo input)
        {
            var result = await _nlpContext.ToDos.FindAsync(input.Id);
            if (result == null)
            {
                return NotFound();
            }
            result.Name = input.Name;
            result.IsComplete = input.IsComplete;

            await _nlpContext.SaveChangesAsync();
            return Ok("Updated successfully");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _nlpContext.ToDos.FindAsync(id);
            _nlpContext.ToDos.Remove(result);
            await _nlpContext.SaveChangesAsync();
            return Ok("Removed successfully");
        }
    }
}
*/