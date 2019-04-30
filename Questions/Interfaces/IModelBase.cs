using System;
using MongoDB.Bson;
namespace Questions.Interfaces
{
    public interface IModelBase
    {
        
        int Id { get; set; }
        ObjectId _Id { get; set; }
        DateTime DateOfCreation { get; set; }

    }
}
