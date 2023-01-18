using BrandUp.CardDav.Server.Controllers.Tests;
using BrandUp.CardDav.Transport.Abstract.Enum;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses;
using BrandUp.CardDav.VCard;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Tests.Controllers
{
    public class CollectionsTests : ControllerTestBase
    {
        public CollectionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task Success_Propfind()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.CTag);

            var propfind = await Client.PropfindAsync("principals/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);
            Assert.Single(propfind.Body.Resources);

            request = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.CTag);

            propfind = await Client.PropfindAsync("principals/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);
            Assert.Equal(2, propfind.Body.Resources.Count);

            foreach (var resource in propfind.Body.Resources)
            {
                Assert.NotNull(resource.Endpoint);
                Assert.Single(resource.FoundProperties);
                Assert.Single(resource.NotFoundProperties);
            }
        }

        [Fact]
        public async Task Success_Propfind_Addressbook()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("principals/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);
            Assert.Single(propfind.Body.Resources);

            request = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.CTag);

            propfind = await Client.PropfindAsync("principals/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);
            Assert.Equal(4, propfind.Body.Resources.Count);

            foreach (var resource in propfind.Body.Resources)
            {
                Assert.NotNull(resource.Endpoint);
                Assert.Single(resource.FoundProperties);
                Assert.Single(resource.NotFoundProperties);
            }
        }

        [Fact]
        public async Task Success_Report_Addressbook_NoFilter()
        {
            #region Addresbook-query no filter, not empty address-data, no limit

            var filter = new FilterBody()
            {

            };

            var addressData = new AddressData(CardProperty.EMAIL, CardProperty.VERSION, CardProperty.TEL);
            var request = ReportRequest.CreateQuery(PropList.Create(Prop.ETag, Prop.CTag), addressData, filter);

            var report = await Client.ReportAsync("principals/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(report.StatusCode.ToString());

            Assert.True(report.IsSuccess);
            Assert.Equal(3, report.Body?.Resources.Count);

            foreach (var resource in report.Body.Resources)
            {
                Assert.NotNull(resource.Endpoint);
                Assert.Equal(2, resource.FoundProperties.Count);
                Assert.Single(resource.NotFoundProperties);
            }

            #endregion

            #region Multiget report not empty address-data

            var propfindRequest = PropfindRequest.Create(Depth.One, Prop.ETag);

            var propfind = await Client.PropfindAsync("principals/User/Collections/Default", propfindRequest, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);

            addressData = new AddressData(CardProperty.EMAIL, CardProperty.TEL, CardProperty.ORG, CardProperty.VERSION);
            request = ReportRequest.CreateMultiget(PropList.Create(Prop.ETag), addressData, propfind.Body.Resources[1].Endpoint, propfind.Body.Resources[2].Endpoint);

            report = await Client.ReportAsync("principals/User/Collections/Default", request, CancellationToken.None);

            Output.WriteLine(report.StatusCode.ToString());
            Assert.Equal(2, report.Body?.Resources.Count);

            foreach (var resource in report.Body.Resources)
            {
                Assert.NotNull(resource.Endpoint);
                Assert.Equal(2, resource.FoundProperties.Count);
                Assert.Empty(resource.NotFoundProperties);
            }

            #endregion
        }

        [Fact]
        public async Task Success_Report_Addressbook_Filter()
        {
            #region Textmatch filter

            var filter = new FilterBody()
            {
                MatchType = FilterMatchType.All
            };
            filter.AddPropFilter(CardProperty.EMAIL, FilterMatchType.Any, TextMatch.Create("hn", TextMatchType.Contains));
            filter.AddPropFilter
                (
                    CardProperty.ORG,
                    FilterMatchType.All,
                    TextMatch.Create("Example", TextMatchType.Contains),
                    TextMatch.Create("com", TextMatchType.Contains, true)
                );

            var report = await ExecWithFilter(filter);

            Output.WriteLine(report.StatusCode.ToString());

            Assert.True(report.IsSuccess);
            Assert.Equal(1, report.Body?.Resources.Count);
            Assert.NotNull(report.Body.Resources.First().Endpoint);
            Assert.Equal(TestVCards.VCard2, new(report.Body.Resources.First().FoundProperties[Prop.AddressData]));

            #endregion

            #region ParamFilter is not defined filter

            filter = new FilterBody()
            {
                MatchType = FilterMatchType.All
            };
            filter.AddPropFilter(CardProperty.LANG, FilterMatchType.Any, ParamFilter.NotDefined(CardParameter.TYPE));

            report = await ExecWithFilter(filter);

            Output.WriteLine(report.StatusCode.ToString());

            Assert.True(report.IsSuccess);

            Assert.Equal(1, report.Body?.Resources.Count);
            Assert.NotNull(report.Body.Resources.First().Endpoint);
            Assert.Equal(TestVCards.VCard2, new(report.Body.Resources.First().FoundProperties[Prop.AddressData]));

            #endregion

            #region ParamFilter text match filter

            filter = new FilterBody()
            {
                MatchType = FilterMatchType.All
            };
            filter.AddPropFilter(CardProperty.LANG, FilterMatchType.Any, ParamFilter.TextMatch(CardParameter.TYPE, "wOrK", TextMatchType.Equals));

            report = await ExecWithFilter(filter);

            Output.WriteLine(report.StatusCode.ToString());

            Assert.True(report.IsSuccess);

            Assert.Equal(1, report.Body?.Resources.Count);
            Assert.NotNull(report.Body.Resources.First().Endpoint);
            Assert.Equal(TestVCards.VCard1, new(report.Body.Resources.First().FoundProperties[Prop.AddressData]));

            #endregion

            #region IsNotDefined filter

            filter = new FilterBody()
            {
                MatchType = FilterMatchType.All
            };
            filter.AddPropFilter(CardProperty.NICKNAME, FilterMatchType.Any, new IsNotDefined());

            report = await ExecWithFilter(filter);

            Output.WriteLine(report.StatusCode.ToString());

            Assert.True(report.IsSuccess);

            Assert.Equal(1, report.Body?.Resources.Count);
            Assert.NotNull(report.Body.Resources.First().Endpoint);
            Assert.Equal(TestVCards.VCard3, new(report.Body.Resources.First().FoundProperties[Prop.AddressData]));

            #endregion
        }

        [Fact]
        public async Task Success_Mkcol_Addressbook()
        {
            var mkcol = await Client.MkcolAsync("principals/User/Collections/New", CancellationToken.None);

            Output.WriteLine(mkcol.StatusCode.ToString());
            Assert.True(mkcol.IsSuccess);

            var request = PropfindRequest.AllProp(Depth.Zero);
            var propfind = await Client.PropfindAsync("principals/User/Collections/New", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.True(propfind.IsSuccess);
            Assert.Single(propfind.Body.Resources);
            Assert.Equal("/principals/User/Collections/New", propfind.Body.Resources.First().Endpoint);
            Assert.NotNull(propfind.Body.Resources.First().FoundProperties[Prop.CTag]);
        }

        [Fact]
        public async Task Failure_NotExistingUser()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("principals/NotExistingUser/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode.ToString());
            Assert.False(propfind.IsSuccess);
        }

        #region Helpers 
        private Task<ReportResponse> ExecWithFilter(FilterBody filter)
        {
            var addressData = new AddressData();
            var request = ReportRequest.CreateQuery(PropList.Create(Prop.ETag, Prop.CTag), addressData, filter);

            return Client.ReportAsync("principals/User/Collections/Default", request, CancellationToken.None);
        }

        #endregion
    }
}