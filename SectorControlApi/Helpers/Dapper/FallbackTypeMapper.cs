using Dapper;
using NLog;
using System.Reflection;

namespace SectorControlApi.Helpers.Dapper
{
	/*
	 * Implementation of custom column mapper based on following sources
	 * https://stackoverflow.com/questions/8902674/manually-map-column-names-with-class-properties - Kaleb Pederson answer
	 * https://gist.github.com/senjacob/8539127
	 */

	public class FallbackTypeMapper : SqlMapper.ITypeMap
	{
		private readonly IEnumerable<SqlMapper.ITypeMap> _mappers;
		private readonly NLog.ILogger _logger;

		public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
		{
			_mappers = mappers;
			_logger = LogManager.GetCurrentClassLogger();
		}


		public ConstructorInfo FindConstructor(string[] names, Type[] types)
		{
			foreach (var mapper in _mappers)
			{
				try
				{
					ConstructorInfo result = mapper.FindConstructor(names, types);
					if (result != null)
					{
						return result;
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}
			}
			return null;
		}

		public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
		{
			foreach (var mapper in _mappers)
			{
				try
				{
					var result = mapper.GetConstructorParameter(constructor, columnName);
					if (result != null)
					{
						return result;
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}
			}
			return null;
		}

		public SqlMapper.IMemberMap GetMember(string columnName)
		{
			foreach (var mapper in _mappers)
			{
				try
				{
					var result = mapper.GetMember(columnName);
					if (result != null)
					{
						return result;
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}
			}
			return null;
		}

		public ConstructorInfo FindExplicitConstructor()
		{
			return _mappers
				.Select(mapper => mapper.FindExplicitConstructor())
				.FirstOrDefault(result => result != null);
		}
	}
}
