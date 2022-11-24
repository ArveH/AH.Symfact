﻿using System.Threading.Tasks;

namespace AH.Symfact.UI.Services;

public interface ISchemaService
{
    Task<bool> CreateCollectionAsync(string collectionName, string fileName);
    Task<bool> AddToCollectionAsync(string collectionName, string fileName);
}