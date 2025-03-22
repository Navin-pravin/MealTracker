using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace ProjectHierarchyApi.Services
{
    public class ProjectService
    {
        private readonly IMongoCollection<Project> _projects;

        public ProjectService(IMongoDatabase database)
        {
            _projects = database.GetCollection<Project>("Projects");
        }

        public async Task<List<Project>> GetAllProjectsAsync() =>
            await _projects.Find(_ => true).ToListAsync();

        public async Task<Project?> GetProjectByIdAsync(string id) =>
            await _projects.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateProjectAsync(Project project) =>
            await _projects.InsertOneAsync(project);

       public async Task<bool> UpdateProjectAsync(string id, Project updatedProject)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
        return false; // Invalid ID format

    var filter = Builders<Project>.Filter.Eq(p => p.Id, id);
    var result = await _projects.ReplaceOneAsync(filter, updatedProject);

    return result.ModifiedCount > 0;
}


        public async Task<bool> DeleteProjectAsync(string id)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
        return false; // Invalid ID format

    var filter = Builders<Project>.Filter.Eq(p => p.Id, id);
    var result = await _projects.DeleteOneAsync(filter);

    return result.DeletedCount > 0;
}

    }
}
