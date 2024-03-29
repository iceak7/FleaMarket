﻿using FleaMarket.Models;

namespace FleaMarket.Infrastructure.Repositories
{
    public interface IItemCategoryRepository : IGenericRepository<ItemCategory>
    {
        Task<List<ItemCategory>> GetByIds(List<int> ids);
    }
}
