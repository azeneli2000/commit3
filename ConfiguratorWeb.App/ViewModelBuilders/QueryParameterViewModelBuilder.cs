using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model.Online;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public class QueryParameterViewModelBuilder
    {
        public static QueryParameterViewModel Build(OnlineQuery source)
        {
            QueryParameterViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new QueryParameterViewModel
                    {
                        QueryID = source.Id,
                        ParameterLabel = source.ParameterLabel,
                        UnitLabel = source.UnitLabel,
                        Description = source.Description,
                        SQLQuery = source.Query,
                        LastSaveUserID = source.LastSaveUserId,
                        LastSaveDatetime = source.LastSaveDatetime,
                        LastSaveUserName = ""
                    };
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<QueryParameterViewModel> BuildList(IEnumerable<OnlineQuery> source)
        {
            try
            {
                return source.Select(Build);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
