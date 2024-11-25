using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;

namespace sktodo.Plugins
{
    public class ToDoPlugin
    {
        // In-memory storage for tasks
        private readonly List<ToDoTaskModel> tasks = new()
    {
        new ToDoTaskModel { Id = 1, Description = "Buy groceries", IsComplete = false },
        new ToDoTaskModel { Id = 2, Description = "Complete the report", IsComplete = false },
        new ToDoTaskModel { Id = 3, Description = "Plan the weekend trip", IsComplete = true }
    };

        [KernelFunction("get_tasks")]
        [Description("Gets a list of tasks and their current state")]
        [return: Description("An array of tasks")]
        public async Task<List<ToDoTaskModel>> GetTasksAsync()
        {
            return tasks;
        }

        [KernelFunction("add_task")]
        [Description("Adds a new task")]
        [return: Description("The added task")]
        public async Task<ToDoTaskModel> AddTaskAsync(string description, bool isComplete)
        {
            var newTask = new ToDoTaskModel
            {
                Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1,
                Description = description,
                IsComplete = isComplete
            };

            tasks.Add(newTask);
            return newTask;
        }

        [KernelFunction("change_task_state")]
        [Description("Changes the state of a task")]
        [return: Description("The updated task state; null if the task does not exist")]
        public async Task<ToDoTaskModel?> ChangeTaskStateAsync(int id, bool isComplete)
        {
            var task = tasks.FirstOrDefault(task => task.Id == id);

            if (task == null) return null;

            task.IsComplete = isComplete;
            return task;
        }

        [KernelFunction("update_task")]
        [Description("Updates an existing task")]
        [return: Description("The updated task; null if the task does not exist")]
        public async Task<ToDoTaskModel?> UpdateTaskAsync(int id, string description, bool isComplete)
        {
            var task = tasks.FirstOrDefault(task => task.Id == id);

            if (task == null) return null;

            task.Description = description;
            task.IsComplete = isComplete;
            return task;
        }

        [KernelFunction("delete_task")]
        [Description("Deletes a task")]
        [return: Description("True if the task was deleted; false if the task does not exist")]
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = tasks.FirstOrDefault(task => task.Id == id);

            if (task == null) return false;

            tasks.Remove(task);
            return true;
        }
    }

    public class ToDoTaskModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("is_complete")]
        public bool IsComplete { get; set; }
    }
}