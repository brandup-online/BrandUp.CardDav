# BrandUp.CardDav

BrandUp.CardDav is the library for work with CardDav protocol. At the moment library provides two packages: 

- **BrandUp.CardDav.Client** for executing requests to servers that supports CradDav protocol.
- **BrandUp.CardDav.Server** provides simple functional of cardDav server

## BrandUp.CardDav.Client

### Adding Client to dependencies 

```
Services.AddCardDavClient();
```

### Creating a client

```
 client = cardDavClientFactory.CreateClientWithCredentials("api", login, password);
 //or
 client = cardDavClientFactory.CreateClientWithAccessToken("api", token);
 
```
### Examples

#### Propfind request

```
var request = PropfindRequest.Create(Depth.One, Prop.ETag);
var response = await client.PropfindAsync("*Your CardDav  server*",request);
```

#### Report request

```
filter = new FilterBody();

filter.AddPropFilter(VCardProperty.FN, FilterMatchType.All, TextMatch.Create("ma", TextMatchType.Contains));

var addressData = new AddressData(VCardProperty.FN, VCardProperty.N, VCardProperty.EMAIL)
var request = ReportRequest.CreateQuery(PropList.Create(Prop.CTag, Prop.ETag), addressData, filter);
var response = await client.ReportAsync("*Your CardDav  server*", request);
```

#### Mkcol request

```
var response = await Client.MkcolAsync(endpoint);
```

## BrandUp.CardDav.Serer

### Adding Server to dependencies 
```
Services.AddCradDavServer()
        .AddRepositories<UserRepository, AddressBookRepository, ContactRepository>();
```

### Server endpoints
List of endpoints that providing by server

+ "Principal/{Name}/Collections"
+ "Principal/{Name}/Collections/{AddressBook}"
+ "Principal/{Name}/Collections/{AddressBook}/{Contact}"

Also server have controller for root endpoint fot http method OPTIONS 
