using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationLookup.Models
{
    public interface ILocationApi
    {
        Task<IEnumerable<Destination>> GetItems(WayPoint userLocation, int numItems = 3);

        Task<string> GetMapImageUrl(WayPoint p1, WayPoint p2);

        Task<string> GetRouteDistance(WayPoint p1, WayPoint p2);
    }
}
