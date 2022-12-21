using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Requests.Body.Mkcol;
using BrandUp.CardDav.VCard;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests.Controllers
{
    public class CollectionsTests : ControllerTestBase
    {
        public CollectionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task Success_Propfind()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("Principal/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
            Assert.Single(propfind.Body.Resources);

            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            propfind = await Client.PropfindAsync("Principal/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
            Assert.Equal(2, propfind.Body.Resources.Count);
        }

        [Fact]
        public async Task Success_Propfind_Addressbook()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("Principal/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
            Assert.Single(propfind.Body.Resources);

            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            propfind = await Client.PropfindAsync("Principal/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
            Assert.Equal(4, propfind.Body.Resources.Count);
        }

        [Fact]
        public async Task Success_Report_Addressbook()
        {
            #region Addresbook-query no filter, not empty address-data, no limit

            var filter = new Filter()
            {

            };

            var addressData = Prop.AddressData(VCardProperty.EMAIL, VCardProperty.VERSION, VCardProperty.TEL);
            var request = ReportRequest.CreateQuery(Depth.Zero, PropList.Create(Prop.ETag, Prop.CTag, addressData), filter);

            var report = await Client.ReportAsync("Principal/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(report.StatusCode);
            Assert.Equal(3, report.Body?.Resources.Count);

            #endregion

            #region Addresbook-query with filter 

            filter = new Filter()
            {
                MatchType = FilterMatchType.All
            };
            filter.AddPropFilter(VCardProperty.EMAIL, FilterMatchType.Any, TextMatch.Create("hn", TextMatchType.Contains));
            filter.AddPropFilter
                (
                    VCardProperty.ORG,
                    FilterMatchType.All,
                    TextMatch.Create("Example", TextMatchType.Contains),
                    TextMatch.Create("com", TextMatchType.Contains, true)
                );

            addressData = Prop.AddressData();
            request = ReportRequest.CreateQuery(Depth.Zero, PropList.Create(Prop.ETag, Prop.CTag, addressData), filter);

            report = await Client.ReportAsync("Principal/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(report.StatusCode);
            Assert.Equal(3, report.Body?.Resources.Count);

            #endregion

            #region Multiget report not empty address-data

            var propfindRequest = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("Principal/User/Collections/Default", propfindRequest, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);

            addressData = Prop.AddressData(VCardProperty.EMAIL, VCardProperty.TEL, VCardProperty.ORG, VCardProperty.VERSION);
            request = ReportRequest.CreateMultiget(Depth.Zero, PropList.Create(Prop.ETag, addressData), propfind.Body.Resources[1].Endpoint, propfind.Body.Resources[2].Endpoint);

            report = await Client.ReportAsync("Principal/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(report.StatusCode);
            Assert.Equal(2, report.Body?.Resources.Count);

            #endregion
        }

        [Fact]
        public async Task Success_Mkcol_Addressbook()
        {
            var request = new MkcolRequest { Body = new SetPropBody("new-coll", "new collection") };

            var propfind = await Client.MkcolAsync("Principal/User/Collections/New", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }
    }
}
