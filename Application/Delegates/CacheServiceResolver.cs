using Application.Interfaces;
using DAL.Enums;

namespace Application.Delegates;

public delegate ICacheService CacheServiceResolver(CachingType  cachingType); 